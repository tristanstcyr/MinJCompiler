module MinJ.Ast.TypeCheck

open Scanner.Tokens
open MinJ.Ast
open System.IO
open System
open System.Collections.Generic

exception TypeCheckError of string * MinJType * Token

let checkTypes (typ1 : MinJType, id1 : Token) (typ2 : MinJType, id2 : Token) = 
    if typ1 <> typ2 then
        raise <| TypeCheckError("Unexpected type", typ1, id1)

let checkTypeLists (types1 : (MinJType * Token) list) (types2 : (MinJType * Token) list) =
    for a, b in List.zip types1 types2 do
        checkTypes a b

type VariableReference with 
    member this.TypeCheck() =
        match this with
            | VariableReference(varId, None) -> 
                varId.Attributes.Value.Type, varId.Token :> Token
            | VariableReference(varId, Some exp) -> 
                match varId.Attributes.Value.Type with
                    | ArrayType(typ) as arrayType ->
                        Primitive(typ), varId.Token :> Token
                    | typ ->
                        raise <| TypeCheckError("Expected array type", typ, varId.Token)
        
and Element with
    member this.TypeCheck() =
        match this with
            | VariableElement(varRef) ->
                varRef.TypeCheck()
            | NumberElement(n) ->
                Primitive(IntType), n :> Token
            | CharConstElement(c) ->
                Primitive(CharType), c :> Token

and Primitive with
    member this.TypeCheck() =
        match this with
            | VariablePrimitive(varRef) ->
                varRef.TypeCheck()
            
            | NumberPrimitive(n) -> 
                Primitive IntType, n :> Token
            
            | CharPrimitive(c) -> 
                Primitive CharType, c :> Token
            
            | ParenPrimitive(exp) ->
                exp.TypeCheck()

            | MethodInvocationPrimitive(funcId, arguments) ->
                let argTypes = List.map (fun (a : Element) -> a.TypeCheck()) arguments
                checkTypeLists funcId.Attributes.Value.ParameterTypes argTypes
                funcId.Attributes.Value.ReturnType, funcId.Token :> Token

and TermP with
    member this.TypeCheck() =
        match this with
            | TermP(operator, primitive, rest) ->
                let prim = primitive.TypeCheck()
                match rest with
                    | Some(termP) ->
                        let rest = termP.TypeCheck()
                        checkTypes rest prim
                        prim
                    | None ->
                        prim

and Term with
    member this.TypeCheck() =
        match this with
            | Term(primitive, termP) ->
                let prim = primitive.TypeCheck()
                match termP with
                    | Some(e) ->
                        prim
                    | None -> 
                        prim

and RelativeExpression with
    member this.TypeCheck() =
        match this with
            | RelativeExpression(expLeft, relOp, expRight) ->
                let typeLeft = expLeft.TypeCheck()
                let typeRight = expRight.TypeCheck()
                checkTypes typeLeft typeRight

and LogicalExpression with
    member this.TypeCheck() =
        match this with
            | LogicalRelativeExpression(relExp) ->
                relExp.TypeCheck()
            | LogicalExpression(relExp, logOp, logExp) ->
                relExp.TypeCheck()
                logExp.TypeCheck()

and ExpressionPrime with
    member this.TypeCheck() =
        match this with
            | ExpressionPrime(op, term, rest) ->
                let termType = term.TypeCheck()
                match rest with
                    | Some(rest) -> checkTypes termType <| rest.TypeCheck()
                    | _ -> ()
                termType

and Expression with
    member this.TypeCheck() =
        match this with
            | Expression(bool, term, rest) ->
                let termType, token = term.TypeCheck()
                if bool && termType <> Primitive(IntType) then
                    raise <| TypeCheckError("Only expressions of type int can be negated", termType, token)
                if rest.IsSome then
                    let restType = rest.Value.TypeCheck() 
                    checkTypes (termType, token) restType
                termType, token

and Assignment with
    member this.TypeCheck() =
        match this with
            | ExpressionAssignment(varRef, expression) ->
                checkTypes <| varRef.TypeCheck() <| expression.TypeCheck()
            | SystemInAssignment(assigned, inType) ->
                let assignedType, token = assigned.TypeCheck()
                let expected = Primitive(inType)
                if assignedType <> expected then
                    raise <| TypeCheckError("Incompatible type used with System.in", expected, token)

and Statement with
    member this.TypeCheck(funcId : FunctionIdentifier option) =
        match this with
            | Block(statements) ->
                for statement in statements do 
                    statement.TypeCheck(funcId)
            
            | AssignmentStatement(assignment) ->
                assignment.TypeCheck()

            | IfElse(lExp, ifStatement, elseStatement) ->
                lExp.TypeCheck()
                ifStatement.TypeCheck(funcId)
                elseStatement.TypeCheck(funcId)

            | WhileStatement(logicExp, body) ->
                logicExp.TypeCheck()
                body.TypeCheck(funcId)

            | ReturnStatement(exp) ->
                let expType, expToken = exp.TypeCheck()
                match funcId with
                    | Some(funcId) ->
                        
                        let funcType = funcId.Attributes.Value.ReturnType, funcId.Token :> Token
                        checkTypes (expType, expToken) funcType
                    | _ ->
                        raise <| TypeCheckError("The main function cannot have a return statement.", 
                            expType, expToken)

            | MethodInvocationStatement(identifier, arguments) ->
                let argTypes = List.map (fun (a : Element) -> a.TypeCheck()) arguments
                checkTypeLists argTypes identifier.Attributes.Value.ParameterTypes
            
            | SystemOutInvocation(arguments) -> ()
            
            | EmptyStatement -> ()

and FunctionBody with
    member this.TypeCheck(returnType) =
        match this with
            | FunctionBody(decls, stmts) ->
                for stmt in stmts do stmt.TypeCheck(returnType)

and MainFunction with
    member this.TypeCheck() =
        match this with
            | MainFunction(body) -> body.TypeCheck(None)

and FunctionDefinition with
    member this.TypeCheck() =
        match this with
            | FunctionDefinition(funcId, _, body) ->
                body.TypeCheck(Some funcId)

and Program with
    member this.TypeCheck() =
        match this with
            | Program(decls, mainF, funcDefs) ->
                mainF.TypeCheck()
                for funcDef in funcDefs do funcDef.TypeCheck()