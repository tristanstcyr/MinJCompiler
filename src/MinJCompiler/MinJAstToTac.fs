module MinJ.Ast.ToTac

open MinJ
open Tac
open System.Collections.Generic

let private tmp = Local(12)

type Constant = 
    | CharConstant of char
    | NumberConstant of int64

type ProgramContext(instructions, functionCount) =
    let mutable literals : Literal list = [];
    let mutable labelCount = functionCount - 1;
     
    new(functionCount) = ProgramContext(new List<Instruction>(), functionCount)

    member this.Instructions with get() = instructions

    member this.CreateConstant(literal) =
        literals <- literal :: literals
        Tac.Constant(literals.Length - 1)
    
    member this.CreateLabel() =
        labelCount <- labelCount + 1
        Label labelCount

    member this.Literals with get() = literals

    static member (<--) (context : ProgramContext, instruction) =
        context.Instructions.Add(instruction)
    static member (<--) (context : ProgramContext, instructions) =
        context.Instructions.AddRange(instructions)

type FunctionContext(program : ProgramContext, label) =
    let mutable stackSize = 16;

    member this.Label with get() = label
    member this.StackSize with get() = stackSize and set v = stackSize <- v
    member this.CreateLabel() = program.CreateLabel()
    member this.Program with get() = program

    static member (<--) (context : FunctionContext, instruction) =
        context.Program.Instructions.Add(instruction)
    static member (<--) (context : FunctionContext, instructions) =
        context.Program.Instructions.AddRange(instructions)

let private functionEpilogue (context : FunctionContext)  index =
    context <--
        [
        Labeled(context.Label)
        Assign(Local(4), TopSt)
        Inst3(TopSt, Add, TopSt, FrSz)
        Assign(Local(8), FrSz)
        Assign(FrSz, Frame(index))
        ]

let private functionProlog (context : FunctionContext) =
    context <-- 
        [
        Assign(FrSz, Frame(8))
        Assign(TopSt, Local(4))
        Assign(RetAdd, Local(0))
        ]

type Ast.Program with
    member this.ToTac() =
        match this with
            | Ast.Program(varDecls, main, funcDecls) ->
                let context = ProgramContext(funcDecls.Length + 1)
                let globalSpace = List.fold VariableDeclaration.ToTac 0 varDecls
                let mainSize = MainFunction.ToTac context main
                let frameSizes = mainSize :: List.map (FunctionDefinition.ToTac context) funcDecls
                Tac.Program(context.Instructions, frameSizes, context.Literals, globalSpace)


and Ast.Assignment with
    static member ToTac (context : FunctionContext) this =
        match this with
            | ExpressionAssignment(varRef, exp) ->
                let varPtr = VariableReference.ToTac context varRef
                let ptr = Expression.ToTac context exp
                
                context <-- Assign(varPtr, ptr)
            
            | SystemInAssignment(varRef, _) ->
                let varPtr = VariableReference.ToTac context varRef
                
                context <-- Read(varPtr)

and Ast.RelOperator with
    member this.ToTac() =
        match this with
            | Ast.Lt -> Tac.Lt
            | Ast.Gt -> Tac.Gt
            | Ast.Eq -> Tac.Eq
            | Ast.LtEq -> Tac.LtEq 
            | Ast.GtEq -> Tac.GtEq
            | Ast.Not  -> Tac.Not
            | Ast.NotEq -> Tac.NotEq

and Ast.RelativeExpression with
    static member ToTac (context : FunctionContext) this =
        match this with
            | RelativeExpression(expLeft, relOp, expRight) ->
                let leftPtr = Expression.ToTac context expLeft
                let rightPtr = Expression.ToTac context expRight
                context <-- Inst3(tmp, relOp.ToTac(), leftPtr, rightPtr)
                tmp

and Ast.LogicalExpression with
    static member ToTac (context : FunctionContext) this =
        match this with
            | LogicalRelativeExpression(relExp) ->
                RelativeExpression.ToTac context relExp
            | LogicalExpression(relExp, logOp, logExp) ->
                let relExpPtr = RelativeExpression.ToTac context relExp
                let logExpPtr = LogicalExpression.ToTac context logExp
                let tacOp =
                    match logOp with
                        | OrOp -> Tac.And
                        | AndOp -> Tac.Or
                context <-- Inst3(tmp, tacOp, relExpPtr, logExpPtr)
                tmp

and Ast.Statement with
    static member ToTac (context : FunctionContext) this =
        match this with
            | Block(statements) ->
                for statement in statements do 
                    Statement.ToTac context statement
            
            | AssignmentStatement(assignment) ->
                Assignment.ToTac context assignment

            | IfElse(lExp, ifStatement, elseStatement) ->
                let expPtr = LogicalExpression.ToTac context lExp
                let elseLabel = context.CreateLabel()
                let elseEndLabel = context.CreateLabel()

                context <-- IfFalse(expPtr, elseLabel)
                
                Statement.ToTac context ifStatement
                
                context <-- Goto(elseEndLabel)
                context <-- Labeled(elseLabel)
                
                Statement.ToTac context elseStatement
                
                context <-- Labeled(elseEndLabel)

            | WhileStatement(logicExp, body) ->
                let startLabel = context.CreateLabel()
                let endLabel = context.CreateLabel()

                context <-- Labeled(startLabel)
                let ptrLogExp = LogicalExpression.ToTac context logicExp
                context <-- IfFalse(ptrLogExp, endLabel)
                Statement.ToTac context body
                context <-- Goto(startLabel)
                context <-- Labeled(endLabel)

            | ReturnStatement(exp) ->
                let expPtr = Expression.ToTac context exp
                context <-- Assign(tmp, expPtr)
                context <-- Return

            | MethodInvocationStatement(identifier, arguments) ->
                let ptrs = List.map (Element.ToTac context) arguments
                for ptr in ptrs do context <-- Push ptr
                context <-- Call(Label(identifier.Attributes.Value.Index), ptrs.Length)
            
            | SystemOutInvocation(arguments) -> 
                for arg in arguments do
                    context <-- Write(Element.ToTac context arg)
            
            | EmptyStatement -> ()

and Element with
    static member ToTac context this =
        match this with
            | VariableElement(v) ->
                VariableReference.ToTac context v
            | NumberElement(n) ->
                context.Program.CreateConstant(NumberLiteral(n.Value))
            | CharConstElement(c) ->
                context.Program.CreateConstant(CharLiteral(c.Value))

and Primitive with
    static member ToTac (context : FunctionContext) this =
        match this with
            | VariablePrimitive(varRef) ->
                VariableReference.ToTac context varRef
            | NumberPrimitive(n) -> 
                context.Program.CreateConstant(NumberLiteral(n.Value))
            | CharPrimitive(c) -> 
                context.Program.CreateConstant(CharLiteral(c.Value))
            | ParenPrimitive(exp) ->
                Expression.ToTac context exp
            | MethodInvocationPrimitive(funcId, arguments) ->
                let ptrs = List.map (Element.ToTac context) arguments
                List.iter (fun ptr -> context <-- Push ptr) ptrs
                context <-- Call(Label(funcId.Attributes.Value.Index), arguments.Length)
                tmp

and TermOp with
    static member ToTac this =
        match this with
            | MulOp -> Tac.Mul
            | DivOp -> Tac.Div
            | ModOp -> Tac.Mod

and TermP with
    static member ToTac context ptr this =
        match this with
            | TermP(op, prim, termPOp) ->
                let primPtr = Primitive.ToTac context prim
                context <-- Inst3(tmp, TermOp.ToTac op, ptr, primPtr)
                match termPOp with
                    | Some(termP) ->
                        TermP.ToTac context tmp termP
                    | None ->
                        tmp

and Term with
    static member ToTac context this =
        match this with
            | Term(prim, termPOp) ->
                let primPtr = Primitive.ToTac context prim
                match termPOp with
                    | Some(termP) ->
                        TermP.ToTac context primPtr termP
                    | None ->
                        primPtr

and ExpressionOp with
    static member ToTac this =
        match this with
            | AddOp -> Tac.Add
            | SubOp -> Tac.Sub

and ExpressionPrime with
    static member ToTac context term1Ptr this =
        match this with
            | ExpressionPrime(op, term, expPOp) ->
                let term2Ptr = Term.ToTac context term
                let tacOp = ExpressionOp.ToTac op
                context <-- Inst3(tmp, tacOp, term1Ptr, term2Ptr)
                match expPOp with
                    | Some(expP) ->
                        ExpressionPrime.ToTac context tmp expP
                    | None ->
                        tmp
and Expression with
    static member ToTac context this =
        match this with
            | Expression(isNegated, term, rest) ->
                // TODO: Negation
                let expPtr = Term.ToTac context term
                match rest with
                    | Some(expP) ->
                        ExpressionPrime.ToTac context expPtr expP
                    | None ->
                        expPtr

and VariableReference with
    static member ToTac context this =
        match this with
            | VariableReference(varId, exp) ->
                let address = varId.Attributes.Value.MemoryAddress
                let varPtr = 
                    match varId.Attributes.Value.Scope with
                        | GlobalVariable ->
                            Global(address)
                        | LocalVariable ->
                            Local(address)
                        | ParameterVariable ->
                            Param(address)
                match exp with
                    | Some(exp) ->
                        let expPtr = Expression.ToTac context exp
                        context <-- ArrayDeref(tmp, varPtr, expPtr)
                        tmp
                    | None ->
                        varPtr
                
and VariableDeclaration with
    static member ToTac spaceSize this =
        match this with
            | NonArrayVariableDeclaration(id) ->
                id.Attributes.Value.MemoryAddress <- spaceSize
                spaceSize + 4
            | ArrayVariableDeclaration(id, _, size) ->
                id.Attributes.Value.MemoryAddress <- spaceSize
                size

and FunctionBody with
    static member ToTac (context : FunctionContext) this =
        match this with
            | FunctionBody(varDecls, stmts) ->
                context.StackSize <- List.fold VariableDeclaration.ToTac context.StackSize varDecls
                List.iter (Statement.ToTac context) stmts

and MainFunction with
    static member ToTac context this : FrameSize =
        match this with
            | MainFunction(body) ->
                let context = FunctionContext(context, Label(0))
                functionEpilogue context 0
                FunctionBody.ToTac context body
                functionProlog context
                context.StackSize

and FunctionDefinition with
    static member ToTac (context : ProgramContext) this : FrameSize =
        match this with
            | FunctionDefinition(id, parameters, body) ->
                let context = FunctionContext(context, Label(id.Attributes.Value.Index))
                functionEpilogue context id.Attributes.Value.Index
                FunctionBody.ToTac context body
                functionProlog context
                context.StackSize