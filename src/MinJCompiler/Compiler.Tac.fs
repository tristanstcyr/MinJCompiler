namespace Compiler.Tac

(* Abstract syntax tree of Three Address Code *)

open System.IO

/// A pointer to a value.
type Ptr = 
    /// The address of a global value from a field in MinJ.
    | Global of uint32
    /// The address of a local value.
    | Local of uint32 
    /// The address of a parameter.
    | Param of uint32 
    /// The address of a constnat.
    | Constant of uint32
    /// The top of the current stack.
    | TopSt 
    /// The frame size of a function
    | Frame of uint32
    /// The frame size of the current function
    | FrSz
    /// The address to return to.
    | RetAdd
    /// The address for return values of functons
    | Result
    /// Address where the globals start
    | Globals

type Operator = 
    | Add | Sub | Mul | Div | Mod
    | And | Or | Not | Eq | NotEq | Lt | LtEq | Gt | GtEq
    
/// A label in a three address code program.
type Label = | Label of int

/// A literal in the program. These are stored as constants.
type Literal =
    | CharLiteral of char
    | NumberLiteral of int32

/// An instruction in a three addres code program.
type Instruction =
    | Entry
    /// Assignment of a 32-bit value at a location
    /// to another location. The first pointer is
    /// the destination, the second pointer is the source.
    | Assign of Ptr * Ptr
    /// Dereferencing on an array. 
    /// The first pointer is the destination.
    /// The second pointer points to a ptr that is array address start
    /// The third pointer point to an index value.
    | ArrayDeref of Ptr * Ptr * Ptr
    /// Assigns a value to an element in a vector.
    /// The first pointer is the address of the array.
    /// The second pointer points to an index.
    /// The third pointer points to the value to assign.
    | ArrayAssign of Ptr * Ptr * Ptr
    /// A call to a function at a label with a number of parameters.
    /// It is expected that this number of parameter have been pushed
    /// before the this instruction.
    | Call of Label * int
    /// A 3 pointer operation.
    /// The first pointer points to the destination.
    /// The second pointer points to the first operand.
    /// The third pointer points to the second operand.
    | Inst3 of Ptr * Operator * Ptr * Ptr
    /// Instruction after this is labeled.
    | Labeled of Label
    /// Jumps to a label if a value at an address is false.
    | IfFalse of Ptr * Label
    /// Writes the value at an address to standard out.
    | Write of Ptr
    /// Reads a value from standard in an places it at an address.
    | Read of Ptr
    /// Adds a param to the next function call.
    | Push of Ptr
    /// Jumps to a label.
    | Goto of Label
    /// Returns from the current function.
    | Return
    | Halt

type GlobalsSize = uint32
type FrameSize = uint32
type Constants = Literal list
/// A three address code program.
type Program = | Program of Instruction seq * FrameSize list * Constants * GlobalsSize
