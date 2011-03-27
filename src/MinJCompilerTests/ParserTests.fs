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
type ParserTests() =
    
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

    static member TestFunctDefNoParam =     p("class hello{void main(){;} char hello(){;}}").ParsePrg()
    static member TestFunctDefNoDecl =      p("class hello{void main(){;} char hello(int a, int b){;}}").ParsePrg()
    static member TestFunctDef =            p("class hello{void main(){;} char hello(int a, int b){int i;;}}").ParsePrg()

    static member TestPrgDeclSimple =       p("class hello{int i; void main(){;}}").ParsePrg()
    static member TestPrgDeclArray =        p("class hello{int[] myArray = new int[3]; void main(){;}}").ParsePrg()
    
    
/// Runs all tests for the MinJ lexer
let RunAllTests() = 
    RunAllTests typeof<ParserTests>
    Console.WriteLine("\nAll tests have been run")
    ignore(Console.Read())