module MinJTokens
open Tokens
open System

(* ll* *)
type Identifier(str, startloc : Location) =
    inherit Token(str, startloc)

(* [0-9]* *)
type Number(value : Int64, startloc : Location) =
    inherit Token(string(value), startloc)
let CreateNumber (s : string) l =
    try
        Number(int64(s), l) :> Token
    with | :? OverflowException -> 
        Error("Number constants cannot exceed " + string(Int64.MaxValue), l) :> Token

(* = *)
type Assign(startloc : Location) = 
    inherit Token("=", startloc)

(* 'c' *)
type CharConst(c : char, startloc : Location) = 
    inherit Token("'" + string(char) + "'", startloc)
let CreateCharConst (s : string) l =
    let noQuotes = s.Substring(1, s.Length - 2)
    try
        CharConst(char(noQuotes), l) :> Token
    with | :? FormatException ->
        Error("Invalid character format " + noQuotes, l) :> Token

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