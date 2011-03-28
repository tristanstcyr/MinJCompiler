/// For translating a MinJ AST to three address code ASTs.
module MinJ.Ast.ToTac

open MinJ
open Tac
open System.Collections.Generic

/// Temporary storage address 1.
/// This is also assumed to be used for storing the return
/// value of functions.
let TmpPtr1 = Local(12)
/// Temporary storage address 2.
let TmpPtr2 = Local(16)

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
        Tac.Constant(literals.Length - 1)
    
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
    let mutable stackSize = 20;

    /// The label for this function
    member this.Label with get() = label
    /// The stack size of this function. It is increased when
    /// local variables are encountered.
    member this.StackSize with get() = stackSize and set v = stackSize <- v
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
        Assign(Local(4), TopSt)
        Inst3(TopSt, Add, TopSt, FrSz)
        Assign(Local(8), FrSz)
        Assign(FrSz, Frame(index))
        ]

/// Generates the instructions that are at the end of every function.
let private functionEpilogue (context : FunctionContext) =
    context <-- 
        [
        Assign(FrSz, Frame(8))
        Assign(TopSt, Local(4))
        Assign(RetAdd, Local(0))
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
                let globalSpace = List.fold VariableDeclaration.ToTac 0 varDecls
                let mainSize = MainFunction.ToTac context main
                let frameSizes = mainSize :: List.map (FunctionDefinition.ToTac context) funcDecls
                Tac.Program(context.Instructions, frameSizes, context.Literals, globalSpace)


and Ast.Assignment with
    static member ToTac (context : FunctionContext) this =
        match this with
            | ExpressionAssignment(varRef, exp) ->
                let varPtr = VariableReference.ToTac context TmpPtr1 varRef
                let ptr = Expression.ToTac context TmpPtr1 exp
                
                context <-- Assign(varPtr, ptr)
            
            | SystemInAssignment(varRef, _) ->
                let varPtr = VariableReference.ToTac context TmpPtr1 varRef
                
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
    static member ToTac (context : FunctionContext) ptr this =
        match this with
            | RelativeExpression(expLeft, relOp, expRight) ->
                let leftPtr = Expression.ToTac context TmpPtr2 expLeft
                let rightPtr = Expression.ToTac context TmpPtr1 expRight
                context <-- Inst3(ptr, relOp.ToTac(), leftPtr, rightPtr)

and Ast.LogicalExpression with
    static member ToTac (context : FunctionContext) ptr this =
        match this with
            | SingletonLogicalExpression(relExp) ->
                RelativeExpression.ToTac context ptr relExp
            
            | LogicalExpression(logLeft, Ast.AndOp, logRight) ->

                LogicalExpression.ToTac context ptr logLeft
                       
                let skipLabel = context.CreateLabel()         
                context <-- IfFalse(ptr, skipLabel)

                LogicalExpression.ToTac context ptr logRight

                context <-- Labeled(skipLabel)
            
            | LogicalExpression(logLeft, Ast.OrOp, logRight) ->
                
                LogicalExpression.ToTac context ptr logLeft

                let skipLabel1 = context.CreateLabel()
                let skipLabel2 = context.CreateLabel()
                context <-- IfFalse(ptr, skipLabel1)
                context <-- Goto(skipLabel2)
                context <-- Labeled(skipLabel1)
                LogicalExpression.ToTac context ptr logRight
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
                let resultPtr = TmpPtr1
                let elseLabel = context.CreateLabel()
                let elseEndLabel = context.CreateLabel()

                LogicalExpression.ToTac context resultPtr lExp
                
                context <-- IfFalse(resultPtr, elseLabel)
                
                Statement.ToTac context ifStatement
                
                context <-- Goto(elseEndLabel)
                context <-- Labeled(elseLabel)
                
                Statement.ToTac context elseStatement
                
                context <-- Labeled(elseEndLabel)

            | WhileStatement(logicExp, body) ->
                let resultPtr = TmpPtr1
                let startLabel = context.CreateLabel()
                let endLabel = context.CreateLabel()

                context <-- Labeled(startLabel)
                LogicalExpression.ToTac context resultPtr logicExp
                context <-- IfFalse(resultPtr, endLabel)
                Statement.ToTac context body
                context <-- Goto(startLabel)
                context <-- Labeled(endLabel)

            | ReturnStatement(exp) ->
                Expression.ToTac context TmpPtr1 exp |> ignore
                context <-- Return

            | MethodInvocationStatement(identifier, arguments) ->
                let ptrs = List.map (Element.ToTac context TmpPtr1) arguments
                for ptr in ptrs do context <-- Push ptr
                context <-- Call(Label(identifier.Attributes.Value.Index), ptrs.Length)
            
            | SystemOutInvocation(arguments) -> 
                for arg in arguments do
                    context <-- Write(Element.ToTac context TmpPtr1 arg)
            
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
                List.iter (fun ptr -> context <-- Push ptr) (List.map (Element.ToTac context tmpPtr) arguments)
                context <-- Call(Label(funcId.Attributes.Value.Index), arguments.Length)
                if tmpPtr <> TmpPtr1 then
                    context <-- Tac.Assign(tmpPtr, TmpPtr1)
                tmpPtr

and TermOp with
    static member ToTac this =
        match this with
            | MulOp -> Tac.Mul
            | DivOp -> Tac.Div
            | ModOp -> Tac.Mod

and TermP with
    static member ToTac context termPtr dest this =
        match this with
            | TermP(op, prim, termPOp) ->
                let primPtr = Primitive.ToTac context dest prim
                context <-- Inst3(dest, TermOp.ToTac op, termPtr, primPtr)
                match termPOp with
                    | Some(termP) ->
                        TermP.ToTac context dest dest termP
                    | None ->
                        dest

and Term with
    static member ToTac context dest this =
        match this with
            | Term(prim, termPOp) ->
                
                match termPOp with
                    | Some(termP) ->
                        let primPtr = Primitive.ToTac context TmpPtr2 prim
                        TermP.ToTac context primPtr dest termP
                    | None ->
                        Primitive.ToTac context dest prim

and ExpressionOp with
    static member ToTac this =
        match this with
            | AddOp -> Tac.Add
            | SubOp -> Tac.Sub

and ExpressionPrime with
    static member ToTac context term1Ptr destPtr this =
        match this with
            | ExpressionPrime(op, term, expPOp) ->
                let term2Ptr = Term.ToTac context destPtr term 
                let tacOp = ExpressionOp.ToTac op
                context <-- Inst3(destPtr, tacOp, term1Ptr, term2Ptr)
                match expPOp with
                    | Some(expP) ->
                        ExpressionPrime.ToTac context destPtr destPtr expP
                    | None ->
                        destPtr
and Expression with
    static member ToTac context (destPtr : Ptr) this =
        match this with
            | Expression(isNegated, term, rest) ->
                // TODO: Negation
                match rest with
                    | Some(expP) ->
                        let termPtr = Term.ToTac context TmpPtr2 term
                        ExpressionPrime.ToTac context termPtr destPtr expP
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
                        let expPtr = Expression.ToTac context tmpPtr exp
                        context <-- ArrayDeref(tmpPtr, varPtr, expPtr)
                        tmpPtr
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
                functionPrologue context 0
                FunctionBody.ToTac context body
                functionEpilogue context
                context.StackSize

and FunctionDefinition with
    static member ToTac (context : ProgramContext) this : FrameSize =
        match this with
            | FunctionDefinition(id, parameters, body) ->
                let context = FunctionContext(context, Label(id.Attributes.Value.Index))
                functionPrologue context id.Attributes.Value.Index
                FunctionBody.ToTac context body
                functionEpilogue context
                context.StackSize