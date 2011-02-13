[<AutoOpen>]
module Scanner.Tokens

/// Defines a location in a file
type Location = {
    /// The line number
    Row: int;
    Col: int;
}

(* Some helpers for Location *)
/// The beginning of a file
let OriginLocation = {Row=1;Col=1;}
let AdvanceRow l = {Row = l.Row + 1; Col = 1}
let AdvanceCol l = {l with Col = l.Col + 1}

(* Lets define some base token types that are useful for any language *)
/// Base of all tokens
type Token(startloc : Location) = 
    member this.StartLocation with get() = startloc

/// Indicates the end of a token stream
type End(loc : Location) = inherit Token(loc)

/// Represents an error during the lexing phase
type Error(message, startloc : Location) =
    inherit Token(startloc)
    override this.ToString() = message