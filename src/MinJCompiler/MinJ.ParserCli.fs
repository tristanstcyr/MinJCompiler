module MinJ.ParserCli

open Compiler
open Compiler.Tac
open Compiler.Tac.ToMoon
open Moon
open MinJ
open MinJ.ToTac
open MinJ.Scanner

open System
open System.IO
open System.Diagnostics

let openOutputFiles (directory : DirectoryInfo) =
    let dirName = directory.FullName
    (new StreamWriter(File.OpenWrite(dirName + @"\rules.txt")),
     new StreamWriter(File.OpenWrite(dirName + @"\listing.txt")),
     new StreamWriter(File.OpenWrite(dirName + @"\tac.txt")),
     new StreamWriter(File.OpenWrite(dirName + @"\moon.m")))

let openOutputStream (directory : DirectoryInfo) filename =
    let path = directory.FullName + @"\" + filename
    if File.Exists path then File.Delete path
    new StreamWriter(File.OpenWrite(path))

let time (output : TextWriter) what func =
    let sw = new Stopwatch()    
    sw.Start() 
    try 
        func()
    finally
        output.WriteLine(sprintf "%s in %.3f" what (float(sw.ElapsedMilliseconds) / float(1000)))

let countTokensThen onDone tokens = seq {
    let count = ref 0
    for token in tokens do
        count := !count + 1
        yield token
    onDone !count
}
    

/// Entry point with command line params parsed
let Run inputPath =

    (* Output our listing and attributes file *)
    let file = FileInfo(inputPath)

    if not file.Exists then
        printfn "Input file not found"
    else
        // Create output files paths
        let openOutputStream = openOutputStream file.Directory
        use rulesOutput = openOutputStream @"rules.txt"
        use listingOutput = openOutputStream @"listing.txt"
        use tacOutput = openOutputStream @"tac.txt"
        use moonOutput = openOutputStream @"moon.m"

        let listingWriter = ListingWriter(listingOutput)
        let ruleLogger = RuleLogger(rulesOutput)
    
        let time what func a = time listingOutput what (fun() -> func a)
        let printTokenCount count =
             listingOutput.WriteLine(sprintf "%i tokens" count)

        // Start a timer and do the work 
        let sw = new Stopwatch()
        sw.Start()
        try
            (openAsCharSeq inputPath) 
                |> tokenize listingWriter
                |> countTokensThen printTokenCount
                |> time "Lexing and parsing " (Parser.parse rulesOutput ruleLogger)
                |> time "Semantic analysis" Semantics.verify
                |> time "Intermediate code generation" Program.ToTac
                |> Program.ToStream tacOutput
                |> Program.ToMoon
                |> time "Target code generation" (Moon.ToStream.write moonOutput) 
            sw.Stop()
            
        with
            // A CompilerException might occure at any stage of the compilation process.
            // The CompileException type is composed of multiple exceptions that have been
            // accumulated throughout the compilation process.
            | CompilerException(errors) as e ->
                let location(m, l) = l
                sw.Stop()
                // Print the errors
                let sortedyLocation = List.sortBy location errors
                listingOutput.WriteLine()
                for message, location in sortedyLocation do
                    listingOutput.WriteLine(sprintf "%i:%i\t%s" location.Row location.Col message)
            | CompilerInternalException(message) as e ->
                listingOutput.WriteLine(sprintf "An internal error occured:%s" e.Message)
                listingOutput.WriteLine(e.StackTrace)
        
        // Print a message a the end with the amount of time taken.
        listingOutput.WriteLine(sprintf "Concluded in %.3f seconds\n" <| float(sw.ElapsedMilliseconds) / float(1000))

/// Main entry point of the application
let Main() =
    let cmds = Environment.GetCommandLineArgs()
    if cmds.Length <> 2 then
        printfn "usage: <input-file>"
    else
        Run <| Environment.GetCommandLineArgs().[1]
Main()