namespace Scanner

open Scanner

/// Type for printing the listing of a program
/// this is mostly used by the scanner 
type IListingWriter = 
    
    abstract member AdvanceLine : unit -> unit  
    /// Adds a character on a line
    abstract member AddChar : char -> unit

    /// Adds a token that was found on the current line
    abstract member AddToken : Token -> unit

    /// Forces the current line to be writter
    /// This needs to be called in case line information
    /// might still be in memory
    abstract member End : unit -> unit