module ParserTests
open Scanner
open MinJ.Scanner
open MinJ.Parser
open MinJ.Tokens
open MinJ

open System
open System.Diagnostics
open TestFramework
open System.IO

let p str = 
    let memStream = new StreamWriter(new MemoryStream())
    let parser = new Parser(createMinJScanner str <| NullListingWriter(), memStream, RuleLogger(memStream))
    parser.Init()
    parser

/// Contains the tests for the MinJ parser, see the documentation for details.
type ScannerTests() =
    
    static member TestIndexNull =           p("x").ParseIndex()
    static member TestIndex =               p("[1]").ParseIndex()
    
    static member TestElemId =              p("hello").ParseElem()
    static member TestElemNumber =          p("123").ParseElem()
    static member TestElemChar =            p("'c'").ParseElem()
    static member TestElemArray =           p("heju[3]").ParseElem()
    
    static member TestPrimId =              p("heju").ParsePrim()
    static member TestPrimArray =           p("heju[3]").ParsePrim()
    static member TestPrimNumber =          p("123").ParsePrim()
    static member TestPrimChar =            p("'1'").ParsePrim()
    static member TestPrimParen =           p("('1')").ParsePrim()
    static member TestPrimFunctionCall =    p("hello(a)").ParsePrim()
    
    static member TestTerm =                p("a * b").ParseTerm()
    
    static member TextExp =                 p("a + b * b").ParseExp()
    
    static member TestRelExp =              p("(a + b * b != 3)").ParseRelExp()
    
    static member TestLExpSimple =          p("(a + b * b != 3)").ParseLExp()
    static member TestLExpComplex =         p("(a + b * b != 3) && (a != 3)").ParseLExp()

    static member TestAsgStSystem =         p("[3] = System.in.int();").ParseAsgSt <| Identifier("a", OriginLocation)
    static member TestAsgStExp =            p("[3] = 1 + 3;").ParseAsgSt <| Identifier("a", OriginLocation)

    static member TestCompSt =              p("{ ; }").ParseCompSt()
    
    static member TestStCompSt =            p("{ ; }").ParseSt()
    static member TestStIf =                p("if (1 == 3) ; else ;").ParseSt()
    static member TestStWhile =             p("while (1 == 3);").ParseSt()
    static member TestStReturn =            p("return 3;").ParseSt()
    static member TestStSystem =            p("System.out(3);").ParseSt()

    static member TestStAsgSt =             p("i = 3;").ParseSt()
    static member TestStCall =              p("i(3);").ParseSt()

    static member TestPTypeSimple =         p("int").ParsePType()
    static member TestPTypeArray =          p("int[]").ParsePType()

    static member TestParList =             p("int feer, int deer, char beer, char ceer").ParseParList()

    static member TestFunctDefNoParam =     p("char hello() { ; }").ParseFunctDef()
    static member TestFunctDefNoDecl =      p("char hello(int a, int b) { ; }").ParseFunctDef()
    static member TestFunctDef =            p("char hello(int a, int b) { int i;; }").ParseFunctDef()

    static member TestDeclSimple =          p("int i;")
    static member TestDeclArray =           p("int[] myArray = new int[3];")

    static member TestPrgSimple =           p("class hello { void main() { ; } }")
    static member TestPrgDecl =             p("class hello { int z; char x; void main() { ; } }")
    static member TestPrgFunctDef =         p("class hello { int z; char x; void main() { ; } int myFunc() { ; } }")     

/// Runs all tests for the MinJ lexer
let RunAllTests() = 
    RunAllTests typeof<ScannerTests>
    Console.WriteLine("\nAll tests have been run")
    ignore(Console.Read())