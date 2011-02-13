/// All tests for the MinJ scanner
module ScannerTests

open Scanner
open MinJ
open System
open System.Diagnostics
open TestFramework
open System.IO
open System.Collections.Generic

/// Some mock location
let l = OriginLocation

/// Compares two tokens
let Compare (result : Token) (expected : Token) = 
    if not (result.ToString() = expected.ToString()) then
        let message = sprintf "Tokens were different: got \"%s\" expected \"%s\"" (result.ToString()) (expected.ToString())
        raise <| AssertionException(message)
    elif not (result.GetType() = expected.GetType()) then
        let message = sprintf "Tokens were of different type: got \"%s\" expected \"%s\"" (result.GetType().Name) (expected.GetType().Name)
        raise <| AssertionException(message)

let tokenize input =
    createMinJScanner input <| NullListingWriter()

/// Verifies only the types of the tokens generated
let CheckTokenType input expected =
    use tokens = tokenize input
    for expected in expected do
        if not <| tokens.MoveNext() then
            Fail "Less tokens than expected"
        Assert "Token types were different" (tokens.Current.GetType() = expected)
        

/// Verifies the types and values of generated tokens
let CheckTokens  input (expected : Token list) = 
    use tokens = tokenize input
    for expected in expected do
        if not <| tokens.MoveNext() then
            Fail "Less tokens than expected"
        Compare tokens.Current expected

let CheckTerminal input tt =
    CheckTokens input [Terminal(tt, l)]

let largeText = 
    "// example of a code in MinJ
    class Average
    }
    //a sample program. It calculates the average of several numbers,
    // the first value gives how many numbers are to be read
    void main() //the main method
    }
    int n,ave;
    n=In.int();
    if (n > 0)
    }
    int total = sum_int(n);
    ave =(total/ n);
    System.out( 'a','=',ave);
    {
    else System.out('e','r','r','o','r');
    {
    int sum_int(int m) // read and sum up m integers
    }
    int i=1;
    int[] val = new int[100];
    int res= 0;
    while (i <=m)
    }
    val[i] =System.in.int();
    res = res + val[i];
    i = i+1;
    {
    return res;
    {
    {"

/// Contains the tests for the MinJ scanner, see the documentation for details.
type ScannerTests() =
    
    static member TestIdentifierIsTokenizedWithSpaces = CheckTokens " hello " [Identifier("hello", l)]

    static member TestIdentifierIsTokenizedNoSpaces = CheckTokens "hello" [Identifier("hello", l)]
    static member TestTwoIdentifiersAreTokenized = CheckTokens " he llo " [Identifier("he", l); Identifier("llo", l)]
    
    static member TestSingleDigitNumber = CheckTokens "3" [Number(3L, l)]

    static member TestMultiDigitNumber = CheckTokens "3345Hello" [Number(3345L, l); Identifier("Hello", l)]
    
    static member TestConstantChar = CheckTokens "'c'=='b'" [CharConst('c', l);CreateIdentifierOrToken "==" l;CharConst('b', l)]
    
    static member TestUnicodeCharConst = CheckTokens "'Ń'" [CharConst('Ń', l)]

    static member TestInvalidCharConst = CheckTokens "'askdjh'" [Error("Invalid character format askdjh", l)]

    static member TestTooLargeNumber = 
        CheckTokens "92473246238746238746287364872346283468273462834682364782648" 
            [Error("Number constants cannot exceed " + string(Int64.MaxValue), l)]

    static member TestTerminals =
        let (input, tokens) = Map.fold (fun state strValue token -> 
                                    match state with 
                                        | (str, tokens) -> (strValue+" "+str, Terminal(token, l) :> Token :: tokens)) 
                                        ("", []) terminalMap
        CheckTokens input tokens

        //let keywordsString  = String.Join(" ", Set.toArray terminalMap)
        //CheckTokens keywordsString (List.map (fun str -> Terminal(str, l) :> Token) (Set.toList (keywords))) 

    static member TestIncompleteToken =
        CheckTokenType "&" [typeof<Error>]
        CheckTokenType "|" [typeof<Error>]

    static member TestNoToken = CheckTokens "" []
  
    static member TestSingleLetterIdentifier = CheckTokens "a" [Identifier("a", l)]
    
    static member TestCommentEnd = CheckTokens "//\nIdentifier" [Identifier("Identifier", l)]
 
    static member TestLargeText =
        let tokens = (tokenize largeText)
        while tokens.MoveNext() do
            match tokens.Current with
                | :? Error as e ->  Fail <| "Found error token " + e.ToString()
                | _ -> ()

    static member PerfTest =
        let input = ref largeText
        for i in [1..10] do
            input := !input + !input
        let sw = Stopwatch()
        sw.Start()
        let tokens = (tokenize !input)
        let count = ref 0
        while tokens.MoveNext() do
            count := !count + 1
        sw.Stop()
        printfn "%i tokens scanned in %i ms" !count sw.Elapsed.Milliseconds
        let seconds = float32(sw.Elapsed.Milliseconds) / 1000.0f
        printfn "or %f chars / second" (float32((!input).Length) / seconds)
        printfn "or %f tokens / second" (float32(!count) / seconds)

/// Runs all tests for the MinJ lexer
let RunAllTests() = 
    RunAllTests typeof<ScannerTests>
    Console.WriteLine("\nAll tests have been run")
    ignore(Console.Read())