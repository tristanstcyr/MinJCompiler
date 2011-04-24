/// For translating a MinJ AST to three address code ASTs.
module MinJ.Ast.ToTac

open MinJ
open Tac
open System
open System.Collections.Generic

let private baseFunctionSize = 12u
let private tempSize = 4u

exception TemporaryVariableReleaseException of string * Ptr

type TemporaryVariablePool() =
    let mutable startAddress = 0u
    let mutable count = 0u
    let mutable max = 0u

    member this.StartAddress 
        with get() = startAddress
        and set(value) = startAddress <- value

    member this.RequiredStackSpace with get() = max * 4u

    member this.Acquire(func : Ptr -> 'a) =
        count <- count + 1u
        let result = func (Local(startAddress + (count - 1u) * tempSize))
        count <- count - 1u
        max <- Math.Max(max, count)
        result
            
/// Context of a program being translated to three address code.
/// It is passed directly to "ToTac" functions that translate AST
/// nodes found directly inside the global scope such as functions and fields.
type ProgramContext(instructions, functionCount) =
    /// The literals encountered in the program.
    let mutable literals : Literal list = [];
    /// A count of the labels generated. This is required
    /// to generate a unique label everytime.
    let mutable labelCount = functionCount - 1;
     
    new(functionCount) = ProgramContext(new List<Instruction>(), functionCount)

    /// Instructions generated during translation.
    member this.Instructions with get() = instructions

    /// Adds a constant to this context.
    member this.CreateConstant(literal) =
        literals <- literal :: literals
        Tac.Constant(((uint32)literals.Length - 1u) * 4u)
    
    /// Creates a label with a unique number.
    member this.CreateLabel() =
        labelCount <- labelCount + 1
        Label labelCount

    /// The literals encountered while converting
    /// a program to three address code.
    member this.Literals with get() = literals

    /// Convenient operator for adding a three address code
    /// instruction to this context.
    static member (<--) (context : ProgramContext, instruction) =
        context.Instructions.Add(instruction)
    /// Convenient operator for adding several three address code
    /// instruction to the prorgam context.
    static member (<--) (context : ProgramContext, instructions) =
        context.Instructions.AddRange(instructions)

/// Context for a function that is being translated to three address code.
/// An instant of this class is used for keeping track of instrucitons, labels, literals etc.
/// It is passed to "ToTac" functions that translate AST node found inside functions.
type FunctionContext(program : ProgramContext, label) =

    let mutable localDeclarationsSize = 12u;

    let tempVariablePool = TemporaryVariablePool()

    let mutable endLabel = program.CreateLabel()

    member this.TemporaryVariables with get() = tempVariablePool

    /// The label for this function
    member this.Label with get() = label

    member this.EndLabel 
        with get() = endLabel 

    /// The index of the stack size of this function
    member this.Index 
        with get() =
            match label with
                | Label(index) -> index;
    
    /// The stack size of this function. It is increased when
    /// local variables are encountered.
    member this.LocalDeclarationsSize 
        with get() = localDeclarationsSize 
        and set v = localDeclarationsSize <- v

    member this.TotalStackSize 
        with get() = localDeclarationsSize + tempVariablePool.RequiredStackSpace

    /// Creates a label with a unique number.
    member this.CreateLabel() = program.CreateLabel()
    
    /// Context for the program being translated.
    member this.Program with get() = program

    /// Convenient operator for adding a three address code
    /// instruction to this context.
    static member (<--) (context : FunctionContext, instruction) =
        context.Program.Instructions.Add(instruction)
    /// Convenient operator for adding several three address code
    /// instruction to this context.
    static member (<--) (context : FunctionContext, instructions) =
        context.Program.Instructions.AddRange(instructions)

/// Generates the instructions that are at the beginning of
/// every function.
let private functionPrologue (context : FunctionContext)  index =
    context <--
        [
        Labeled(context.Label)
        // The previous frame size to the stack pointer
        Inst3(TopSt, Add, TopSt, FrSz)
        // Store the return address
        Assign(Local(0u), RetAdd)
        // Store the previous stack position
        Inst3(Local(4u), Sub, TopSt, FrSz)
        // Store the previous frame size
        Assign(Local(8u), FrSz)
        // Set the frame size of the current frame
        Assign(FrSz, Frame((uint32)(context.Index * 4)))
        ]

/// Generates the instructions that are at the end of every function.
let private functionEpilogue (context : FunctionContext) =
    context <-- 
        [
        Labeled(context.EndLabel)
        Assign(FrSz, Local(8u))
        Assign(RetAdd, Local(0u))
        Assign(TopSt, Local(4u))
        Return
        ]

(*
    The following code adds static functions for MinJ AST nodes for translating them
    to a three address code program. Each function takes a "context". Contexts are
    essentially the state of the translation.
*)

type Ast.Program with
    member this.ToTac() =
        match this with
            | Ast.Program(varDecls, main, funcDecls) ->
                let context = ProgramContext(funcDecls.Length + 1)
                let globalSpace = List.fold VariableDeclaration.ToTac 0u varDecls
                let mainSize = MainFunction.ToTac context main
                let frameSizes = mainSize :: List.map (FunctionDefinition.ToTac context) funcDecls
                Tac.Program(context.Instructions, frameSizes, List.rev context.Literals, globalSpace)


and Ast.Assignment with
    static member ToTac (context : FunctionContext) this =
        // TODO: We could potentially be using more temp variables than needed
        context.TemporaryVariables.Acquire(fun tempPtr ->
            match this with
                | ExpressionAssignment(varRef, exp) ->
                    match varRef with
                        | VariableReference(varId, Some(indexExpression)) ->
                            context.TemporaryVariables.Acquire(fun indexTempPtr ->
                                let expressionResultPtr = Expression.ToTac context tempPtr exp
                                let indexTempPtr = Expression.ToTac context indexTempPtr indexExpression 
                                context <-- Tac.ArrayAssign(
                                    Local(varId.Attributes.Value.MemoryAddress),
                                    indexTempPtr,
                                    expressionResultPtr)
                            )
                        | _ ->   
                            let destPtr = VariableReference.ToTac context tempPtr varRef
                            let expressionResultPtr = Expression.ToTac context destPtr exp
                            if (expressionResultPtr <> destPtr) then
                                context <-- Assign(destPtr, expressionResultPtr)
            
                | SystemInAssignment(varRef, _) ->
                    let varPtr = VariableReference.ToTac context tempPtr varRef
                    context <-- Read(varPtr)
        )

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
    static member ToTac (context : FunctionContext) destPtr this =
        match this with
            | RelativeExpression(expLeft, relOp, expRight) ->
                context.TemporaryVariables.Acquire(fun tempLeft ->
                    context.TemporaryVariables.Acquire(fun tempRight ->
                        let resultLeft = Expression.ToTac context tempLeft expLeft
                        let resultRight = Expression.ToTac context tempRight expRight
                        context <-- Inst3(destPtr, relOp.ToTac(), resultLeft, resultRight)
                    )
               )

and Ast.LogicalExpression with
    static member ToTac (context : FunctionContext) destPtr this =
        match this with
            | SingletonLogicalExpression(relExp) ->
                RelativeExpression.ToTac context destPtr relExp
            
            | LogicalExpression(logLeft, Ast.AndOp, logRight) ->

                LogicalExpression.ToTac context destPtr logLeft
                       
                let skipLabel = context.CreateLabel()         
                context <-- IfFalse(destPtr, skipLabel)

                LogicalExpression.ToTac context destPtr logRight

                context <-- Labeled(skipLabel)
            
            | LogicalExpression(logLeft, Ast.OrOp, logRight) ->
                
                LogicalExpression.ToTac context destPtr logLeft

                let skipLabel1 = context.CreateLabel()
                let skipLabel2 = context.CreateLabel()
                context <-- IfFalse(destPtr, skipLabel1)
                context <-- Goto(skipLabel2)
                context <-- Labeled(skipLabel1)
                LogicalExpression.ToTac context destPtr logRight
                context <-- Labeled(skipLabel2)

and Ast.Statement with
    static member ToTac (context : FunctionContext) this =
        match this with
            | Block(statements) ->
                for statement in statements do 
                    Statement.ToTac context statement
            
            | AssignmentStatement(assignment) ->
                Assignment.ToTac context assignment

            | IfElse(lExp, ifStatement, elseStatement) ->
                
                let elseLabel = context.CreateLabel()
                let elseEndLabel = context.CreateLabel()
                
                // if (logicalExpression)
                context.TemporaryVariables.Acquire(fun logicalExpressionResult ->
                    LogicalExpression.ToTac context logicalExpressionResult lExp
                    context <-- IfFalse(logicalExpressionResult, elseLabel)
                )
                
                // {
                Statement.ToTac context ifStatement
                context <-- Goto(elseEndLabel)
                // }
                
                // else {
                context <-- Labeled(elseLabel)
                Statement.ToTac context elseStatement
                // }

                context <-- Labeled(elseEndLabel)

            | WhileStatement(logicExp, body) ->
                let startLabel = context.CreateLabel()
                let endLabel = context.CreateLabel()

                // while(logicExp)
                context <-- Labeled(startLabel)
                context.TemporaryVariables.Acquire(fun tmpPtr1 ->
                    LogicalExpression.ToTac context tmpPtr1 logicExp
                    context <-- IfFalse(tmpPtr1, endLabel)
                )

                // {
                Statement.ToTac context body
                context <-- Goto(startLabel)
                // }

                context <-- Labeled(endLabel)

            | ReturnStatement(exp) ->
                let expressionResultPtr = Expression.ToTac context Result exp
                if (expressionResultPtr <> Result) then
                    context <-- Assign(Result, expressionResultPtr)
                context <-- Goto(context.EndLabel)
                
            | MethodInvocationStatement(identifier, arguments) ->
                context.TemporaryVariables.Acquire(fun tempPtr ->
                    for argument in arguments do
                        let elemPtr = Element.ToTac context tempPtr argument
                        context <-- Push elemPtr
                )
                context <-- Call(Label(identifier.Attributes.Value.Index), arguments.Length)
            
            | SystemOutInvocation(arguments) -> 
                context.TemporaryVariables.Acquire(fun tempPtr ->
                    for arg in arguments do
                        context <-- Write(Element.ToTac context tempPtr arg)
                )
            
            | EmptyStatement -> ()

and Element with
    static member ToTac context (destPtr : Ptr) this =
        match this with
            | VariableElement(v) ->
                VariableReference.ToTac context destPtr v
            | NumberElement(n) ->
                context.Program.CreateConstant(NumberLiteral(n.Value))
            | CharConstElement(c) ->
                context.Program.CreateConstant(CharLiteral(c.Value))

and Primitive with
    static member ToTac (context : FunctionContext) (tmpPtr : Ptr) this =
        match this with

            | VariablePrimitive(varRef) ->
                VariableReference.ToTac context tmpPtr varRef

            | NumberPrimitive(n) -> 
                context.Program.CreateConstant(NumberLiteral(n.Value))

            | CharPrimitive(c) -> 
                context.Program.CreateConstant(CharLiteral(c.Value))

            | ParenPrimitive(exp) ->
                Expression.ToTac context tmpPtr exp

            | MethodInvocationPrimitive(funcId, arguments) ->
                // Add the push instructions for each argument in the invocation
                for ptr in (Seq.map (Element.ToTac context tmpPtr) arguments) do
                    context <-- Push ptr
                // Call the function
                context <-- Call(Label(funcId.Attributes.Value.Index), arguments.Length)
                // Result is in the special result address
                context <-- Assign(tmpPtr, Result)
                tmpPtr

and TermOp with
    static member ToTac this =
        match this with
            | MulOp -> Tac.Mul
            | DivOp -> Tac.Div
            | ModOp -> Tac.Mod

and TermP with
    static member ToTac (context : FunctionContext) termPtr destPtr this =
        match this with
            | TermP(op, prim, termPOption) ->
                context.TemporaryVariables.Acquire(fun tempPtr ->
                    let primPtr = Primitive.ToTac context tempPtr prim
                    match termPOption with
                        | Some(termP) ->
                            context <-- Inst3(tempPtr, TermOp.ToTac op, termPtr, primPtr)
                            TermP.ToTac context tempPtr destPtr termP
                        | None ->
                            context <-- Inst3(destPtr, TermOp.ToTac op, termPtr, primPtr)
                )

and Term with
    static member ToTac (context : FunctionContext) dest this =
        match this with
            | Term(prim, termP) ->
                
                match termP with
                    | Some(termP) ->
                        context.TemporaryVariables.Acquire(fun tempPtr ->
                            let primPtr = Primitive.ToTac context tempPtr prim
                            TermP.ToTac context primPtr dest termP
                            dest
                        )
                    | None ->
                        Primitive.ToTac context dest prim

and ExpressionOp with
    static member ToTac this =
        match this with
            | AddOp -> Tac.Add
            | SubOp -> Tac.Sub

and ExpressionPrime with
    static member ToTac context term1Ptr destPtr this : unit =
        match this with
            | ExpressionPrime(op, term, expPOp) ->
                let term2Ptr = Term.ToTac context destPtr term 
                let tacOp = ExpressionOp.ToTac op
                context <-- Inst3(destPtr, tacOp, term1Ptr, term2Ptr)
                match expPOp with
                    | Some(expP) ->
                        ExpressionPrime.ToTac context destPtr destPtr expP
                    | None -> ()
and Expression with
    static member ToTac context (destPtr : Ptr) this : Ptr =
        match this with
            | Expression(isNegated, term, rest) ->
                // TODO: Negation
                match rest with
                    | Some(expP) ->
                        context.TemporaryVariables.Acquire(fun termResultPtr ->
                            let termPtr = Term.ToTac context termResultPtr term
                            ExpressionPrime.ToTac context termPtr destPtr expP
                            destPtr
                        )

                    | None ->
                        Term.ToTac context destPtr term

and VariableReference with
    static member ToTac context (tmpPtr : Ptr) this =
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
                        let expressionPtr = Expression.ToTac context tmpPtr exp
                        context <-- ArrayDeref(tmpPtr, varPtr, expressionPtr)
                        tmpPtr
                    | None ->
                        varPtr
                
and VariableDeclaration with
    static member ToTac spaceSize this =
        match this with
            | NonArrayVariableDeclaration(id) ->
                id.Attributes.Value.MemoryAddress <- spaceSize
                spaceSize + 4u
            | ArrayVariableDeclaration(id, _, size) ->
                id.Attributes.Value.MemoryAddress <- spaceSize
                spaceSize + (uint32)size * 4u

and FunctionBody with
    static member ToTac (context : FunctionContext) this =
        match this with
            | FunctionBody(varDecls, stmts) ->
                for varDecl in varDecls do
                     let newSize = VariableDeclaration.ToTac context.LocalDeclarationsSize varDecl
                     context.LocalDeclarationsSize <- newSize
                context.TemporaryVariables.StartAddress <- context.LocalDeclarationsSize
                List.iter (Statement.ToTac context) stmts

and MainFunction with
    static member ToTac context this : FrameSize =
        match this with
            | MainFunction(body) ->
                let context = FunctionContext(context, Label(0))
                functionPrologue context 0
                FunctionBody.ToTac context body
                functionEpilogue context
                context.TotalStackSize

and FunctionDefinition with
    static member ToTac (context : ProgramContext) this : FrameSize =
        match this with
            | FunctionDefinition(id, parameters, body) ->
                let context = FunctionContext(context, Label(id.Attributes.Value.Index))
                functionPrologue context (id.Attributes.Value.Index)
                // Set memory address of parameters
                for param in parameters do
                    param.Attributes.MemoryAddress <- context.LocalDeclarationsSize
                    context.LocalDeclarationsSize <- context.LocalDeclarationsSize + 4u
                FunctionBody.ToTac context body
                functionEpilogue context
                context.TotalStackSize