module Tac.ToMoon
open Moon.Ast
open Compiler
open Tac.Printing

let FrameSizesLabel = "fz"
let ConstantsLabel = "cs"
let GlobalsLabel = "gb"

// Register definitions
let TopStackRegister = 1
let GlobalVarsAddressRegister = 2
let AccRegister1 = 3
let AccRegister2 = 4
let AccRegister3 = 5
let FrameSizeRegister = 10
let HelperRegister = 13
let ResultRegister = 14
let ReturnAddressRegister = 15

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

let loadAddress register address = seq {
    yield Line(None, Instruction("addi", [Register(register); Register(0); Constant(Number(address), None)]), None)
}

let loadOffsetFromLabel register address label = seq {
    yield! loadAddress register address
    yield Line(None, Instruction("lw", [Register(register); Constant(Symbol(label), Some register)]), None)
}

let setTo register number = seq {
    yield Line(None, Instruction("addi", [Register(register); Register(0); Constant(Number(number), None)]), None)
}

let loadInRegister dest src = seq {
    match src with
        | Tac.Global(address) ->
            yield! loadOffsetFromLabel dest address GlobalsLabel
        | Tac.Constant(address) ->
            yield! loadOffsetFromLabel dest address ConstantsLabel
        | Tac.Local(address) | Tac.Param(address) ->
            // Add ParameterStart + address to the register
            yield Line(None, Instruction("addi", [Register(dest); Register(TopStackRegister); Constant(Number(address), None)]), None)
            // Load where at the address at the register
            yield Line(None, Instruction("lw", [Register(dest); Constant(Number(0u), Some(dest))]), None)
        | Tac.TopSt ->
            yield Line(None, Instruction("add", [Register(dest); Register(0); Register(TopStackRegister)]), 
                Some(sprintf "Put the address of the top of the stack in r%i" dest))
        | Tac.Frame(index) ->
            yield! loadOffsetFromLabel dest ((uint32)index) FrameSizesLabel
        | Tac.RetAdd ->
            yield Line(None, Instruction("add", [Register(dest); Register(0); Register(ReturnAddressRegister)]), 
                Some(sprintf "Put the return address into r%i" dest))
        | Tac.Result ->
            yield Line(None, Instruction("add", [Register(dest);Register(0);Register(ResultRegister)]), 
                Some(sprintf "Put the returned value into r%i" dest))
        | Tac.FrSz ->
            yield Line(None, Instruction("add", [Register(dest);Register(0);Register(FrameSizeRegister)]), 
                Some(sprintf "Put the current frame's size value into r%i" dest))
}

let storeRegister dest src = seq {
    match dest with
        | Tac.Global(address) ->
            yield! setTo HelperRegister address
            yield Line(None, Instruction("sw", [Constant(Symbol(GlobalsLabel), Some(HelperRegister)); Register(src)]), None)
        | Tac.Local(address) | Tac.Param(address) ->
            // Add ParameterStart + address to the register
            yield Line(None, Instruction("addi", [Register(HelperRegister); Register(TopStackRegister); Constant(Number(address), None)]), None)
            yield Line(None, Instruction("sw", [Constant(Number(0u), Some(HelperRegister));Register(src)]), None)
        | Tac.TopSt ->
            yield Line(None, Instruction("add", [Register(TopStackRegister);Register(0);Register(src)]), None)
        | Tac.RetAdd ->
            yield Line(None, Instruction("add", [Register(ReturnAddressRegister); Register(0); Register(src)]), None)
        | Tac.Result ->
            yield Line(None, Instruction("add", [Register(ResultRegister); Register(0); Register(src)]), None)
        | Tac.FrSz ->
            yield Line(None, Instruction("add", [Register(FrameSizeRegister);Register(0);Register(src)]), None)
        | Tac.Constant(_) | Tac.Frame(_) ->
            raise <| CompilerException([("Attempted to set constant.", Scanner.Tokens.OriginLocation)])
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
                yield Line(None, Blank, Some (a.ToAssembly()))
                yield! loadInRegister AccRegister1 src
                yield! storeRegister dest AccRegister1
            | Tac.Call(Label(i), paramCount) as c ->
                yield Line(
                    None, 
                    Instruction("jl", [Register(ReturnAddressRegister);Constant(Symbol(sprintf "L%i" i), None)]), 
                    Some(c.ToAssembly()))
            | Tac.Inst3(dest, op, operand1, operand2) as i3 ->
                let operation = Tac.Operator.ToMoonOperation op
                yield Line(None, Blank, Some (i3.ToAssembly()))
                yield! loadInRegister AccRegister1 operand1
                yield! loadInRegister AccRegister2 operand2
                yield Line(None, 
                    Instruction(operation, [Register(AccRegister1);Register(AccRegister1);Register(AccRegister2)]), 
                    None)
                yield! storeRegister dest AccRegister1
                
            | Labeled(Label(i)) ->
                yield Line(Some(sprintf "L%i" i), Blank, None)
            | Goto(Label(label)) as g ->
                yield Line(None, Instruction("j", [Constant(Symbol(sprintf "L%i" label), None)]), Some(g.ToAssembly()))
            | Return as r ->
                yield Line(None, Instruction("jr", [Register(ReturnAddressRegister)]), Some(r.ToAssembly()))
            | Write(src) as w ->
                yield Line(None, Blank, Some (w.ToAssembly()))
                yield! loadInRegister AccRegister1 src
                yield Line(None, Instruction("putc", [Register(AccRegister1)]), None)
            | _ -> ()
    }

type Tac.Program with
    static member ToMoon program = seq {
        // Allocate space for globals
        match program with
            | Tac.Program(instructions, frameSizes, constants, globalSize) ->
                yield Line(Some "Stack", Directive(Res(10000u)), Some "The stack")
                yield! defineFrameSizes frameSizes
                yield! defineConstants constants
                yield defineGlobalSection globalSize
                yield Line(None, Directive(Entry), Some "Start here")
                yield Line(None, 
                    Instruction("add", [Register(TopStackRegister);Register(0);Register(0)]), 
                    Some "Initialize top stack to 0")
                yield Line(None,
                    Instruction("add", [Register(FrameSizeRegister);Register(0);Register(0)]),
                    Some "Initialize current frame size to zero")
                yield Line(None, Instruction("jl", [Register(ReturnAddressRegister);Constant(Symbol("L0"), None)]), None)
                yield Line(None, Instruction("hlt", []), None)
                for instruction in instructions do
                    yield! Instruction.ToMoon instruction
     }
                  