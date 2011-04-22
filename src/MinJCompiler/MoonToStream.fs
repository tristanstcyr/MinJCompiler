module Moon.ToStream
open Ast
open System.IO

let private asCommaList (out : TextWriter) func (l : 'a list) =
    func out (l.Head)
    for k in l.Tail do
        out.Write(",")
        func out k
         
type Register with
    static member ToStream(out : TextWriter) this =
        match this with
            | Register.Register r ->
                out.Write("r")
                out.Write(r)

type Operand with
    static member ToStream(out : TextWriter) this =
        match this with
            | Register(r) ->
                Register.ToStream out r
            | Constant(constant, register) ->
                match constant with
                    | Number(i) ->
                        out.Write(constant)
                    | Symbol(str) ->
                        out.Write(str)
                match register with
                    | Some(r) ->
                        Register.ToStream out r
                    | None -> ()
            | String(str) ->
                out.Write('"')
                out.Write(str)
                out.Write('"')

type DwK with
    static member ToStream(out : TextWriter) this =
        match this with
            | DwKInt(i) ->
                out.Write(i)
            | DwKStr(str) ->
                out.Write('"')
                out.Write(str)
                out.Write('"')

type Directive with
    static member ToStream(out : TextWriter) this =
        match this with
            | Entry ->
                out.Write("entry")
            | Align ->
                out.Write("align")
            | Org(addr) ->
                out.Write("org ")
                out.Write(addr)
            | Dw(ks) ->
                out.Write("dw ")
                asCommaList out DwK.ToStream ks
            | Db(is) ->
                out.Write("db ")
                asCommaList out (fun out (i : int32) -> out.Write(i)) is
            | Res(i) ->
                out.Write("res ")
                out.Write(i)
                

type LineContent with
    static member ToStream (out : TextWriter) this =
        match this with
            | Instruction(code, operands) ->
                out.Write(code)
                out.Write(" ")
                asCommaList out Operand.ToStream operands

            | Directive(directive) ->
                Directive.ToStream out directive

type Line with
    static member ToStream (out : TextWriter) (Line(symbol, content, comment)) =
        match symbol with
            | Some(str) ->
                out.Write(str)                        
            | None -> ()

        out.Write("\t")
        LineContent.ToStream out content
                
        match comment with
            | Some(str) ->
                out.Write("\t")
                out.Write("% ")
                out.Write(str)
            | None -> ()


let write (out : TextWriter) lines =
    for line in lines do
        Line.ToStream out line
        out.Write("\n")