module Tac.ToMoon
open Moon.Ast

let FrameSizesLabel = "fz"
let ConstantsLabel = "cs"
let GlobalsLabel = "gb"

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
    yield defineConstant (Some ConstantsLabel) (List.head literals)
    for literal in List.tail literals do
        yield defineConstant None literal
}

let defineGlobalSection size = 
    Line(Some GlobalsLabel, Directive(Res size), None)

type Tac.Program with
    static member ToMoon program = seq {
        // Allocate space for globals
        match program with
            | Tac.Program(instructions, frameSizes, constants, globalSize) ->
                yield! defineFrameSizes frameSizes
                yield! defineConstants constants
                yield defineGlobalSection globalSize
                yield Line(None, Directive(Entry), Some "Start here")
                yield Line(None, Instruction("j", [Constant(Symbol("L0"), None)]), None)
     }
                  