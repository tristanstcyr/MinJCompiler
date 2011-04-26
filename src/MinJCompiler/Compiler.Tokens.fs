namespace Compiler

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

/// ll*
type Identifier(str, startloc : Location) = 
    inherit Token(startloc)

    override this.ToString() = str

    member this.Value with get() = str