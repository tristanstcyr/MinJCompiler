module MinJ.ThreeAddressCodeTranslationTests

open TestFramework
open Scanner
open Compiler
open Tac
open MinJ
open MinJ.Scanner
open MinJ.Parser
open MinJ.Ast
open MinJ.Ast.ToTac
open MinJ.Ast.TypeCheck
open System.IO
open System
open MinJ.Ast.TypeCheck

let p str =
    let memStream = new StreamWriter(new MemoryStream())
    let parser = new Parser(createMinJScanner str <| NullListingWriter(), memStream, RuleLogger(memStream))
    parser.Init()
    parser

let testInMain ast toTac expected =
    let prog = ProgramContext(1)
    let func = FunctionContext(prog, Label(0))
    ignore <| (toTac func <| ast)
    let instructions = Seq.toList prog.Instructions
    Assert "Programs are not equal" (instructions = expected)

let useTmpPtr1 func cont ast = func cont TmpPtr1 ast

type ThreeAddressCodeTranslationTests() =
   
    static member TestRelativeExpression() =
        let ptr = Local(12)
        testInMain (p("(3 == 4)").ParseRelExp()) (fun cont ast -> RelativeExpression.ToTac cont ptr ast)
            [Inst3(ptr, Tac.Eq, Constant(0), Constant(1))]

    static member TestTerm() =
        testInMain (p("3 * 4").ParseTerm()) (useTmpPtr1 Term.ToTac)
            [Inst3(Local(12), Tac.Mul, Constant(0), Constant(1))]  
            
    static member TestIfElse() =
        testInMain (p("if (1 == 3) System.out(3); else System.out(4);").ParseSt()) Statement.ToTac
            [
                Inst3(TmpPtr1, Tac.Eq, Constant(0), Constant(1))
                IfFalse(TmpPtr1, Label(1))
                Write(Constant(2))
                Goto(Label(2))
                Labeled(Label(1))
                Write(Constant(3))
                Labeled(Label(2))
            ]

    static member TestWhile() =
        testInMain (p("while (1 == 3) System.out(10);").ParseSt()) Statement.ToTac
            [
                Labeled(Label(1))
                Inst3(TmpPtr1, Tac.Eq, Constant(0), Constant(1))
                IfFalse(TmpPtr1, Label(2))
                Write(Constant(2))
                Goto(Label(1))
                Labeled(Label(2))
            ]

    static member TestNestedFunctionCall() =
        let expected = 
            [
                Push(Constant(0))
                Call(Label(1), 1)
                Tac.Assign(Global(0), TmpPtr1)
            ]

        match p("class Foo{int i; void main(){ i = func(3);} int func(int i){ return i;}}").ParsePrg() with
            | Program(_, MainFunction(FunctionBody(_, call :: rest)), _) ->    
                testInMain call Statement.ToTac expected
            | _ -> 
                Fail("Unexpected parsing result")

    static member TestLogicalExpressionShortCircuit() =
        testInMain (p("(4 == 10) && (4 != 10) || ('c' == 'b')").ParseLExp()) (useTmpPtr1 LogicalExpression.ToTac)
            [
                // 4 == 10
                Inst3(TmpPtr1, Tac.Eq, Constant(0), Constant(1))
                IfFalse(TmpPtr1, Label(1))
                // && (4 != 10)
                Inst3(TmpPtr1, Tac.NotEq, Constant(2), Constant(3))
                // ||
                Labeled(Label(1))
                IfFalse(TmpPtr1, Label(2))
                Goto(Label(3))
                Labeled(Label(2))
                // 'c' == 'b'
                Inst3(TmpPtr1, Tac.Eq, Constant(4), Constant(5))
                Labeled(Label(3))
            ]

    static member TestFunctionCallComparison() =
        let expected = 
            [
                Push(Constant(0))
                Call(Label(1), 1)
                Tac.Assign(TmpPtr2, TmpPtr1)
                Push(Constant(1))
                Call(Label(1), 1)
                Inst3(TmpPtr1, Tac.Eq, TmpPtr2 , TmpPtr1)
            ]

        match p("class Foo{int i; void main(){ if (func(4) == func(3));else;} int func(int i){return i;}}").ParsePrg() with
            | Program(_, MainFunction(FunctionBody(_, IfElse(logExp, _, _) :: rest)), _) ->    
                testInMain logExp (useTmpPtr1 LogicalExpression.ToTac) expected
            | _ -> 
                Fail("Unexpected parsing result")

        
/// Runs all tests for the MinJ lexer
let RunAllTests() = 
    RunAllTests typeof<ThreeAddressCodeTranslationTests>
    Console.WriteLine("\nAll tests have been run")
    ignore(Console.Read())