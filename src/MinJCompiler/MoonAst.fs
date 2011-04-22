module Moon.Ast

type DwK = DwKInt of int32 | DwKStr of string
type Directive = 
    | Entry 
    | Align 
    | Org of uint32 
    | Dw of DwK list
    | Db of int32 list
    | Res of uint32

type Register = | Register of int

type Constant =
    | Number of int32
    | Symbol of string
type Operand =
    | Register of Register 
    | Constant of Constant * Register option
    | String of string

type LineContent =
    | Instruction of string * Operand list
    | Directive of Directive
type Line = 
    | Line of string option * LineContent * string option