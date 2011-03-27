module MinJ.Ast.TypeCheck

open Compiler
open Scanner.Tokens
open MinJ.Ast
open System.IO
open System
open System.Collections.Generic

/// Instances of this type are used for doing semantic verification 
/// on MinJ AST nodes.
/// The class does not have any state except for a collection of 
/// errors encountered while checking.
type TypeChecker() =
    
    /// Accumulator for errors encountered
    let mutable errors = new List<CompilationError>()

    /// Helper function for checking two types
    let checkTypes (typ1 : MinJType, id1 : Token) (typ2 : MinJType, id2 : Token) = 
        if typ1 <> typ2 then
            errors.Add("Unexpected type", id1.StartLocation)

    /// Helper function for checking lists for types.
    /// This is used for comparing types of function call arguments.
    let checkTypeLists (types1 : (MinJType * Token) list) (types2 : (MinJType * Token) list) =
        for a, b in List.zip types1 types2 do
            checkTypes a b

    member this.Errors with get() = errors

    member this.TypeCheck ast  =
        match ast with
            | VariableReference(varId, None) -> 
                varId.Attributes.Value.Type, varId.Token :> Token
            | VariableReference(varId, Some exp) -> 
                match varId.Attributes.Value.Type with
                    | ArrayType(typ) as arrayType ->
                        Primitive(typ), varId.Token :> Token
                    | typ ->
                        errors.Add("Expected array type", varId.Token.StartLocation)
                        typ, varId.Token :> Token

    member this.TypeCheck ast = 
        match ast with
            | VariableElement(varRef) ->
                this.TypeCheck varRef
            | NumberElement(n) ->
                Primitive(IntType), n :> Token
            | CharConstElement(c) ->
                Primitive(CharType), c :> Token
  
    member this.TypeCheck (TermP(operator, primitive, rest)) =
        let primType, (token : Token) = this.TypeCheck primitive
        if primType <> Primitive(IntType) then
            errors.Add("* / % operators may only be used on values of type \"int\"", token.StartLocation)
        match rest with
            | Some(termP) ->
                let rest = this.TypeCheck termP
                checkTypes rest (primType, token)
                primType, token
            | None ->
                primType, token

    member this.TypeCheck (Term(primitive, termP)) =
        let primType, token = this.TypeCheck primitive
        match termP with
            | Some(termP) ->
                if primType <> Primitive(IntType) then
                    errors.Add("* / % operators may only be used on values of type \"int\"", token.StartLocation)
                this.TypeCheck(termP)
            | None -> 
                primType, token

    member this.TypeCheck (ExpressionPrime(op, term, rest)) =
        let termType = this.TypeCheck term
        match rest with
            | Some(rest) -> checkTypes termType <| this.TypeCheck rest
            | _ -> ()
        termType

    member this.TypeCheck(Expression(bool, term, rest)) =
        let termType, token = this.TypeCheck term
        if bool && termType <> Primitive(IntType) then
            errors.Add("Only expressions of type int can be negated", token.StartLocation)
        if rest.IsSome then
            let restType = this.TypeCheck rest.Value
            checkTypes (termType, token) restType
        termType, token

    member this.TypeCheck(ast : Primitive) =
        match ast with
            | VariablePrimitive(varRef) ->
                this.TypeCheck varRef
            
            | NumberPrimitive(n) -> 
                Primitive IntType, n :> Token
            
            | CharPrimitive(c) -> 
                Primitive CharType, c :> Token
            
            | ParenPrimitive(exp) ->
                this.TypeCheck exp

            | MethodInvocationPrimitive(funcId, arguments) ->
                let argTypes = List.map (fun (a : Element) -> this.TypeCheck a) arguments
                checkTypeLists funcId.Attributes.Value.ParameterTypes argTypes
                funcId.Attributes.Value.ReturnType, funcId.Token :> Token

    member this.TypeCheck (RelativeExpression(expLeft, relOp, expRight)) =
        let typeLeft = this.TypeCheck expLeft
        let typeRight = this.TypeCheck expRight
        checkTypes typeLeft typeRight

    member this.TypeCheck (ast : LogicalExpression) =
        match ast with
            | SingletonLogicalExpression(relExp) ->
                this.TypeCheck relExp
            | LogicalExpression(relExp, logOp, logExp) ->
                this.TypeCheck relExp
                this.TypeCheck logExp

    member this.TypeCheck (ast : Assignment) =
        match ast with
            | ExpressionAssignment(varRef, expression) ->
                checkTypes (this.TypeCheck varRef) (this.TypeCheck expression)
            | SystemInAssignment(assigned, inType) ->
                let assignedType, token = this.TypeCheck assigned
                let expected = Primitive(inType)
                if assignedType <> expected then
                    errors.Add("Incompatible type used with System.in", token.StartLocation)

    member this.TypeCheck(ast, funcId : FunctionIdentifier option) =
        match ast with
            | Block(statements) ->
                let returnFound = ref false
                for stmt in statements do
                    returnFound := !returnFound || this.TypeCheck(stmt, funcId)
                !returnFound
            
            | AssignmentStatement(assignment) ->
                this.TypeCheck assignment
                false

            | IfElse(lExp, ifStatement, elseStatement) ->
                this.TypeCheck lExp
                this.TypeCheck(ifStatement, funcId) 
                    && this.TypeCheck(elseStatement, funcId)

            | WhileStatement(logicExp, body) ->
                this.TypeCheck logicExp
                this.TypeCheck(body, funcId)

            | ReturnStatement(exp) ->
                let expType, expToken = this.TypeCheck exp
                match funcId with
                    | Some(funcId) ->
                        let funcType = funcId.Attributes.Value.ReturnType, funcId.Token :> Token
                        checkTypes (expType, expToken) funcType
                    | _ ->
                        errors.Add("The main function cannot have a return statement.", expToken.StartLocation)
                true

            | MethodInvocationStatement(identifier, arguments) ->
                let argTypes = List.map (fun (a : Element) -> this.TypeCheck a) arguments
                checkTypeLists argTypes identifier.Attributes.Value.ParameterTypes
                false
            
            | SystemOutInvocation(arguments) -> false
            
            | EmptyStatement -> false
    

    member this.TypeCheck(FunctionBody(decls, stmts), funcId) =
        let returnFound = ref false
        for stmt in stmts do
            returnFound := !returnFound || this.TypeCheck(stmt, funcId)
        !returnFound
    
    member this.TypeCheck (MainFunction(body)) =
        this.TypeCheck(body, None) |> ignore
    
    member this.TypeCheck (FunctionDefinition(funcId, _, body)) =
        if not <| this.TypeCheck(body, Some funcId) then
            errors.Add("Not all paths return a value", funcId.Token.StartLocation)
    
    member this.TypeCheck (Program(decls, mainF, funcDefs)) =
        this.TypeCheck mainF
        for funcDef in funcDefs do 
            this.TypeCheck funcDef

/// Type checks are MinJ Ast Program node and all of its children.
/// If an error is encountered, a CompilerException is thrown
/// with all the encountered errors.
let typeCheck (prg : Program) =
    let typeCheck = TypeChecker()
    typeCheck.TypeCheck prg
    if (typeCheck.Errors.Count > 0) then
        raise <| CompilerException(typeCheck.Errors)