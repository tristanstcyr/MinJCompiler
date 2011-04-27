/// Contains extensions to Three Address Code types for printing them.
/// The printed representation are almost identical to the class slides.
[<AutoOpen>]
module Compiler.Tac.ToString

open Compiler.Tac

open System.IO

type Operator with
    static member ToString this =
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
    static member ToString this =
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
    static member ToString this =
        match this with
            | Label(i) -> sprintf "L%i" i

type Instruction with
    static member ToString this =
        match this with
            | Assign(dest, src) ->
                sprintf "%s, =, %s" 
                    <| Ptr.ToString dest 
                    <| Ptr.ToString src
            | ArrayDeref(dest, arr, index) ->
                sprintf "%s, %s, [, %s" 
                    <| Ptr.ToString dest
                    <| Ptr.ToString arr 
                    <| Ptr.ToString index
            | ArrayAssign(arr, index, src) ->
                sprintf "%s, [, %s, %s" 
                    <| Ptr.ToString arr 
                    <| Ptr.ToString index
                    <| Ptr.ToString src
            | Call(lbl, paramCount) ->
                sprintf "call %s, %i" <| Label.ToString lbl <| paramCount
            | Inst3(ptr1, op, ptr2, ptr3) ->
                sprintf "%s, %s, %s, %s"
                    <| Ptr.ToString ptr1
                    <| Operator.ToString op
                    <| Ptr.ToString ptr2
                    <| Ptr.ToString ptr3
            | Labeled(lbl) ->
                lbl.ToString()
            | IfFalse(ptr, lbl) ->
                sprintf "if_false %s goto %s"
                    <| Ptr.ToString ptr
                    <| Label.ToString lbl
            | Write(ptr) ->
                sprintf "write %s" <| Ptr.ToString ptr
            | Read(ptr) ->
                sprintf "read %s" <| Ptr.ToString ptr
            | Push(ptr) ->
                sprintf "param %s" <| Ptr.ToString ptr
            | Goto(lbl) ->
                sprintf "goto %s" <| Label.ToString lbl
            | Return -> "return"
            | Halt -> "halt"
            | Entry -> "start"