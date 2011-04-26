module Compiler.Tac.ToMoon

open Moon.Ast
open Compiler

open System

// Register definitions
let TopStackRegister = 1
let GlobalVarsAddressRegister = 2
let AccRegister1 = 3
let AccRegister2 = 4
let AccRegister3 = 5
let FrameSizeRegister = 10
let ParameterPassingRegister = 11
let HelperRegister = 13
let ResultRegister = 14
let ReturnAddressRegister = 15

// Labels
let FrameSizesLabel = "fz"
let ConstantsLabel = "cs"
let GlobalsLabel = "gb"
let StackLabel = "st"

let StackSize = 10000u

type ProgramContext = {
    ConstantsOffset : uint32;
    FrameSizeTableOffset : uint32;
}

let defineFrameSize label size = 
    Line(label, Directive(Dw([DwKInt((int32)size)])), None)

let defineFrameSizes frameSizes = seq {
    yield defineFrameSize (Some FrameSizesLabel) (List.head frameSizes)
    for size in List.tail frameSizes do
        yield defineFrameSize None size
}

let uint32ToInt16 number =
    if number > (uint32)Int16.MaxValue then
        raise <| CompilerException([("An operand was out of bounds while compiling to Moon", Location.origin)])
    (int16)number

let defineConstant label literal =
    match literal with
        | CharLiteral(char) ->
            Line(label, Directive(Dw [DwKInt((int32)char)]), None)
        | NumberLiteral(i) ->
            Line(label, Directive(Dw [DwKInt((int32)i)]), None)

let defineConstants literals = seq {
    if (List.length literals > 0) then
        yield defineConstant (Some ConstantsLabel) (List.head literals)
        for literal in List.tail literals do
            yield defineConstant None literal
}

let defineGlobalSection size = 
    Line(Some GlobalsLabel, Directive(Res size), None)

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
            yield! loadOffsetFromLabel destRegister (uint32ToInt16 address) GlobalsLabel
        
        | Tac.Constant(address) ->
            yield! loadOffsetFromLabel destRegister (uint32ToInt16 address) ConstantsLabel
        
        | Tac.Local(address) | Tac.Param(address) ->
            
            // Add ParameterStart + address to the register
            yield Line(None, Instruction("addi", [Register(destRegister); Register(TopStackRegister); Constant(Number(uint32ToInt16 address), None)]), None)
            // Load where at the address at the register
            yield Line(None, Instruction("lw", [Register(destRegister); Constant(Number(0s), Some(destRegister))]), None)
        
        | Tac.TopSt ->
            yield Line(None, Instruction("add", [Register(destRegister); Register(0); Register(TopStackRegister)]), 
                Some(sprintf "Put the address of the top of the stack in r%i" destRegister))
        
        | Tac.Frame(address) ->
            yield! loadOffsetFromLabel destRegister (uint32ToInt16 address) FrameSizesLabel
        
        | Tac.RetAdd ->
            yield Line(None, Instruction("add", [Register(destRegister); Register(0); Register(ReturnAddressRegister)]), 
                Some(sprintf "Put the return address into r%i" destRegister))
        
        | Tac.Result ->
            yield Line(None, Instruction("add", [Register(destRegister);Register(0);Register(ResultRegister)]), 
                Some(sprintf "Put the returned value into r%i" destRegister))
        
        | Tac.FrSz ->
            yield Line(None, Instruction("add", [Register(destRegister);Register(0);Register(FrameSizeRegister)]), 
                Some(sprintf "Put the current frame's size value into r%i" destRegister))
        | Tac.Globals ->
            yield Line(None, Instruction("add", [Register(destRegister); Register(0); Register(GlobalVarsAddressRegister)]), 
                None)
}

let storeRegister dest src = seq {
    match dest with
        | Tac.Global(address) ->
            yield! setTo HelperRegister (uint32ToInt16 address)
            yield Line(None, Instruction("sw", [Constant(Symbol(GlobalsLabel), Some(HelperRegister)); Register(src)]), None)
        | Tac.Local(address) | Tac.Param(address) ->
            // Add ParameterStart + address to the register
            yield Line(None, Instruction("addi", [Register(HelperRegister); Register(TopStackRegister); Constant(Number(uint32ToInt16 address), None)]), None)
            yield Line(None, Instruction("sw", [Constant(Number(0s), Some(HelperRegister));Register(src)]), None)
        | Tac.TopSt ->
            yield Line(None, Instruction("add", [Register(TopStackRegister);Register(0);Register(src)]), None)
        | Tac.RetAdd ->
            yield Line(None, Instruction("add", [Register(ReturnAddressRegister); Register(0); Register(src)]), None)
        | Tac.Result ->
            yield Line(None, Instruction("add", [Register(ResultRegister); Register(0); Register(src)]), None)
        | Tac.FrSz ->
            yield Line(None, Instruction("add", [Register(FrameSizeRegister);Register(0);Register(src)]), None)
        | Tac.Constant(_) | Tac.Frame(_) | Tac.Globals ->
            raise <| CompilerException([("Attempted to set constant.", Location.origin)])
}

let GetVariableAddress ptr =
    match ptr with
        | Global(address) | Local(address) | Param(address) ->
            address
        | _ ->
            raise <| CompilerException([("Expected a variable", Location.origin)])

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
                yield! loadInRegister AccRegister1 src
                yield! storeRegister dest AccRegister1
            
            | Tac.Call(Label(i), paramCount) as c ->
                yield Line(None, Blank, Some (c.ToString()))
                yield Line(None,
                    Instruction("add", [Register(ParameterPassingRegister);Register(0);Register(0)]),
                    Some "Clear the prameter passing register")
                yield Line(
                    None, 
                    Instruction("jl", [Register(ReturnAddressRegister);Constant(Symbol(sprintf "L%i" i), None)]), 
                    None)
            
            | Tac.Inst3(dest, op, operand1, operand2) as i3 ->
                let operation = Tac.Operator.ToMoonOperation op
                yield Line(None, Blank, Some (i3.ToString()))
                yield! loadInRegister AccRegister1 operand1
                yield! loadInRegister AccRegister2 operand2
                yield Line(None, 
                    Instruction(operation, [Register(AccRegister1);Register(AccRegister1);Register(AccRegister2)]), 
                    None)
                yield! storeRegister dest AccRegister1
                
            | Tac.Labeled(Label(i)) ->
                yield Line(Some(sprintf "L%i" i), Blank, None)
            
            | Tac.Goto(Label(label)) as g ->
                yield Line(None, Instruction("j", [Constant(Symbol(sprintf "L%i" label), None)]), Some(g.ToString()))
            
            | Tac.Return as r ->
                yield Line(None, Instruction("jr", [Register(ReturnAddressRegister)]), Some(r.ToString()))
            
            | Tac.Write(src) as w ->
                yield Line(None, Blank, Some (w.ToString()))
                yield! loadInRegister AccRegister1 src
                yield Line(None, Instruction("putc", [Register(AccRegister1)]), None)

            | Tac.Read(dest) as tacInstruction ->
                yield Line(None, Blank, Some(tacInstruction.ToString()))
                yield Line(None, Instruction("getc", [Register(AccRegister1)]), None)
                yield! storeRegister dest AccRegister1

            | Tac.IfFalse(ptr, Label(i)) as tacInstruction ->
                yield Line(None, Blank, Some (tacInstruction.ToString()))
                yield! loadInRegister AccRegister1 ptr
                yield Line(None, Instruction("bz", [Register(AccRegister1);Constant(Symbol(sprintf "L%i" i), None)]), None)
            
            | Tac.ArrayDeref(dest, arrayPtr, indexPtr) as tacInstruction ->
                yield Line(None, Blank, Some (tacInstruction.ToString()))
                yield! GetArrayEntryAddress arrayPtr indexPtr AccRegister1 AccRegister2
                // Read what's there
                yield Line(None, Instruction("lw", [Register(AccRegister1);Constant(Number(0s), Some AccRegister1)]),
                    Some "Read the value at the index");
                // Store it at dest
                yield! storeRegister dest AccRegister1

            | Tac.ArrayAssign(arrayPtrPtr, indexPtr, srcPtr) as tacInstruction ->
                yield Line(None, Blank, Some (tacInstruction.ToString()))
                yield! GetArrayEntryAddress arrayPtrPtr indexPtr AccRegister1 AccRegister2
                yield! loadInRegister AccRegister2 srcPtr
                yield Line(None, Instruction("sw", [Constant(Number(0s), Some AccRegister1);Register(AccRegister2)]),
                    Some "Set value in array")
            
            | Tac.Push(ptr) as tacInstruction ->
                
                yield Line(None, Blank, Some (tacInstruction.ToString()))
                // Add the current stack size to the stack pointer
                yield Line(None, Instruction("add", [Register(AccRegister1);Register(TopStackRegister);Register(FrameSizeRegister)]), 
                    Some "Find the start of the next frame by adding the top of the stack to the current frame's size")
                yield Line(None, Instruction("addi", [Register(AccRegister1);Register(AccRegister1);Constant(Number(12s), None)]), 
                    Some "Add 12 for the head of the of the next frame")
                yield Line(None, Instruction("add", [Register(AccRegister1); Register(AccRegister1);Register(ParameterPassingRegister)]),
                    Some "Add the parameter passing register's offset to that")
                // Read the value at ptr
                yield! loadInRegister AccRegister2 ptr
                yield Line(None, Instruction("sw", [Constant(Number(0s), Some AccRegister1); Register(AccRegister2)]),
                    Some "Store the argument on the stack");
                yield Line(None, Instruction("addi", [Register(ParameterPassingRegister); Register(ParameterPassingRegister);Constant(Number(4s), None)]),
                    Some "Add 4 to the parameter passing register")

            | Tac.Entry as tacInstruction ->
                // Define where the program starts and some initializations
                yield Line(None, Blank, Some(tacInstruction.ToString()))
                yield Line(None, Instruction("entry", []), None)
                yield Line(None, 
                    Instruction("addi", [Register(TopStackRegister);Register(0);Constant(Symbol(StackLabel), None)]), 
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
                        sprintf "Top of stack: r%i" TopStackRegister
                        sprintf "Global vars: r%i" GlobalVarsAddressRegister
                        sprintf "Accumulator 1: r%i" AccRegister1
                        sprintf "Accumulator 2: r%i" AccRegister2
                        sprintf "Accumulator 3: r%i" AccRegister3
                        sprintf "Current Frame size: r%i" FrameSizeRegister
                        sprintf "Parameter passing: r%i" ParameterPassingRegister
                        sprintf "helper: r%i" HelperRegister
                        sprintf "Return value: r%i" ResultRegister
                        sprintf "Return address: r%i" ReturnAddressRegister
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
                yield Line(Some StackLabel, Directive(Res(StackSize)), Some "The stack")
     }
                  