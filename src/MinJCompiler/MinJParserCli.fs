module MinJ.ParserCli
open Parser
open Scanner

open Compiler
open MinJ.Ast
open MinJ.Ast.ToTac
open MinJ.Ast.TypeCheck
open Tac.Printing

open System
open System.IO
open System.Diagnostics

/// Entry point with command line params parsed
let Run inputPath =

    (* Output our listing and attributes file *)
    let file = FileInfo(inputPath)
    let rulesPath = file.Directory.FullName + @"\rules.txt"
    let listingPath = file.Directory.FullName + @"\listing.txt"
    let assemblyPath = file.Directory.FullName + @"\assembly.txt"

    List.iter File.Delete [rulesPath;listingPath;assemblyPath]
    
    use listingOutput = new StreamWriter(File.OpenWrite(listingPath))
    use rulesOutput = new StreamWriter(File.OpenWrite(rulesPath))
    use assemblyOutput = new StreamWriter(File.OpenWrite(assemblyPath))

    let listingWriter = ListingWriter(listingOutput)
    let scanner = createMinJScanner (ToCharSeq inputPath) listingWriter
    let parser = Parser(scanner, rulesOutput, RuleLogger(rulesOutput))
    
    let sw = new Stopwatch()
    (* Start a timer and do the work *)
    try
        sw.Start()
        let prg = parser.Parse()
        let typeChecker = TypeChecker()
        typeCheck prg
        assemblyOutput.PrintProgram <| prg.ToTac()
        sw.Stop()
    with
        | CompilerException(errors) as e ->
            sw.Stop()
            for message, location in Seq.sortBy (fun (m, l) -> l) errors do
                printfn "%i:%i\t%s" location.Row location.Col message
        
    listingOutput.WriteLine(sprintf "Concluded in %.3f seconds\n" <| float(sw.ElapsedMilliseconds) / float(1000))

/// Main entry point of the application
let Main() =
    let cmds = Environment.GetCommandLineArgs()
    if cmds.Length <> 2 then
        printfn "usage: <assembly-file>"
    else
        Run <| Environment.GetCommandLineArgs().[1]
Main()