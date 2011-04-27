module Compiler.Tac.ToMoon

open Moon.Ast
open Compiler

open System

module Registers =
    let TopStack = 1
    let GlobalVarsAddress = 2
    let Acc1 = 3
    let Acc2 = 4
    let Acc3 = 5
    let FrameSize = 10
    let ParameterPassing = 11
    let Helper = 13
    let Result = 14
    let ReturnAddress = 15

module Labels =
    let FrameSizes = "fz"
    let Constants = "cs"
    let Globals = "gb"
    let Stack = "st"

module InternalErrors =
    let OperandOutOfBounds =
        "An operand was out of bounds while compiling to Moon."
    let AttemptedToSetConstant =
        "Attempted to set constant."

let StackSize = 10000u

type ProgramContext = {
    ConstantsOffset : uint32;
    FrameSizeTableOffset : uint32;
}

let defineFrameSize label size = 
    Line(label, Directive(Dw([DwKInt((int32)size)])), None)

let defineFrameSizes frameSizes = seq {
    yield defineFrameSize (Some Labels.FrameSizes) (List.head frameSizes)
    for size in List.tail frameSizes do
        yield defineFrameSize None size
}

let uint32ToInt16 number =
    if number > (uint32)Int16.MaxValue then
        raise <| CompilerInternalException(InternalErrors.OperandOutOfBounds)
    (int16)number

let defineConstant label literal =
    match literal with
        | CharLiteral(char) ->
            Line(label, Directive(Dw [DwKInt((int32)char)]), None)
        | NumberLiteral(i) ->
            Line(label, Directive(Dw [DwKInt((int32)i)]), None)

let defineConstants literals = seq {
    if (List.length literals > 0) then
        yield defineConstant (Some Labels.Constants) (List.head literals)
        for literal in List.tail literals do
            yield defineConstant None literal
}

let defineGlobalSection size = 
    Line(Some Labels.Globals, Directive(Res size), None)

let setTo register number = seq {
    yield Line(None, Instruction("addi", [Register(register); Register(0); Constant(Number(number), None)]), None)
}

let loadOffsetFromLabel register address label = seq {
    yield! setTo register address
    yield Line(None, Instruction("lw", [Register(register); Constant(Symbol(label), Some register)]), None)
}

let loadInRegister destRegister src = seq {
    match src with
        
        | Tac.Global(address) ->
            yield! loadOffsetFromLabel destRegister (uint32ToInt16 address) Labels.Globals
        
        | Tac.Constant(address) ->
            yield! loadOffsetFromLabel destRegister (uint32ToInt16 address) Labels.Constants
        
        | Tac.Local(address) | Tac.Param(address) ->
            
            // Add ParameterStart + address to the register
            yield Line(None, Instruction("addi", [Register(destRegister); Register(Registers.TopStack); Constant(Number(uint32ToInt16 address), None)]), None)
            // Load where at the address at the register
            yield Line(None, Instruction("lw", [Register(destRegister); Constant(Number(0s), Some(destRegister))]), None)
        
        | Tac.TopSt ->
            yield Line(None, Instruction("add", [Register(destRegister); Register(0); Register(Registers.TopStack)]), 
                Some(sprintf "Put the address of the top of the stack in r%i" destRegister))
        
        | Tac.Frame(address) ->
            yield! loadOffsetFromLabel destRegister (uint32ToInt16 address) Labels.FrameSizes
        
        | Tac.RetAdd ->
            yield Line(None, Instruction("add", [Register(destRegister); Register(0); Register(Registers.ReturnAddress)]), 
                Some(sprintf "Put the return address into r%i" destRegister))
        
        | Tac.Result ->
            yield Line(None, Instruction("add", [Register(destRegister);Register(0);Register(Registers.Result)]), 
                Some(sprintf "Put the returned value into r%i" destRegister))
        
        | Tac.FrSz ->
            yield Line(None, Instruction("add", [Register(destRegister);Register(0);Register(Registers.FrameSize)]), 
                Some(sprintf "Put the current frame's size value into r%i" destRegister))
        | Tac.Globals ->
            yield Line(None, Instruction("add", [Register(destRegister); Register(0); Register(Registers.GlobalVarsAddress)]), 
                None)
}

let storeRegister dest src = seq {
    match dest with
        | Tac.Global(address) ->
            yield! setTo Registers.Helper (uint32ToInt16 address)
            yield Line(None, Instruction("sw", [Constant(Symbol(Labels.Globals), Some(Registers.Helper)); Register(src)]), None)
        | Tac.Local(address) | Tac.Param(address) ->
            // Add ParameterStart + address to the register
            yield Line(None, Instruction("addi", [Register(Registers.Helper); Register(Registers.TopStack); Constant(Number(uint32ToInt16 address), None)]), None)
            yield Line(None, Instruction("sw", [Constant(Number(0s), Some(Registers.Helper));Register(src)]), None)
        | Tac.TopSt ->
            yield Line(None, Instruction("add", [Register(Registers.TopStack);Register(0);Register(src)]), None)
        | Tac.RetAdd ->
            yield Line(None, Instruction("add", [Register(Registers.ReturnAddress); Register(0); Register(src)]), None)
        | Tac.Result ->
            yield Line(None, Instruction("add", [Register(Registers.Result); Register(0); Register(src)]), None)
        | Tac.FrSz ->
            yield Line(None, Instruction("add", [Register(Registers.FrameSize);Register(0);Register(src)]), None)
        | Tac.Constant(_) | Tac.Frame(_) | Tac.Globals ->
            raise <| CompilerInternalException(InternalErrors.AttemptedToSetConstant)
}

let GetArrayEntryAddress arrayAddressPtr indexPtr destRegister helpRegister = seq {
    // address of array into helpRegister
    yield! loadInRegister helpRegister arrayAddressPtr
    // 
    yield! loadInRegister destRegister indexPtr
    yield Line(None, Instruction("sl", [Register(destRegister);Constant(Number(2s), None)]), 
        Some "Get the offset by multiplying by the word size")
    yield Line(None, Instruction("add", [Register(destRegister);Register(destRegister);Register(helpRegister);]), 
        Some "Add the address of the array to the offset")
}

type Tac.Operator with
    static member ToMoonOperation this =
        match this with
            | Tac.Add -> "add"
            | Tac.Sub -> "sub"
            | Tac.Mul -> "mul"
            | Tac.Div -> "div"
            | Tac.Mod -> "mod"
            | Tac.And -> "and"
            | Tac.Or -> "or"
            | Tac.Not -> "not"
            | Tac.Eq -> "ceq"
            | Tac.NotEq -> "cne"
            | Tac.Lt -> "clt"
            | Tac.LtEq -> "cle"
            | Tac.Gt -> "cgt"
            | Tac.GtEq -> "cge"

type Tac.Instruction with
    static member ToMoon instruction : seq<Moon.Ast.Line> = seq {
        match instruction with 
            
            | Assign(dest, src) as a ->
                yield Line(None, Blank, Some (a.ToString()))
                yield! loadInRegister Registers.Acc1 src
                yield! storeRegister dest Registers.Acc1
            
            | Tac.Call(Label(i), paramCount) as c ->
                yield Line(None, Blank, Some (c.ToString()))
                yield Line(None,
                    Instruction("add", [Register(Registers.ParameterPassing);Register(0);Register(0)]),
                    Some "Clear the prameter passing register")
                yield Line(
                    None, 
                    Instruction("jl", [Register(Registers.ReturnAddress);Constant(Symbol(sprintf "L%i" i), None)]), 
                    None)
            
            | Tac.Inst3(dest, op, operand1, operand2) as i3 ->
                let operation = Tac.Operator.ToMoonOperation op
                yield Line(None, Blank, Some (i3.ToString()))
                yield! loadInRegister Registers.Acc1 operand1
                yield! loadInRegister Registers.Acc2 operand2
                yield Line(None, 
                    Instruction(operation, [Register(Registers.Acc1);Register(Registers.Acc1);Register(Registers.Acc2)]), 
                    None)
                yield! storeRegister dest Registers.Acc1
                
            | Tac.Labeled(Label(i)) ->
                yield Line(Some(sprintf "L%i" i), Blank, None)
            
            | Tac.Goto(Label(label)) as g ->
                yield Line(None, Instruction("j", [Constant(Symbol(sprintf "L%i" label), None)]), Some(g.ToString()))
            
            | Tac.Return as r ->
                yield Line(None, Instruction("jr", [Register(Registers.ReturnAddress)]), Some(r.ToString()))
            
            | Tac.Write(src) as w ->
                yield Line(None, Blank, Some (w.ToString()))
                yield! loadInRegister Registers.Acc1 src
                yield Line(None, Instruction("putc", [Register(Registers.Acc1)]), None)

            | Tac.Read(dest) as tacInstruction ->
                yield Line(None, Blank, Some(tacInstruction.ToString()))
                yield Line(None, Instruction("getc", [Register(Registers.Acc1)]), None)
                yield! storeRegister dest Registers.Acc1

            | Tac.IfFalse(ptr, Label(i)) as tacInstruction ->
                yield Line(None, Blank, Some (tacInstruction.ToString()))
                yield! loadInRegister Registers.Acc1 ptr
                yield Line(None, Instruction("bz", [Register(Registers.Acc1);Constant(Symbol(sprintf "L%i" i), None)]), None)
            
            | Tac.ArrayDeref(dest, arrayPtr, indexPtr) as tacInstruction ->
                yield Line(None, Blank, Some (tacInstruction.ToString()))
                yield! GetArrayEntryAddress arrayPtr indexPtr Registers.Acc1 Registers.Acc2
                // Read what's there
                yield Line(None, Instruction("lw", [Register(Registers.Acc1);Constant(Number(0s), Some Registers.Acc1)]),
                    Some "Read the value at the index");
                // Store it at dest
                yield! storeRegister dest Registers.Acc1

            | Tac.ArrayAssign(arrayPtrPtr, indexPtr, srcPtr) as tacInstruction ->
                yield Line(None, Blank, Some (tacInstruction.ToString()))
                yield! GetArrayEntryAddress arrayPtrPtr indexPtr Registers.Acc1 Registers.Acc2
                yield! loadInRegister Registers.Acc2 srcPtr
                yield Line(None, Instruction("sw", [Constant(Number(0s), Some Registers.Acc1);Register(Registers.Acc2)]),
                    Some "Set value in array")
            
            | Tac.Push(ptr) as tacInstruction ->
                
                yield Line(None, Blank, Some (tacInstruction.ToString()))
                // Add the current stack size to the stack pointer
                yield Line(None, Instruction("add", [Register(Registers.Acc1);Register(Registers.TopStack);Register(Registers.FrameSize)]), 
                    Some "Find the start of the next frame by adding the top of the stack to the current frame's size")
                yield Line(None, Instruction("addi", [Register(Registers.Acc1);Register(Registers.Acc1);Constant(Number(12s), None)]), 
                    Some "Add 12 for the head of the of the next frame")
                yield Line(None, Instruction("add", [Register(Registers.Acc1); Register(Registers.Acc1);Register(Registers.ParameterPassing)]),
                    Some "Add the parameter passing register's offset to that")
                // Read the value at ptr
                yield! loadInRegister Registers.Acc2 ptr
                yield Line(None, Instruction("sw", [Constant(Number(0s), Some Registers.Acc1); Register(Registers.Acc2)]),
                    Some "Store the argument on the stack");
                yield Line(None, Instruction("addi", [Register(Registers.ParameterPassing); Register(Registers.ParameterPassing);Constant(Number(4s), None)]),
                    Some "Add 4 to the parameter passing register")

            | Tac.Entry as tacInstruction ->
                // Define where the program starts and some initializations
                yield Line(None, Blank, Some(tacInstruction.ToString()))
                yield Line(None, Instruction("entry", []), None)
                yield Line(None, 
                    Instruction("addi", [Register(Registers.TopStack);Register(0);Constant(Symbol(Labels.Stack), None)]), 
                    Some "Initialize top stack address")

            | Tac.Halt as tacInstruction ->
                yield Line(None, Blank, Some(tacInstruction.ToString()))
                yield Line(None, Instruction("hlt", []), None)
    }

type Tac.Program with
    static member ToMoon program = seq {
        // Allocate space for globals
        match program with
            | Tac.Program(instructions, frameSizes, constants, globalSize) ->
                // Print some helpful information
                let messages = 
                    [
                        sprintf "Top of stack: r%i" Registers.TopStack
                        sprintf "Global vars: r%i" Registers.GlobalVarsAddress
                        sprintf "Accumulator 1: r%i" Registers.Acc1
                        sprintf "Accumulator 2: r%i" Registers.Acc2
                        sprintf "Current Frame size: r%i" Registers.FrameSize
                        sprintf "Parameter passing: r%i" Registers.ParameterPassing
                        sprintf "helper: r%i" Registers.Helper
                        sprintf "Return value: r%i" Registers.Result
                        sprintf "Return address: r%i" Registers.ReturnAddress
                        sprintf " "
                    ]

                for message in messages do
                    yield Line(None, Blank, Some message)

                // Translate the Tac Instructions
                for instruction in instructions do
                    yield! Instruction.ToMoon instruction

                // Define the data for the program
                yield! defineFrameSizes frameSizes
                yield! defineConstants constants
                yield defineGlobalSection globalSize
                yield Line(Some Labels.Stack, Directive(Res(StackSize)), Some "The stack")
     }
                  