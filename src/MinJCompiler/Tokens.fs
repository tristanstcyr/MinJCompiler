module Tokens

open System

(* Defines a location in a file *)
type Location = {
    Row: int; (* aka line number *)
    Col: int;
}

let OriginLocation = {Row=1;Col=1;}
let AdvanceRow l = {Row = l.Row + 1; Col = 1}
let AdvanceCol l = {l with Col = l.Col + 1}

(* Lets define some base token types that are useful for any language *)

(* This is the base of all tokens *)
type Token(str : string, startloc : Location) = 
    member this.StartLocation with get() = startloc
    override this.ToString() = str

(* Represents an error during the lexing phase *)
type Error(str, startloc : Location) =
    inherit Token(str, startloc)

type Keyword(str, startloc : Location) = 
    inherit Token(str, startloc)