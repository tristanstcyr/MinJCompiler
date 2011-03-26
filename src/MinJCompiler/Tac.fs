namespace Tac

open System.IO

type Ptr = 
    | Global of int
    | Local of int 
    | Param of int 
    | Constant of int
    | Frame of int
    | TopSt | FrSz | RetAdd

type TacOperator = 
    | Add | Sub | Mul | Div | Mod | ArrayAccess
    | And | Or | Not | Eq | NotEq | Lt | LtEq | Gt | GtEq
    
type Label = | Label of int

type Literal =
    | CharLiteral of char
    | NumberLiteral of int64

type Instruction =
    | Assign of Ptr * Ptr
    | ArrayDeref of Ptr * Ptr * Ptr
    | ArrayAssign of Ptr * Ptr * Ptr
    | Call of Label * int
    | Inst3 of Ptr * TacOperator * Ptr * Ptr
    | Labeled of Label
    | IfFalse of Ptr * Label
    | Write of Ptr
    | Read of Ptr
    | Push of Ptr
    | Goto of Label
    | Return

type GlobalsSize = int
type FrameSize = int
type Constants = Literal list
type Program = | Program of Instruction seq * FrameSize list * Constants * GlobalsSize
