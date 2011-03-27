module MinJ.ParserIdentifierResolutionTests

open TestFramework
open Compiler
open Scanner
open MinJ.Parser
open MinJ.Scanner
open MinJ

open System.IO
open System

let p str errorCount = 
    let memStream = new StreamWriter(new MemoryStream())
    let parser = new Parser(createMinJScanner str <| NullListingWriter(), memStream, RuleLogger(memStream))
    let errors = 
        try
            let ast = parser.Parse()
            Seq.empty
        with
            | CompilerException(errors) -> errors
    if Seq.length errors <> errorCount then
        Fail (sprintf "Expected %d errors but got %d" errorCount (Seq.length errors))


type ParserIdentifierResolutionTests() =
    static member TestNoErrors = 
        p "class X { int field; void main() {;} }" 0
    static member TestPrimError = 
        p "class X { int field; void main() { i=3;} }" 1

    static member TestFunctionForwardReference = 
        p "class X { void main() { func(3); } int func(int i) {;} }" 0
    static member TestElemError = 
        p "class X { int field; void main() { int i; i=func(x); } int func(int x) {;} }" 1
    static member TestVarError = 
        p "class X { int field; void main() { int i; a=func(3); } int func(int x) {;} }" 1
    static member TestField = 
        p "class X { int field; void main() { int i; field=func(3); } int func(int x) {;} }" 0
    
    static member TestParameters =
        p "class X { void main() { ; } int func(int x) { x=3;} }" 0

    static member TestDuplicateField =
        p "class X { int field; int field; void main() { ; } }" 1
    static member TestDuplicateParameter =
        p "class X { void main() { ; } int func(int x, int x) { ;} }" 1
    static member TestDuplicateLocal =
        p "class X { void main() { ; } int func(int x) { int i; int i; ;} }" 1

    static member TestFieldShadowing = 
        p "class X { int field; void main() { int field; field=func(3); } int func(int x) {;} }" 0
    static member TestNoParameterShadowing =
        p "class X { void main() { ; } int func(int x) { int x;;} }" 1

    static member TestFunctionTableClearing = 
        p "class X { void main() { int i; i=func(3); } int func(int x) { i=3;} }" 1
    

/// Runs all tests for the MinJ lexer
let RunAllTests() = 
    RunAllTests typeof<ParserIdentifierResolutionTests>
    Console.WriteLine("\nAll tests have been run")
    ignore(Console.Read())