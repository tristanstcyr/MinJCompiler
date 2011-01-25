/// Main entry point of the program is in here.
/// Also includes some helper functions specific to
/// output for the standalone lexical analyzer.
module Main

open System
open System.IO
open System.Diagnostics

open Scanner
open MinJ

/// Opens a file into a sequence of chars.
/// This is where some of the magic of loading characters on demand happens.
let ToCharSeq path = seq {
    use reader = new StreamReader(File.OpenRead(path))
    while not reader.EndOfStream do
        yield char(reader.Read())
} 

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

/// Writes the listing to a file.
let WriteListing sourcePath listingPath tokens = 

    /// Function that filters out tokens not on a line number.
    /// Not very efficient, but this is just for presentation anyway.
    let GetTokensOnLine lineNum = 
        Seq.filter (fun (t : Token) -> t.StartLocation.Row = lineNum) tokens
    
    File.Delete(listingPath)
    
    (* Open the source file and the listing file *)
    use sourceReader = new StreamReader(File.OpenRead(sourcePath))
    use listingWriter = new StreamWriter(File.OpenWrite(listingPath))
    
    /// Prints the listing, line by line.
    let rec PrintListing lineNum = 
        (* As we read lines of text from the source,
           we grab tokens that correspond to that line
           and print them under the line. This is exactly
           as it is done in the book *)
        if not sourceReader.EndOfStream then
            (* Print the line of source *)
            listingWriter.WriteLine(sprintf "%i: %s" lineNum (sourceReader.ReadLine()))
            let foundError = ref false
            for token in GetTokensOnLine lineNum do
                match token with :? Error -> foundError := true | _ -> ()
                listingWriter.WriteLine(sprintf "\t%i: %s\tname = %s" 
                    <|lineNum <| token.GetType().Name <| token.ToString())
            (* Only continue if an error is not found *)
            if not !foundError then
                PrintListing <| lineNum + 1

    PrintListing 1

/// Entry point with command line params parsed
let Run inputPath attributePath listingPath =
    (* To our stuff *)
    let sw = new Stopwatch()
    let tokens = inputPath |> ToCharSeq |> Tokenize |> Seq.toList
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