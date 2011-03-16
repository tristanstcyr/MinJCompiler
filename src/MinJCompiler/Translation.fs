module MinJ.Translation
open MinJ.Ast

open System.IO
open System

exception TypeCheckError of string

let translate (output : TextWriter) prg =

    let checkArgumentTypes (types1 : MinJType list) (types2 : MinJType list) =
        if types1 <> types2 then
           raise <| TypeCheckError "Incorrect types for method invocation" 

    let checkTypes (typ1 : MinJType) (typ2 : MinJType) = 
        if typ1 <> typ2 then
            raise <| TypeCheckError "Incorrect type"

    let rec translateVariableReference varRef =
        match varRef with
            | VariableReference(varId, None) -> 
                varId.Attributes.Type
            | VariableReference(varId, Some exp) -> 
                match varId.Attributes.Type with
                    | ArrayType(_) as arrayType ->
                        arrayType
                    | _ ->
                        raise <| TypeCheckError "Expected array type"

    and translateElement (element : Element) =
        match element with
            | VariableElement(varRef) ->
                translateVariableReference(varRef)
            | NumberElement(n) ->
                Primitive(IntType)
            | CharConstElement(c) ->
                Primitive(CharType)

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
                let argTypes = List.map translateElement arguments
                checkArgumentTypes funcId.Attributes.ParameterTypes argTypes  
                funcId.Attributes.ReturnType

    and translateTermP termP =
        match termP with
            | TermP(operator, primitive, rest) ->
                let prim = translatePrimitive(primitive)
                match rest with
                    | Some(termP) ->
                        let rest = translateTermP termP
                        checkTypes rest prim
                        prim
                    | None ->
                        prim

    and translateTerm term =
        match term with
            | Term(primitive, termP) ->
                let prim = translatePrimitive(primitive)
                match termP with
                    | Some(e) ->
                        prim
                    | None -> 
                        prim

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
            | IfElse(lExp, ifStatement, elseStatement) ->
                raise <| NotImplementedException()
            | WhileStatement(logicExp, body) ->
                raise <| NotImplementedException()
            | ReturnStatement(exp) ->
                raise <| NotImplementedException()
            | MethodInvocationStatement(identifier, arguments) -> 
                raise <| NotImplementedException()
            | SystemOutInvocation(arguments) ->
                raise <| NotImplementedException()
            | EmptyStatement ->
                raise <| NotImplementedException()

    and translateMainFunction (MainFunction(decls, stmts)) =
        for stmt in stmts do
            translateStatement stmt

    and translatePrg (Program(decls, mainF, funcDefs)) =
        translateMainFunction mainF
        for func in funcDefs do
            raise <| NotImplementedException()

    translatePrg prg