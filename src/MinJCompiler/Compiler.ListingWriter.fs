namespace Compiler

open System.IO
open System.Text

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

/// A listing writer that does nothing.
/// This is used to disable outputting of the listing
/// without altering the scanner code.
type NullListingWriter() = 
    interface IListingWriter with
        member this.AdvanceLine() = ()
        member this.AddChar(c : char) = ()
        member this.AddToken(token : Token) = ()
        member this.End() = ()

/// Type for printing the listing of a program
/// this is mostly used by the scanner 
type ListingWriter(writer : TextWriter) = 
    
    /// Stores the current line from source
    let mutable line = StringBuilder()
    /// Stores the tokens found on the current line
    let mutable tokens : Token list = []
    /// Stores count of the line number
    let mutable lineNumber = 0

    /// Writes all of the line information
    member private this.WriteLine() =
        writer.Write(lineNumber)
        writer.Write(": ")
        writer.Write(line.ToString())
        writer.WriteLine()
        for token in List.rev tokens do
            writer.WriteLine(sprintf "\t%A" token)
            writer.WriteLine(sprintf "\t\tType=%s" <| token.GetType().Name)
            writer.WriteLine(sprintf "\t\tLoc=%d, %d" token.StartLocation.Row token.StartLocation.Col)
        writer.WriteLine()

    interface IListingWriter with
        member this.AdvanceLine() = 
            this.WriteLine()
            tokens <- []
            lineNumber <- lineNumber + 1
            line.Clear() |> ignore
        member this.AddChar(c : char) = line.Append(c) |> ignore
        member this.AddToken(token : Token) =
            tokens <- token :: tokens
        member this.End() =
            this.WriteLine()
            writer.WriteLine()