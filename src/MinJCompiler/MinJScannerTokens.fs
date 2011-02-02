[<AutoOpen>]
/// Defines all tokens for the MinJ language
module MinJ.Tokens

open Scanner
open System

/// ll*
type Identifier(str, startloc : Location) = inherit Token(str, startloc)
/// dd*
type NumOp(str, startloc : Location) = inherit Token(str, startloc)
/// Relation operators >= > < <= !=
type RelOp(str, startloc : Location) = inherit Token(str, startloc)
/// Logical operators && ||
type LogOp(str, startloc : Location) = inherit Token(str, startloc)

/// [0-9]
type Number(value : Int64, startloc : Location) =
    inherit Token(value.ToString(), startloc)
    member this.Value with get() = value
    /// Creates a CharConst or an error token if the number overflows
    static member Create (s : string) l =
        try Number(int64(s), l) :> Token
        with | :? OverflowException -> 
            Error(sprintf "Number constants cannot exceed %d" Int64.MaxValue, l) :> Token
/// =
type Assign(startloc : Location) = inherit Token("=", startloc)
/// 'c'
type CharConst(value : char, startloc : Location) = 
    inherit Token(sprintf "\'%c\'" value, startloc)
    member this.Value with get() = value
    /// Creates a CharConst or an error token if the character
    /// form is invalid.
    static member Create (s : string) l =
        let noQuotes = s.Substring(1, s.Length - 2)
        try CharConst(char(noQuotes), l) :> Token
        with | :? FormatException ->
            Error("Invalid character format " + noQuotes, l) :> Token

/// All keywords in the language in a set for fast lookup
let keywords = Set.ofList ["main"; "int"; "char"; "void"; "if"; 
    "else"; "while"; "return"; "System"; "in"; "out"; "class"; "new"]

/// Returns an identifier or keyword token depending
/// if it matches with what is in the keyword set
let CreateIdentifierOrToken t l =
    if keywords.Contains (t) then
        Keyword(t, l) :> Token
    else
        Identifier(t, l) :> Token