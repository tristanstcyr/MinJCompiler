/// All tests for the MinJ scanner
module MinJ.Scanner.Tests.Unit

open TestFramework
open Compiler
open MinJ
open MinJ.Tests

open System
open System.Diagnostics
open System.IO
open System.Collections.Generic

/// Some mock location
let l = Location.origin

/// Compares two tokens
let Compare (result : Token) (expected : Token) = 
    if not (result.ToString() = expected.ToString()) then
        let message = sprintf "Tokens were different: got \"%s\" expected \"%s\"" (result.ToString()) (expected.ToString())
        raise <| AssertionException(message)
    elif not (result.GetType() = expected.GetType()) then
        let message = sprintf "Tokens were of different type: got \"%s\" expected \"%s\"" (result.GetType().Name) (expected.GetType().Name)
        raise <| AssertionException(message)

let tokenize = Scanner.tokenize (NullListingWriter())

/// Verifies only the types of the tokens generated
let CheckTokenType input expected =
    use tokens = (tokenize input).GetEnumerator()
    for expected in expected do
        if not <| tokens.MoveNext() then
            Fail "Less tokens than expected"
        Assert "Token types were different" (tokens.Current.GetType() = expected)
        

/// Verifies the types and values of generated tokens
let CheckTokens  input (expected : Token list) = 
    use tokens = (tokenize input).GetEnumerator()
    for expected in expected do
        if not <| tokens.MoveNext() then
            Fail "Less tokens than expected"
        Compare tokens.Current expected

let CheckTerminal input tt =
    CheckTokens input [Terminal(tt, l)]

/// Contains the tests for the MinJ scanner, see the documentation for details.
type Tests() =
    
    static member TestIdentifierIsTokenizedWithSpaces = CheckTokens " hello " [Identifier("hello", l)]

    static member TestIdentifierIsTokenizedNoSpaces = CheckTokens "hello" [Identifier("hello", l)]
    static member TestTwoIdentifiersAreTokenized = CheckTokens " he llo " [Identifier("he", l); Identifier("llo", l)]
    
    static member TestSingleDigitNumber = CheckTokens "3" [Number(3, l)]

    static member TestMultiDigitNumber = CheckTokens "3345Hello" [Number(3345, l); Identifier("Hello", l)]
    
    static member TestConstantChar = CheckTokens "'c'=='b'" [CharConst('c', l);CreateIdentifierOrToken "==" l;CharConst('b', l)]

    static member TestInvalidCharConst = CheckTokens "'askdjh'" [Error(Errors.InvalidCharacterFormat "'askdjh'", l)]

    static member TestTooLargeNumber = 
        CheckTokens "92473246238746238746287364872346283468273462834682364782648" 
            [Error(MinJ.Tokens.Errors.NumberConstantOverflow, l)]

    static member TestTerminals =
        let (input, tokens) = Map.fold (fun state strValue token -> 
                                    match state with 
                                        | (str, tokens) -> (strValue+" "+str, Terminal(token, l) :> Token :: tokens)) 
                                        ("", []) terminalMap
        CheckTokens input tokens

    static member TestIncompleteToken =
        CheckTokenType "&" [typeof<Error>]
        CheckTokenType "|" [typeof<Error>]

    static member TestNoToken = CheckTokens "" []
  
    static member TestSingleLetterIdentifier = CheckTokens "a" [Identifier("a", l)]
    
    static member TestCommentEnd = CheckTokens "//\nIdentifier" [Identifier("Identifier", l)]
 
    static member TestLargeText =
        let tokens = (tokenize (Data.makeClass Data.mainText (Data.makeFunction "test"))).GetEnumerator()
        while tokens.MoveNext() do
            match tokens.Current with
                | :? Error as e ->  Fail <| "Found error token " + e.ToString()
                | _ -> ()