/// Defines all tokens for the MinJ language
[<AutoOpen>]
module MinJ.Tokens

open Compiler
open System
open System.Text.RegularExpressions

module Errors =
    let NumberConstantOverflow = 
        sprintf <| "Number constants must be between %d and %d." 
                <| Int32.MinValue
                <| Int32.MaxValue
    let OnlyASCIIChararacters = 
        "Only ASCII characters can be used as character constants."
    let InvalidCharacterFormat s =
        sprintf "Invalid character format %s." s

/// [0-9]*
type Number(value : int32, startloc : Location) =
    inherit Token(startloc)
    member this.Value with get() = value
    /// Creates a CharConst or an error token if the number overflows
    static member Create (s : string) l =
        try Number(int32(s), l) :> Token
        with | :? OverflowException -> 
            Error(Errors.NumberConstantOverflow, l) :> Token
    override this.ToString() = value.ToString()

/// 'c'
type CharConst(value : char, startloc : Location) = 
    inherit Token(startloc)
    member this.Value with get() = value
    /// Creates a CharConst or an error token if the character
    /// form is invalid.
    static member Create (s : string) l =
        let noQuotes = s.Substring(1, s.Length - 2)
        try 
            let c = Char.Parse(Regex.Unescape(noQuotes))
            if (int32)c > 255 then 
                Error(Errors.OnlyASCIIChararacters, l) :> Token
            else
                CharConst(c, l) :> Token
        with | :? FormatException ->
            Error(Errors.InvalidCharacterFormat s, l) :> Token

    override this.ToString() = sprintf "\'%c\'" value

/// An enumeration of all the possible types for a terminal.
type TerminalType =
    | Main | IntT | CharT | VoidT | If | Else | While | Return | System | In | Out | Class | New
    | Or | And | Assign | Equal | Greater | Lesser | GreaterEqual | LesserEqual | Not | NotEqual 
    | Add | Sub | Div | Mul | Mod | OParen | CParen | SemiCol | Comma | OSquare | CSquare
    | OCurly | CCurly | Period
    with 
        override this.ToString() =
            match this with
                | Main -> "main"
                | IntT -> "int"
                | CharT -> "char"
                | VoidT -> "void"
                | If -> "if"
                | Else -> "else"
                | While -> "while"
                | Return -> "return"
                | System -> "System"
                | In -> "in"
                | Out -> "out"
                | Class -> "class"
                | New -> "new"
            
                | Not -> "!"
                | Or -> "||"
                | And -> "&&"
            
                | Assign -> "="

                | Equal -> "=="
                | Greater -> ">"
                | Lesser -> "<"
                | GreaterEqual -> ">="
                | LesserEqual -> "<="
                | NotEqual -> "!="

                | Add -> "+"
                | Sub -> "-"
                | Mul -> "*"
                | Div -> "/"
                | Mod -> "%"

                | OParen -> "("
                | CParen  -> ")"
                | SemiCol -> ";"
                | Comma -> ","
                | OSquare -> "["
                | CSquare -> "]"
                | OCurly -> "{"
                | CCurly -> "}"
                | Period -> "."

/// A single character terminal symbol
type Terminal(ttype : TerminalType, startloc : Location) = 
    inherit Token(startloc)
    member this.Type with get() = ttype
    override this.ToString() = ttype.ToString()

let terminalMap = 
    let allTokens = [Main; IntT; CharT; VoidT; If; Else; While; Return; System; In; Out; Class; New
       ; Or; And; Assign; Equal; Greater; Lesser; GreaterEqual; LesserEqual; Not; NotEqual 
       ; Add; Sub; Div; Mul; Mod; OParen; CParen; SemiCol; Comma; OSquare; CSquare
       ; OCurly; CCurly; Period]
    List.fold (fun map t -> Map.add <| t.ToString() <| t <| map) Map.empty allTokens

let createTerminal str location = 
    try
        Terminal(terminalMap.[str], location) :> Token
    with | e -> 
        raise e

/// Returns an identifier or keyword token depending
/// if it matches with what is in the keyword set
let CreateIdentifierOrToken t l =
    match Map.tryFind t terminalMap with
        | Some(token) -> Terminal(token, l) :> Token
        | _ -> Identifier(t, l) :> Token

(* Some active patterns for matching tokens *)
let (|Terminal|_|) tt (token : Token) =
    match token with
        | :? Terminal as t when t.Type = tt -> Some(Terminal)
        | _ -> None

let (|CharConst|_|) (token : Token) =
    match token with
        | :? CharConst as cc -> Some(cc)
        | _ -> None
let (|Number|_|) (token : Token) =
    if token :? Number then
        Some(token :?> Number)
    else
        None
let (|Identifier|_|) (token : Token) = 
    if token :? Identifier then
        Some(token :?> Identifier)
    else
        None

/// Matches any of a list of terminal types
let (|AnyTerminalOf|_|) strs token =
    let rec matchTerminal strs =
        if List.isEmpty strs then 
            None
        else
            let str = List.head strs
            match token with
                | Terminal str -> Some(AnyTerminalOf)
                | _ -> matchTerminal <| List.tail strs
    matchTerminal strs