module ScannerTests
open Tokens
open MinJTokens
open MinJScanner
open System
open System.Diagnostics
open TestFramework

let Consume tokens = for token in tokens do ()

let Compare (result : Token) (expected : Token) = 
    if not (result.ToString() = expected.ToString()) then
        let message = sprintf "Tokens were different: got \"%s\" expected \"%s\"" (result.ToString()) (expected.ToString())
        raise <| AssertionException(message)
    elif not (result.GetType() = expected.GetType()) then
        let message = sprintf "Tokens were of different type: got \"%s\" expected \"%s\"" (result.GetType().Name) (expected.GetType().Name)
        raise <| AssertionException(message)

let CheckTokenType input expected =
    let tokens = Tokenize input
    let zip = Seq.zip tokens expected
    for (result, expected) in zip do
        Assert "Token types were different" (result.GetType() = expected)

let CheckTokens  input (expected : Token list) = 
    let tokens = Tokenize input |> Seq.toList
    Assert "Insufficient tokens extracted" (tokens.Length = Seq.length expected)
    for result, expected in Seq.zip tokens expected do
        Compare result expected

(* Some mock location *)
let l = OriginLocation

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

type ScannerTests() =
    
    static member TestIdentifierIsTokenizedWithSpaces = CheckTokens " hello " [Identifier("hello", l)]

    static member TestIdentifierIsTokenizedNoSpaces = CheckTokens " hello " [Identifier("hello", l)]
    static member TestTwoIdentifiersAreTokenized = CheckTokens " he llo " [Identifier("he", l); Identifier("llo", l)]
    
    static member TestDivision = CheckTokens " / " [NumOp ("/", l)]
    static member TestPlus = CheckTokens " + " [NumOp ("+", l)]
    static member TestMinus = CheckTokens " - " [NumOp ("-", l)]
    static member TestMod = CheckTokens " % " [NumOp ("%", l)]
   
    static member TestCommentNewLine = CheckTokens " // world\n hello" [Identifier ("hello", l)]
    
    static member TestCommentEof = CheckTokens " // end of the file!" []
    
    static member TestLogicalOr = CheckTokens "||" [LogOp ("||", l)]
    static member TestLogicalAnd = CheckTokens "&&" [LogOp ("&&", l)]
    
    static member TestGreaterThan = CheckTokens ">" [RelOp (">", l)]
    static member TestGreaterEqualThan = CheckTokens ">=" [RelOp (">=", l)]
    static member TestLessThan = CheckTokens "<" [RelOp ("<", l)]
    static member TestEqualLessThan = CheckTokens "<=" [RelOp ("<=", l)]
    
    static member TestAssign = CheckTokens "=" [Assign(l)]
    
    static member TestEquals = CheckTokens "==" [RelOp ("==", l)]
    
    static member TestSingleDigitNumber = CheckTokens "3" [Number(3L, l)]

    static member TestMultiDigitNumber = CheckTokens "3345Hello" [Number(3345L, l); Identifier("Hello", l)]
    
    static member TestConstantChar = CheckTokens "'c'=='b'" [CharConst('c', l);RelOp("==", l);CharConst('b', l)]
    
    static member TestUnicodeCharConst = CheckTokens "'Ń'" [CharConst('Ń', l)]

    static member TestInvalidCharConst = CheckTokens "'askdjh'" [Error("Invalid character format askdjh", l)]

    static member TestTooLargeNumber = 
        CheckTokens "92473246238746238746287364872346283468273462834682364782648" 
            [Error("Number constants cannot exceed " + string(Int64.MaxValue), l)]

    static member TestKeywords = 
        let keywordsString  = String.Join(" ", keywords)
        CheckTokens keywordsString (List.map (fun str -> Keyword(str, l) :> Token) (Set.toList (keywords)))

    static member TestIncompleteToken =
        CheckTokenType "&" [typeof<Error>]
        CheckTokenType "|" [typeof<Error>]

    static member TestNoToken = CheckTokens "" []
  
    static member TestSingleLetterIdentifier = CheckTokens "a" [Identifier("a", l)]
    
    static member TestCommentEnd = CheckTokens "//\nIdentifier" [Identifier("Identifier", l)]
 
    static member TestLargeText =
        let tokens = Tokenize largeText
        for t in tokens do
            match t with
                | :? Error ->  Fail <| "Found error token " + t.ToString()
                | _ -> ()

    static member PerfTest =
        let input = ref largeText
        for i in [1..10] do
            input := !input + !input
        let sw = Stopwatch()
        sw.Start()
        let tokens = (Tokenize !input) |> Seq.toList
        sw.Stop()
        printfn "%i tokens scanned in %i ms" tokens.Length sw.Elapsed.Milliseconds
        let seconds = float32(sw.Elapsed.Milliseconds) / 1000.0f
        printfn "or %f chars / second" (float32((!input).Length) / seconds)
        printfn "or %f tokens / second" (float32(tokens.Length) / seconds)
   
let RunAllTests() = 
    RunAllTests typeof<ScannerTests>
    Console.WriteLine("\nAll tests have been run")
    Console.Read()