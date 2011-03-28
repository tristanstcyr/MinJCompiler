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
open System.IO
open System

/// Helpers function for creating a MinJ parser from a string.
let p str =
    let memStream = new StreamWriter(new MemoryStream())
    let parser = new Parser(createMinJScanner str <| NullListingWriter(), memStream, RuleLogger(memStream))
    parser.Init()
    parser

/// <summary>
/// Translates a MinJ snippet to three address code
/// as if it was place in the main function and compares
/// the result to something expected.
/// </summary>
/// <param name="ast">The MinJ AST node to be transtated</param>
/// <param name="toTac">A function for translating "ast" to TAC</param>
/// <param name="expected">The expected instructions</param>
let testInMain ast toTac expected =
    let prog = ProgramContext(1)
    let func = FunctionContext(prog, Label(0))
    ignore <| (toTac func <| ast)
    let instructions = Seq.toList prog.Instructions
    Assert "Programs are not equal" (instructions = expected)

let useTmpPtr1 func cont ast = func cont TmpPtr1 ast

type ThreeAddressCodeTranslationTests() =
   
    static member ``Simple relative expression translation``() =
        let ptr = Local(12)
        testInMain (p("(3 == 4)").ParseRelExp()) (fun cont ast -> RelativeExpression.ToTac cont ptr ast)
            [Inst3(ptr, Tac.Eq, Constant(0), Constant(1))]

    static member ``Term translation``() =
        testInMain (p("3 * 4").ParseTerm()) (useTmpPtr1 Term.ToTac)
            [Inst3(Local(12), Tac.Mul, Constant(0), Constant(1))]  
            
    static member ``If else statement translation``() =
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

    static member ``While statement``() =
        testInMain (p("while (1 == 3) System.out(10);").ParseSt()) Statement.ToTac
            [
                Labeled(Label(1))
                Inst3(TmpPtr1, Tac.Eq, Constant(0), Constant(1))
                IfFalse(TmpPtr1, Label(2))
                Write(Constant(2))
                Goto(Label(1))
                Labeled(Label(2))
            ]

    static member ``Nested function calls``() =
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

    static member ``Short circuiting of logical expressions``() =
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

    static member ``Relative expression sides are stored in different addresses``() =
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

    static member ``Results of both sides of an expression are stored in different addresses``() =
        testInMain (p("3 * 4 + 5 * 6").ParseExp()) (useTmpPtr1 Expression.ToTac)
            [
                Inst3(TmpPtr2, Tac.Mul, Constant(0), Constant(1))
                Inst3(TmpPtr1, Tac.Mul, Constant(2), Constant(3))
                Inst3(TmpPtr1, Tac.Add, TmpPtr2, TmpPtr1)
            ]

    static member ``Result of both sides of a term are stored in different addresses``() =
        testInMain (p("(5 + 5) * (6 + 3)").ParseTerm()) (useTmpPtr1 Term.ToTac)
            [
                Inst3(TmpPtr2, Tac.Add, Constant(0), Constant(1))
                Inst3(TmpPtr1, Tac.Add, Constant(2), Constant(3))
                Inst3(TmpPtr1, Tac.Mul, TmpPtr2, TmpPtr1)
            ]
        
/// Runs all tests for the MinJ lexer
let RunAllTests() = 
    RunAllTests typeof<ThreeAddressCodeTranslationTests>
    Console.WriteLine("\nAll tests have been run")
    ignore(Console.Read())