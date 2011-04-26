/// Contains extensions to Three Address Code types for printing them.
/// The printed representation are almost identical to the class slides.
[<AutoOpen>]
module Compiler.Tac.ToString

open Compiler.Tac

open System.IO

type Operator with
    member this.ToString() =
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
    member this.ToString() =
        match this with
            | Global(i) -> sprintf "(g, %i)" i
            | Local(i) -> sprintf "(l, %i)" i
            | Param(i) -> sprintf "(p, %i)" i
            | Constant(i) -> sprintf "(c, %i)" i
            | TopSt -> "TopSt"
            | Frame(i) -> sprintf "(f, %i)" i
            | RetAdd -> "RetAdd"
            | Result -> "Result"
            | FrSz -> "FrSz"
            | Globals -> "Globals"

type Label with
    member this.ToString() =
        match this with
            | Label(i) -> sprintf "L%i" i

type Instruction with
    member this.ToString() =
        match this with
            | Assign(dest, src) ->
                sprintf "%s, =, %s" 
                    <| dest.ToString() 
                    <| src.ToString()
            | ArrayDeref(dest, arr, index) ->
                sprintf "%s, %s, [, %s" 
                    <| dest.ToString() 
                    <| arr.ToString() 
                    <| index.ToString()
            | ArrayAssign(arr, index, src) ->
                sprintf "%s, [, %s, %s" 
                    <| arr.ToString() 
                    <| index.ToString() 
                    <| src.ToString()
            | Call(lbl, paramCount) ->
                sprintf "call %s, %i" <| lbl.ToString() <| paramCount
            | Inst3(ptr1, op, ptr2, ptr3) ->
                sprintf "%s, %s, %s, %s"
                    <| ptr1.ToString()
                    <| op.ToString()
                    <| ptr2.ToString()
                    <| ptr3.ToString()
            | Labeled(lbl) ->
                lbl.ToString()
            | IfFalse(ptr, lbl) ->
                sprintf "if_false %s goto %s"
                    <| ptr.ToString()
                    <| lbl.ToString()
            | Write(ptr) ->
                sprintf "write %s" <| ptr.ToString()
            | Read(ptr) ->
                sprintf "read %s" <| ptr.ToString()
            | Push(ptr) ->
                sprintf "param %s" <| ptr.ToString()
            | Goto(lbl) ->
                sprintf "goto %s" <| lbl.ToString()
            | Return -> "return"
            | Halt -> "halt"
            | Entry -> "start"