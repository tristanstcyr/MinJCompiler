module MinJ.Ast.SemanticVerification

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
type SemanticVerifier() =
    
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

    member this.Verify ast  =
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

    member this.Verify ast = 
        match ast with
            | VariableElement(varRef) ->
                this.Verify varRef
            | NumberElement(n) ->
                Primitive(IntType), n :> Token
            | CharConstElement(c) ->
                Primitive(CharType), c :> Token
  
    member this.Verify (TermP(operator, primitive, rest)) =
        let primType, (token : Token) = this.Verify primitive
        if primType <> Primitive(IntType) then
            errors.Add("* / % operators may only be used on values of type \"int\"", token.StartLocation)
        match rest with
            | Some(termP) ->
                let rest = this.Verify termP
                checkTypes rest (primType, token)
                primType, token
            | None ->
                primType, token

    member this.Verify (Term(primitive, termP)) =
        let primType, token = this.Verify primitive
        match termP with
            | Some(termP) ->
                if primType <> Primitive(IntType) then
                    errors.Add("* / % operators may only be used on values of type \"int\"", token.StartLocation)
                this.Verify(termP)
            | None -> 
                primType, token

    member this.Verify (ExpressionPrime(op, term, rest)) =
        let termType = this.Verify term
        match rest with
            | Some(rest) -> checkTypes termType <| this.Verify rest
            | _ -> ()
        termType

    member this.Verify(Expression(bool, term, rest)) =
        let termType, token = this.Verify term
        if bool && termType <> Primitive(IntType) then
            errors.Add("Only expressions of type int can be negated", token.StartLocation)
        if rest.IsSome then
            let restType = this.Verify rest.Value
            checkTypes (termType, token) restType
        termType, token

    member this.Verify(ast : Primitive) =
        match ast with
            | VariablePrimitive(varRef) ->
                this.Verify varRef
            
            | NumberPrimitive(n) -> 
                Primitive IntType, n :> Token
            
            | CharPrimitive(c) -> 
                Primitive CharType, c :> Token
            
            | ParenPrimitive(exp) ->
                this.Verify exp

            | MethodInvocationPrimitive(funcId, arguments) ->
                let argTypes = List.map (fun (a : Element) -> this.Verify a) arguments
                checkTypeLists funcId.Attributes.Value.ParameterTypes argTypes
                funcId.Attributes.Value.ReturnType, funcId.Token :> Token

    member this.Verify (RelativeExpression(expLeft, relOp, expRight)) =
        let typeLeft = this.Verify expLeft
        let typeRight = this.Verify expRight
        checkTypes typeLeft typeRight

    member this.Verify (ast : LogicalExpression) =
        match ast with
            | SingletonLogicalExpression(relExp) ->
                this.Verify relExp
            | LogicalExpression(relExp, logOp, logExp) ->
                this.Verify relExp
                this.Verify logExp

    member this.Verify (ast : Assignment) =
        match ast with
            | ExpressionAssignment(varRef, expression) ->
                checkTypes (this.Verify varRef) (this.Verify expression)
            | SystemInAssignment(assigned, inType) ->
                let assignedType, token = this.Verify assigned
                let expected = Primitive(inType)
                if assignedType <> expected then
                    errors.Add("Incompatible type used with System.in", token.StartLocation)

    member this.Verify(ast, funcId : FunctionIdentifier option) =
        match ast with
            | Block(statements) ->
                let returnFound = ref false
                for stmt in statements do
                    returnFound := !returnFound || this.Verify(stmt, funcId)
                !returnFound
            
            | AssignmentStatement(assignment) ->
                this.Verify assignment
                false

            | IfElse(lExp, ifStatement, elseStatement) ->
                this.Verify lExp
                this.Verify(ifStatement, funcId) 
                    && this.Verify(elseStatement, funcId)

            | WhileStatement(logicExp, body) ->
                this.Verify logicExp
                this.Verify(body, funcId) |> ignore
                false

            | ReturnStatement(exp) ->
                let expType, expToken = this.Verify exp
                match funcId with
                    | Some(funcId) ->
                        let funcType = funcId.Attributes.Value.ReturnType, funcId.Token :> Token
                        checkTypes (expType, expToken) funcType
                    | _ ->
                        errors.Add("The main function cannot have a return statement.", expToken.StartLocation)
                true

            | MethodInvocationStatement(identifier, arguments) ->
                let argTypes = List.map (fun (a : Element) -> this.Verify a) arguments
                checkTypeLists argTypes identifier.Attributes.Value.ParameterTypes
                false
            
            | SystemOutInvocation(arguments) -> false
            
            | EmptyStatement -> false
    

    member this.Verify(FunctionBody(decls, stmts), funcId) =
        let returnFound = ref false
        for stmt in stmts do
            returnFound := !returnFound || this.Verify(stmt, funcId)
        !returnFound
    
    member this.Verify (MainFunction(body)) =
        this.Verify(body, None) |> ignore
    
    member this.Verify (FunctionDefinition(funcId, _, body)) =
        if not <| this.Verify(body, Some funcId) then
            errors.Add("Not all paths return a value", funcId.Token.StartLocation)
    
    member this.Verify (Program(decls, mainF, funcDefs)) =
        this.Verify mainF
        for funcDef in funcDefs do 
            this.Verify funcDef

/// Type checks are MinJ Ast Program node and all of its children.
/// If an error is encountered, a CompilerException is thrown
/// with all the encountered errors.
let verify (prg : Program) =
    let verifier = SemanticVerifier()
    verifier.Verify prg
    if (verifier.Errors.Count > 0) then
        raise <| CompilerException(verifier.Errors)
    prg