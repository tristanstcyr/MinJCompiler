module MinJ.Tests.Utils

open TestFramework
open Compiler
open MinJ
open System.IO

let makeParser str = 
    let memStream = new StreamWriter(new MemoryStream())
    let parser = new MinJ.Parser.Parser(Scanner.scan str <| NullListingWriter(), memStream, RuleLogger(memStream))
    parser

let parseWithErrorCount str expectedErrorCount = 
    let memStream = new StreamWriter(new MemoryStream())
    let parser = new MinJ.Parser.Parser(Scanner.scan str <| NullListingWriter(), memStream, RuleLogger(memStream))
    let errors = 
        try
            let ast = parser.Parse()
            []
        with
            | CompilerException(errors) -> errors
    if Seq.length errors <> expectedErrorCount then
        Fail (sprintf "Expected %d errors but got %d" expectedErrorCount (List.length errors))