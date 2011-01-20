module Main
open System
open ScannerTests
open MinJScanner
open Tokens
open System.IO

/// Opens a file into a sequence of chars
let ToCharSeq path = seq {
    use reader = new StreamReader(File.OpenRead(path))
    while not reader.EndOfStream do
        yield char(reader.Read())
} 

(* *)
let GetTokenAttributesStr (token : Token) =
        let ListTokenStr row col name strValue = sprintf "%s,%-3s %-15s %s" row col name strValue
        ListTokenStr (token.StartLocation.Row.ToString()) (token.StartLocation.Col.ToString()) (token.GetType().Name) (token.ToString())

/// Writes the tokens and their attributes to a file
let WriteAttributes path tokens = 
    File.Delete(path)
    use writer = new StreamWriter(File.OpenWrite(path))
    writer.WriteLine(sprintf "%-5s %-15s %s" "Loc" "Type" "StrRep")
    for token in tokens do
        writer.WriteLine(GetTokenAttributesStr token)

/// Writes the listing to a file
let WriteListing sourcePath listingPath tokens =
    use sourceReader = new StreamReader(File.OpenRead(sourcePath))
    use listingWriter = new StreamWriter(File.OpenWrite(listingPath))
    let rec PrintListing lineNum =
        if not sourceReader.EndOfStream then
            listingWriter.WriteLine(sprintf "%i:\t%s" lineNum (sourceReader.ReadLine()))
            for token in Seq.filter (fun (t : Token) -> t.StartLocation.Row = lineNum) tokens do
                listingWriter.WriteLine(sprintf "\t%i: %s\tname = %s" lineNum (token.GetType().Name) (token.ToString()))
            PrintListing <| lineNum + 1
    PrintListing 0

/// Entry point with command line params parsed
let Run inputPath attributePath listingPath =
    let tokens = inputPath |> ToCharSeq |> Tokenize |> Seq.cache 
    WriteListing inputPath listingPath tokens
    WriteAttributes attributePath tokens

/// Main entry point of the application
let Main() =
    printfn "Welcome to the Minj Lexer"
    
    let cmds = Environment.GetCommandLineArgs()
    if cmds.Length <> 4 then
        printfn "usage: MinJCompiler <input-file-path> <attribute-file-path> <listing-file-path>"
    else
        Run (Environment.GetCommandLineArgs().[1]) 
            (Environment.GetCommandLineArgs().[2]) 
            (Environment.GetCommandLineArgs().[3])
//Main()
RunAllTests()
