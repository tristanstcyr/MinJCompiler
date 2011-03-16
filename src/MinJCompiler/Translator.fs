module MinJ.Translator
open MinJ.Ast

open System.IO

exception TypeCheckError of string

type Translator(output : TextWriter) =

    let checkTypes (typ1 : MinJType) (typ2 : MinJType) = 
        if typ1 <> typ2 then
            raise <| TypeCheckError "Incorrect type"

    let rec translateVariableReference varRef =
        match varRef with
            | SimpleReference(varId) -> 
                varId.Type.Value.Type
            | ArrayAccess(varId, exp) -> 
                match varId.Type.Value.Type with
                    | ArrayType(_) as arrayType ->
                        arrayType
                    | _ ->
                        raise <| TypeCheckError "expected array type"

    and translatePrimitive prim =
        match prim with
            | VariablePrimitive(varRef) ->
                translateVariableReference varRef
            
            | NumberPrimitive(n) -> 
                Primitive IntType
            
            | CharPrimitive(c) -> 
                Primitive CharType
            
            | ParenPrimitive(exp) ->
                translateExpression(exp)

            | MethodInvocationPrimitive(funcId, arguments) ->
                funcId.Type.Value.ReturnType
                // TODO: Type chek params

    and translateTerm term =
        match term with
            | Term(primitive, termP) ->
                translatePrimitive(primitive)
                // TODO: Type check termP

    and translateExpressionPrime expressionPrime =
        match expressionPrime with
            | AdditionExpP(term, rest) | SubstractionExpP(term, rest) ->
                let termType = translateTerm term
                if rest.IsSome then
                    let restType = translateExpressionPrime rest.Value
                    checkTypes termType restType
                termType

    and translateExpression (expression : Expression) =
        match expression with
            | Expression(term, rest) | Negation(term, rest) ->
                let termType = translateTerm(term)
                if rest.IsSome then
                    let restType = translateExpressionPrime rest.Value
                    checkTypes termType restType
                termType

    and translateAssignment assignment =
        match assignment with
            | ExpressionAssignment(varRef, expression) ->
                let varType = translateVariableReference varRef
                let expType = translateExpression expression
                checkTypes varType expType
                
            | SystemInAssignment(assigned, inType) ->
                let assignedType = translateVariableReference assigned
                checkTypes assignedType <| Primitive(inType)
                

    and translateStatement statement =
        match statement with
            | Block(statements) ->
                for statement in statements do 
                    translateStatement statement
            | AssignmentStatement(assignment) ->
                translateAssignment assignment
            | IfElse(lExp, ifStatement, elseStatement) -> ()
            | WhileStatement(logicExp, body) -> ()
            | ReturnStatement(exp) -> ()
            | MethodInvocationStatement(identifier, arguments) -> ()
            | SystemOutInvocation(arguments) -> ()
            | EmptyStatement -> ()

    and translateMainFunction (MainFunction(decls, stmts)) =
        for stmt in stmts do
            translateStatement stmt

    and translatePrg (Program(decls, mainF, funcDefs)) =
        translateMainFunction mainF
        for func in funcDefs do
            ()

    member this.Translate prg = translatePrg prg