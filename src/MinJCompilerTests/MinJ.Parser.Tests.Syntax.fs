module MinJ.Parser.Tests.Syntax

open TestFramework
open MinJ.Tests.Utils
open MinJ
open MinJ.Parser
open Compiler

open System
open System.Diagnostics
open System.IO

/// Contains the tests for the MinJ parser, see the documentation for details.
type ParserTests() =
    
    static member TestIndexNull =           makeParser("x").ParseIndex()
    
    static member TestIndex =               makeParser("[1]").ParseIndex()
    
    static member TestElemId =              makeParser("hello").ParseElem()
    static member TestElemNumber =          makeParser("123").ParseElem()
    static member TestElemChar =            makeParser("'c'").ParseElem()
    static member TestElemArray =           makeParser("heju[3]").ParseElem()

    static member TestPrimId =              makeParser("heju").ParsePrim()
    static member TestPrimArray =           makeParser("heju[3]").ParsePrim()
    static member TestPrimNumber =          makeParser("123").ParsePrim()
    static member TestPrimChar =            makeParser("'1'").ParsePrim()
    static member TestPrimParen =           makeParser("('1')").ParsePrim()
    static member TestPrimFunctionCall =    makeParser("hello(a)").ParsePrim()

    static member TestTerm =                makeParser("a * b").ParseTerm()

    static member TextExp =                 makeParser("a + b * b").ParseExp()

    static member TestRelExp =              makeParser("(a + b * b != 3)").ParseRelExp()

    static member TestLExpSimple =          makeParser("(a + b * b != 3)").ParseLExp()

    static member TestLExpComplex =         makeParser("(a + b * b != 3) && (a != 3)").ParseLExp()

    static member TestAsgStSystem =         makeParser("[3] = System.in.int();").ParseAsgSt <| Identifier("a", Location.origin)
    static member TestAsgStExp =            makeParser("[3] = 1 + 3;").ParseAsgSt <| Identifier("a", Location.origin)

    static member TestCompSt =              makeParser("{ ; }").ParseCompSt()

    static member TestStCompSt =            makeParser("{ ; }").ParseSt()
    static member TestStIf =                makeParser("if (1 == 3) ; else ;").ParseSt()
    static member TestStWhile =             makeParser("while (1 == 3);").ParseSt()
    static member TestStReturn =            makeParser("return 3;").ParseSt()
    static member TestStSystem =            makeParser("System.out(3);").ParseSt()

    static member TestStAsgSt =             makeParser("i = 3;").ParseSt()
    static member TestStCall =              makeParser("i(3);").ParseSt()

    static member TestPTypeSimple =         makeParser("int").ParsePType()
    static member TestPTypeArray =          makeParser("int[]").ParsePType()

    static member TestFunctDefNoParam =     makeParser("class hello{void main(){;} char hello(){;}}").ParsePrg()
    static member TestFunctDefNoDecl =      makeParser("class hello{void main(){;} char hello(int a, int b){;}}").ParsePrg()
    static member TestFunctDef =            makeParser("class hello{void main(){;} char hello(int a, int b){int i;;}}").ParsePrg()

    static member TestPrgDeclSimple =       makeParser("class hello{int i; void main(){;}}").ParsePrg()
    static member TestPrgDeclArray =        makeParser("class hello{int[] myArray = new int[3]; void main(){;}}").ParsePrg()