module MinJ.Semantics

open Compiler
open MinJ

open System.IO
open System
open System.Collections.Generic

module Errors =
    
    let UnexpectedType actual expected (location : Location) =
        "Unexpected type", location

    let WrongNumberOfArguments actualCount expectedCount (location : Location) =
        sprintf "Wrong number of argument. Expected %i but is %i" actualCount expectedCount, location
    
    let VariableNotAnArray (token : Identifier) =
        sprintf "Variable \"%s\" is not an array." token.Value, token.StartLocation
    
    let MathWithNonIntegerOperands (token : Token) =
        sprintf "Math operators may only be used on values of type \"int\"", token.StartLocation
    
    let MainCannotHaveReturn (location : Location) =
        sprintf "The main function cannot have a return statement.", location
    
    let NotAllPathsReturn (funcId : FunctionIdentifier) =
        sprintf "Not all paths return a value for function %s" funcId.Token.Value, funcId.Token.StartLocation

    let FunctionHasDeadCode (funcId : FunctionIdentifier) =
        sprintf "Function %s contains dead code." funcId.Token.Value, funcId.Attributes.Value.Definition.StartLocation

    let ArrayTypeCannotBePrinted location =
        "Arrays cannot be passed to System.out", location

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
            errors.Add(Errors.UnexpectedType typ1 typ2 id1.StartLocation)

    /// Helper function for checking lists for types.
    /// This is used for comparing types of function call arguments.
    let checkTypeLists (types1 : (MinJType * Token) list) (types2 : (MinJType * Token) list) =
        if types1.Length <> types2.Length then
            let (typ, token) = types1.Head
            errors.Add(Errors.WrongNumberOfArguments types1.Length types2.Length token.StartLocation)
        else
            for a, b in List.zip types1 types2 do
                checkTypes a b

    member this.VerifyStatementBlock(stmts : Statement list, funcId : FunctionIdentifier option) =
        // Any statements following a return statement is dead code.
        let rec verify previousReturns (stmts : Statement list) =
            match stmts with
                | stmt :: rest ->
                    if previousReturns then
                        errors.Add(Errors.FunctionHasDeadCode funcId.Value)
                        // Continue parsing the remaining statements without
                        // producing any further errors
                        for stmt in stmt :: rest do
                            this.Verify(stmt, funcId) |> ignore
                        true
                    else
                        verify (this.Verify(stmt, funcId)) rest

                | [] -> 
                    previousReturns
        verify false stmts

    /// All errors found during verification
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
                        errors.Add(Errors.VariableNotAnArray varId.Token)
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
            errors.Add(Errors.MathWithNonIntegerOperands token)
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
                    errors.Add(Errors.MathWithNonIntegerOperands token)
                this.Verify(termP)
            | None -> 
                primType, token

    member this.Verify (ExpressionPrime(op, term, rest)) =
        let typeLeft, leftToken = this.Verify term
        if typeLeft <> Primitive(IntType) then
            errors.Add(Errors.MathWithNonIntegerOperands leftToken)
        match rest with
            | Some(rest) -> 
                this.Verify rest |> ignore
            | _ -> ()
        typeLeft, leftToken

    member this.Verify(Expression(isNegated, term, rest)) =
        let termType, token = this.Verify term
        if isNegated && termType <> Primitive(IntType) then
            errors.Add(Errors.MathWithNonIntegerOperands token)
        else if rest.IsSome then
            if termType <> Primitive(IntType) then
                 errors.Add(Errors.MathWithNonIntegerOperands token)
            else          
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
                    errors.Add(Errors.UnexpectedType assignedType expected token.StartLocation)

    /// Verifies the semantics of a function node.
    /// Returns true is all paths in the function return.
    /// funcId is None if the function being parsed is the main function.
    /// Never returns true if funcId is None.
    member this.Verify(ast : Statement, funcId : FunctionIdentifier option) =
        match ast with
            | Block(statements) ->
                this.VerifyStatementBlock(statements, funcId)
            
            | AssignmentStatement(assignment) ->
                this.Verify assignment
                // Assignments never return
                false

            | IfElse(lExp, ifStatement, elseStatement) ->
                // For an if/else statement to return, both if AND else must have a return statement.
                this.Verify lExp
                this.Verify(ifStatement, funcId) && this.Verify(elseStatement, funcId)

            | WhileStatement(logicExp, body) ->
                this.Verify logicExp
                this.Verify(body, funcId) |> ignore
                // While statements are equivalent to an if with an else
                // The while loop could therefore never be entered.
                false

            | ReturnStatement(exp) ->
                let expType, expToken = this.Verify exp
                match funcId with
                    | Some(funcId) ->
                        let funcType = funcId.Attributes.Value.ReturnType, funcId.Token :> Token
                        checkTypes (expType, expToken) funcType
                        true
                    | None ->
                        errors.Add(Errors.MainCannotHaveReturn expToken.StartLocation)
                        // Return false since other parts of this function except
                        // that Main never returns true.
                        false

            | MethodInvocationStatement(identifier, arguments) ->
                let argTypes = List.map (fun (a : Element) -> this.Verify a) arguments
                checkTypeLists argTypes identifier.Attributes.Value.ParameterTypes
                false
            
            | SystemOutInvocation(arguments) ->
                // Arrays cannot be printed
                for arg in arguments do
                    match this.Verify(arg) with
                        | (Primitive(_), _) -> ()
                        | (ArrayType(_), token) ->
                            errors.Add(Errors.ArrayTypeCannotBePrinted token.StartLocation)
                // Never returns
                false
            
            | EmptyStatement -> 
                // Never returns
                false
    

    member this.Verify(FunctionBody(decls, stmts), funcId) =
        this.VerifyStatementBlock(stmts, funcId)
    
    member this.Verify (MainFunction(body)) =
        this.Verify(body, None) |> ignore
    
    member this.Verify (FunctionDefinition(funcId, _, body)) =
        if not <| this.Verify(body, Some funcId) then
            errors.Add(Errors.NotAllPathsReturn funcId)
    
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
        raise <| CompilerException(List.ofSeq verifier.Errors)
    prg