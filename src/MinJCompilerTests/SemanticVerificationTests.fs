module MinJ.ParserTypeCheckingTests

open TestFramework
open Scanner
open Compiler
open MinJ.Scanner
open MinJ.Parser
open MinJ.Ast
open System.IO
open System

let p str expectError = 
    let memStream = new StreamWriter(new MemoryStream())
    let parser = new Parser(createMinJScanner str <| NullListingWriter(), memStream, RuleLogger(memStream))
    let prg = parser.Parse()
    try
        SemanticVerification.verify prg |> ignore
        if expectError then
            Fail "Expected an error but got none"
    with
        | CompilerException(errors) ->
            if (not expectError) || Seq.length errors > 1 then
                Fail (sprintf "Got more errors than expected")
    

type ParserIdentifierResolutionTests() =
    static member TestAssignmentMatchingTypes = 
        p "class X { int field; void main() {field = 3;} }" false
    static member TestAssignmentNonMatchingTypes =
        p "class X { char field; void main() {field = 3;} }" true

    static member TestReturnTypeMatching() =
        p "class X { int field; void main() {;} int func() { return 3; } }" false
    static member TestReturnTypeNonMatching() =
        p "class X { int field; void main() {;} int func() { return 'a'; } }" true

    static member TestFunctionInvocationMatchingReturnType() =
        p " class X {int field;void main(){field = func(3);}int func(int i){return 3;}}" false
    static member TestFunctionInvocationNonMatchingReturnType() =
        p "class X { int field; void main() {field = func(3);} char func(int i) { return 'a'; } }" true

    static member TestNotAllIfElsePathsReturn() =
        p "class X { void main() {;} int foo() { if (3 == 4) { return 3;} else {;} } }" true

    static member TestNotAllWhilePathsReturn() =
        p "class X { void main() {;} int foo() { while (3 == 4) { return 3;} } }" true

/// Runs all tests for the MinJ lexer
let RunAllTests() = 
    RunAllTests typeof<ParserIdentifierResolutionTests>
    Console.WriteLine("\nAll tests have been run")
    ignore(Console.Read())