module MinJ.ParserCli
open Parser
open Scanner

open System
open System.IO
open System.Diagnostics

/// Entry point with command line params parsed
let Run inputPath rulesPath listingPath =
    (* Output our listing and attributes file *)
    File.Delete(listingPath)
    use listingOutput = new StreamWriter(File.OpenWrite(listingPath))
    use rulesOutput = new StreamWriter(File.OpenWrite(rulesPath))

    (* Start a timer and do the work *)
    let sw = new Stopwatch()
    try
        let listingWriter = ListingWriter(listingOutput)
        let scanner = createMinJScanner (ToCharSeq inputPath) listingWriter
        let parser = Parser scanner
        let root = parser.ParsePrg()
        sw.Stop()
        rulesOutput.Write(root.Value)
        listingOutput.WriteLine("No errors encountered.");
    with 
        | UnexpectedToken(token) -> 
            listingOutput.WriteLine(sprintf "Syntax Error: Encountered an unexpected token %s at Line=%d, Column=%d" 
                <| token.ToString() <| token.StartLocation.Row <| token.StartLocation.Col);
        | UnexpectedEnd -> 
            listingOutput.WriteLine("Syntax error: Input ended too early")
        | TokenizationError(e) ->
            listingOutput.WriteLine(sprintf "Tokenization error: %s at Line=%d, Column=%d" 
                <| e.ToString()
                <| e.StartLocation.Row
                <| e.StartLocation.Col)         
    sw.Stop()

    listingOutput.WriteLine(sprintf "Concluded in %.3f seconds\n" <| float(sw.ElapsedMilliseconds) / float(1000))

/// Main entry point of the application
let Main() =
    let cmds = Environment.GetCommandLineArgs()
    if cmds.Length <> 4 then
        printfn "usage: <input-file-path> <rules-output-path> <listing-file-path>"
    else
        Run <| Environment.GetCommandLineArgs().[1] 
            <| Environment.GetCommandLineArgs().[2]
            <| Environment.GetCommandLineArgs().[3]
Main()