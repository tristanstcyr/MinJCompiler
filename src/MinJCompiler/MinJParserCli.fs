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

    let listingWriter = ListingWriter(listingOutput)
    let scanner = createMinJScanner (ToCharSeq inputPath) listingWriter
    let parser = Parser(scanner, rulesOutput, RuleLogger(rulesOutput))
    
    (* Start a timer and do the work *)
    let sw = new Stopwatch()
    sw.Start()
    let root, errors = parser.Parse()
    sw.Stop()

    // Output the errors if any
    if errors.Length = 0 then
        listingOutput.WriteLine("No errors encountered.")
    else
        ErrorPrinter.print errors listingOutput

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