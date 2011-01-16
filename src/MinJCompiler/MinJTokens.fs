module MinJTokens
open Tokens

(* ll* *)
type Identifier(str, startloc : Location) =
    inherit Token(str, startloc)

(* [0-9]* *)
type Number(str, startloc : Location) =
    inherit Token(str, startloc)

(* = *)
type Assign(startloc : Location) = 
    inherit Token("=", startloc)

(* 'c' *)
type CharConst(str, startloc : Location) = 
    inherit Token(str, startloc)

(* A single character terminal symbol *)
type Terminal(str, startloc : Location) = 
    inherit Token(str, startloc)

(* dd* *)
type NumOp(str, startloc : Location) = 
    inherit Token(str, startloc)

(* Relation operators >= > < <= != *)
type RelOp(str, startloc : Location) = 
    inherit Token(str, startloc)

(* Logical operators && || *)
type LogOp(str, startloc : Location) = 
    inherit Token(str, startloc)

let keywords = Set.ofList ["main"; "int"; "char"; "void"; "if"; 
    "else"; "while"; "return"; "System"; "in"; "out"; "class"; "new"]