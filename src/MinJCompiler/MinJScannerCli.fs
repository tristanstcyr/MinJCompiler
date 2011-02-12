/// Main entry point of the program is in here.
/// Also includes some helper functions specific to
/// output for the standalone lexical analyzer.
module Main

open System
open System.IO
open System.Diagnostics

open Scanner
open MinJ

/// Writes the tokens and their attributes to a file as a table.
let WriteAttributes path (tokens : Token seq) = 
    File.Delete(path)
    (* Open the file *)
    use writer = new StreamWriter(File.OpenWrite(path))
    (* Write a header to the table *)
    writer.WriteLine(sprintf "%-5s %-15s %s" "Loc" "Type" "StrRep")
    (* Write the rows of the table *)
    for token in tokens do
        writer.WriteLine(sprintf "%s,%-3s %-15s %s"
            <| token.StartLocation.Row.ToString() 
            <| token.StartLocation.Col.ToString() 
            <| token.GetType().Name 
            <| token.ToString())
    writer.WriteLine(sprintf "\nGenerated %i tokens." <| Seq.length tokens)

/// Entry point with command line params parsed
let Run inputPath attributePath listingPath =
    (* To our stuff *)
    let sw = new Stopwatch()
    let tokens = inputPath |> ToCharSeq |> tokenize |> Seq.toList
    sw.Stop()

    (* Print a helpful message *)
    printfn "%i tokens found in %f seconds elapsed" 
        <| tokens.Length
        <| float(sw.ElapsedMilliseconds) / float(1000)
    
    (* Output our listing and attributes file *)
    WriteListing inputPath listingPath tokens
    WriteAttributes attributePath tokens

/// Main entry point of the application
let Main() =
    printfn "Welcome to the Minj Lexical Analyzer"
    
    let cmds = Environment.GetCommandLineArgs()
    if cmds.Length <> 4 then
        printfn "usage: MinJCompiler <input-file-path> <attribute-file-path> <listing-file-path>"
    else
        Run <| Environment.GetCommandLineArgs().[1] 
            <| Environment.GetCommandLineArgs().[2] 
            <| Environment.GetCommandLineArgs().[3]
Main()