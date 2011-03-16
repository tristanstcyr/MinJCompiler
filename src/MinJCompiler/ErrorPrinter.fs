module MinJ.ErrorPrinter

open System.IO
open Scanner

// Helper closure for printing errors
let print errors (writer : TextWriter) =
    for error in errors do
        let message, location = 
            match error with
                | UnexpectedToken(token) -> 
                    sprintf "Syntax Error: Encountered an unexpected token %s" <| token.ToString(), token.StartLocation
                | UnexpectedEnd ->  "Syntax error: Input ended too early", OriginLocation
                | TokenizationError(e) -> sprintf "Tokenization error: %s" <| e.ToString(), e.StartLocation
                | ParsingError(message, token) -> message, token.StartLocation
                | e -> e.Message, OriginLocation
        writer.WriteLine(sprintf "%d, %d: %s" location.Row location.Col message) 