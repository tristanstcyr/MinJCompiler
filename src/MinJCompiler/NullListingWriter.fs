namespace Scanner

/// A listing writer that does nothing.
/// This is used to disable outputting of the listing
/// without altering the scanner code.
type NullListingWriter() = 
    interface IListingWriter with
        member this.AdvanceLine() = ()
        member this.AddChar(c : char) = ()
        member this.AddToken(token : Token) = ()
        member this.End() = ()