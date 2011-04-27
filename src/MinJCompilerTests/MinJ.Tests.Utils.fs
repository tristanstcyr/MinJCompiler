module MinJ.Tests.Utils

open TestFramework
open Compiler
open MinJ
open System.IO
open System.Diagnostics
open System.Collections.Generic

let makeParser str = 
    let memStream = new StreamWriter(new MemoryStream())
    let parser = new MinJ.Parser.Parser(Scanner.tokenize (NullListingWriter()) str, memStream, RuleLogger(memStream))
    parser

let parseWithErrorCount str expectedErrorCount = 
    let memStream = new StreamWriter(new MemoryStream())
    let parser = new MinJ.Parser.Parser(Scanner.tokenize (NullListingWriter()) str, memStream, RuleLogger(memStream))
    let errors = 
        try
            let ast = parser.Parse()
            []
        with
            | CompilerException(errors) -> errors
    if Seq.length errors <> expectedErrorCount then
        Fail (sprintf "Expected %d errors but got %d" expectedErrorCount (List.length errors))

let time func =
    let sw = Stopwatch()
    sw.Start()
    func()
    sw.Stop()
    float32(sw.ElapsedMilliseconds)

let repeat n func = seq {
    for i in [1..n] do
        yield func()
}

let consume s = for _ in s do ()

let tokenize chars = Scanner.tokenize (NullListingWriter()) chars