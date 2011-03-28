module MinJ.ParserCli
open Parser
open Scanner

open Compiler
open MinJ.Ast
open MinJ.Ast.ToTac
open Tac.Printing

open System
open System.IO
open System.Diagnostics

/// Entry point with command line params parsed
let Run inputPath =

    (* Output our listing and attributes file *)
    let file = FileInfo(inputPath)

    // Create output files paths. These are in the same 
    // directory as the the input file.
    let rulesPath = file.Directory.FullName + @"\rules.txt"
    let listingPath = file.Directory.FullName + @"\listing.txt"
    let assemblyPath = file.Directory.FullName + @"\assembly.txt"
    
    // Delete the files if they already exist
    List.iter File.Delete [rulesPath;listingPath;assemblyPath]
    
    // Open streams for writing to the output files.
    use listingOutput = new StreamWriter(File.OpenWrite(listingPath))
    use rulesOutput = new StreamWriter(File.OpenWrite(rulesPath))
    use assemblyOutput = new StreamWriter(File.OpenWrite(assemblyPath))

    // Create the Scanner and parser. These are objects because they need to be stateful
    // for performance reasons.
    let scanner = createMinJScanner (ToCharSeq inputPath) (ListingWriter(listingOutput))
    let parser = Parser(scanner, rulesOutput, RuleLogger(rulesOutput))
    
    (* Start a timer and do the work *)
    let sw = new Stopwatch()
    try
        sw.Start()
        (parser.Parse() |> SemanticVerification.verify ).ToTac() |> assemblyOutput.PrintProgram
        sw.Stop()
    with
        // A CompilerException might occure at any stage of the compilation process.
        // The CompileException type is composed of multiple exceptions that have been
        // accumulated throughout the compilation process.
        | CompilerException(errors) as e ->
            sw.Stop()
            // Print the errors
            for message, location in Seq.sortBy (fun (m, l) -> l) errors do
                printfn "%i:%i\t%s" location.Row location.Col message
        
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