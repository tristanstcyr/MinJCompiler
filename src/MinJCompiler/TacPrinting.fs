/// Contains extensions to Three Address Code types for printing them.
/// The printed representation are almost identical to the class slides.
module Tac.Printing

open System.IO
open Tac

type Operator with
    member this.ToAssembly() =
        match this with
            | Add       -> "+"
            | Sub       -> "-"
            | Mul       -> "*"
            | Div       -> "/"
            | Mod       -> "%"
            | And       -> "&&"
            | Or        -> "||"
            | Not       -> "!"
            | Eq        -> "=="
            | NotEq     -> "!="
            | Lt      -> "<"
            | LtEq    -> "<="
            | Gt   -> ">"
            | GtEq -> ">="

type Ptr with
    member this.ToAssembly() =
        match this with
            | Global(i) -> sprintf "(g, %i)" i
            | Local(i) -> sprintf "(l, %i)" i
            | Param(i) -> sprintf "(p, %i)" i
            | Constant(i) -> sprintf "(c, %i)" i
            | Frame(i) -> sprintf "(f, %i)" i
            | TopSt -> "TopSt"
            | FrSz -> "FrSz"
            | RetAdd -> "RetAdd"

type Label with
    member this.ToAssembly() =
        match this with
            | Label(i) -> sprintf "L%i" i

type Instruction with
    member this.ToAssembly() =
        match this with
            | Assign(dest, src) ->
                sprintf "%s, =, %s" 
                    <| dest.ToAssembly() 
                    <| src.ToAssembly()
            | ArrayDeref(dest, arr, index) ->
                sprintf "%s, %s, [, %s" 
                    <| dest.ToAssembly() 
                    <| arr.ToAssembly() 
                    <| index.ToAssembly()
            | ArrayAssign(arr, index, src) ->
                sprintf "%s, [, %s, %s" 
                    <| arr.ToAssembly() 
                    <| index.ToAssembly() 
                    <| src.ToAssembly()
            | Call(lbl, paramCount) ->
                sprintf "call %s, %i" <| lbl.ToAssembly() <| paramCount
            | Inst3(ptr1, op, ptr2, ptr3) ->
                sprintf "%s, %s, %s, %s"
                    <| ptr1.ToAssembly()
                    <| op.ToAssembly()
                    <| ptr2.ToAssembly()
                    <| ptr3.ToAssembly()
            | Labeled(lbl) ->
                lbl.ToAssembly()
            | IfFalse(ptr, lbl) ->
                sprintf "if_false %s goto %s"
                    <| ptr.ToAssembly()
                    <| lbl.ToAssembly()
            | Write(ptr) ->
                sprintf "write %s" <| ptr.ToAssembly()
            | Read(ptr) ->
                sprintf "read %s" <| ptr.ToAssembly()
            | Push(ptr) ->
                sprintf "param %s" <| ptr.ToAssembly()
            | Goto(lbl) ->
                sprintf "goto %s" <| lbl.ToAssembly()
            | Return ->
                "return"

type TextWriter with
    member this.PrintInstructions instructions =
        for instruction in instructions do
            match instruction with
                | Labeled(lbl) ->
                    this.Write(lbl.ToAssembly())
                | _  ->
                    this.Write("\t")
                    this.WriteLine(instruction.ToAssembly())

    member this.PrintProgram program =
        match program with
            | Program(instructions, frameSizes, constants, globalSize) ->
                this.PrintInstructions instructions
                this.Write('f')
                for frameSize in frameSizes do
                    this.Write('\t')
                    this.WriteLine(frameSize)
                this.WriteLine(sprintf "Size\t%i" globalSize)
                this.WriteLine("ST")
    