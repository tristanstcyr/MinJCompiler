module Main
open System
open ScannerTests
open MinJScanner
open Tokens

let ListToken (token : Token) =
        let ListTokenStr row col name strValue = sprintf "%s,%-3s %-15s %s" row col name strValue
        ListTokenStr (token.StartLocation.Row.ToString()) (token.StartLocation.Col.ToString()) (token.GetType().Name) (token.ToString())
    
let Listing (str : string) (token : Token) = str + (ListToken token) + "\n"

let Main() =
    printfn "Welcome to the Minj token Scanner"
    printfn "Please enter the code to tokenize in the console. The text will be tokenized when 3 new line characters will be detected"

    let rec CaptureInput newLineCount = seq {
            let c = char (Console.Read())
            match c with 
                | '\r' -> yield! CaptureInput newLineCount
                | '\n' -> 
                    if newLineCount < 2 then 
                        yield c
                        yield! CaptureInput (newLineCount + 1)
                | _ -> 
                    yield c
                    yield! CaptureInput 0
    }

    
    let tokens = CaptureInput 0 |> Seq.toList |> Tokenize |> Seq.toList
    let result = Seq.fold Listing "" tokens
    printfn "Found %d token(s)" (List.length tokens)
    printfn "Here's a listing of the tokens"
    printfn "%-5s %-15s %s" "Loc" "Type" "Value"
    printfn "%s" result


//Main()
RunAllTests()

ignore(Console.Read())
