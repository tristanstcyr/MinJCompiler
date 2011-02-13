namespace Scanner

open Tokens
open System.IO
open System.Text

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