module MinJ.ParserCli

open MinJ
open MinJ.ToTac
open Compiler
open Compiler.Tac
open Compiler.Tac.ToMoon
open Moon

open System
open System.IO
open System.Diagnostics

/// Entry point with command line params parsed
let Run inputPath =

    (* Output our listing and attributes file *)
    let file = FileInfo(inputPath)

    if not file.Exists then
        printfn "Input file not found"
    else
        // Create output files paths. These are in the same 
        // directory as the the input file.
        let rulesPath = file.Directory.FullName + @"\rules.txt"
        let listingPath = file.Directory.FullName + @"\listing.txt"
        let tacPath = file.Directory.FullName + @"\tac.txt"
        let moonPath = file.Directory.FullName + @"\moon.m"
    
        // Delete the files if they already exist
        for filePath in [rulesPath;listingPath;tacPath;moonPath] do
            if File.Exists filePath then
                File.Delete filePath
    
        // Open streams for writing to the output files.
        use listingOutput = new StreamWriter(File.OpenWrite(listingPath))
        use rulesOutput = new StreamWriter(File.OpenWrite(rulesPath))
        use tacOutput = new StreamWriter(File.OpenWrite(tacPath))
        use moonOutput = new StreamWriter(File.OpenWrite(moonPath))

        let listingWriter = ListingWriter(listingOutput)
        let ruleLogger = RuleLogger(rulesOutput)
    
        (* Start a timer and do the work *)
        let sw = new Stopwatch()
        sw.Start()
        try
            // Translate to Three Address Code
            let tacPrg = 
                openAsCharSeq inputPath 
                |> Parser.parse listingWriter rulesOutput ruleLogger
                |> Semantics.verify 
                |> Program.ToTac

            // Output the TAC to a file
            Program.ToStream tacOutput tacPrg
            // Translate the TAC to Moon and output to file
            Program.ToMoon tacPrg |> Moon.ToStream.write moonOutput
            sw.Stop()
        with
            // A CompilerException might occure at any stage of the compilation process.
            // The CompileException type is composed of multiple exceptions that have been
            // accumulated throughout the compilation process.
            | CompilerException(errors) as e ->
                sw.Stop()
                // Print the errors
                let sortedyLocation = Seq.sortBy (fun (m, l) -> l) errors
                listingOutput.WriteLine()
                for message, location in sortedyLocation do
                    listingOutput.WriteLine(sprintf "%i:%i\t%s" location.Row location.Col message)
        
        // Print a message a the end with the amount of time taken.
        listingOutput.WriteLine(sprintf "Concluded in %.3f seconds\n" <| float(sw.ElapsedMilliseconds) / float(1000))

/// Main entry point of the application
let Main() =
    let cmds = Environment.GetCommandLineArgs()
    if cmds.Length <> 2 then
        printfn "usage: <assembly-file>"
    else
        Run <| Environment.GetCommandLineArgs().[1]
Main()