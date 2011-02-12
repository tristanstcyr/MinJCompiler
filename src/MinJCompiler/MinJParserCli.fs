module MinJ.ParserCli
open Parser
open System

open Scanner

let rec printRule depth rule =
    match rule with
        | Some(Rule(str, subrules)) ->
            if depth > 0 then for i in [1..depth] do printf "  "
            printfn "%s" str
            for subrule in subrules do
                printRule (depth + 1) subrule 
        | None -> ()
(*
"class Example 
{ 
    int[] field = new int[10];
    void main() 
    { 
        int y;
        while (y == 0)
        {
            if (y != 1)
            {
                foo(y);
                return 3;
            }
            else
            {
                if (i == 'x')
                {
                    i = System.in.int();
                    System.out(i);
                }
                else
                {
                    z = 'é';
                    y = 4 + 5 / 5 % 4 + 3 - 223428;
                    x = a[3 * 4 + 5];
                }
            }
        }
    } 

    int foo(int x)
    {
        System.out(x);
    }

}" |> tokenize |> parse |> Some |> printRule 0
*)

open System.Diagnostics

/// Entry point with command line params parsed
let Run inputPath listingPath =
    (* To our stuff *)
    let sw = new Stopwatch()
    let tokens = inputPath |> ToCharSeq |> tokenize
    let root = inputPath |> ToCharSeq |> tokenize |> parse
    sw.Stop()

    (* Print a helpful message *)
    printfn "%d tokens parsed in %f seconds" 
        <| Seq.length tokens
        <| float(sw.ElapsedMilliseconds) / float(1000)

    printfn "\nThe following rules were used to parse the input:\n"

    printRule 0 <| Some(root)

    (* Output our listing and attributes file *)
    WriteListing inputPath listingPath tokens

/// Main entry point of the application
let Main() =
    printfn "Welcome to the MinJ Parser"
    
    let cmds = Environment.GetCommandLineArgs()
    if cmds.Length <> 3 then
        printfn "usage: <input-file-path> <listing-file-path>"
    else
        Run <| Environment.GetCommandLineArgs().[1] 
            <| Environment.GetCommandLineArgs().[2]
Main()

Console.Read() |> ignore