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

let useTmpPtr address func cont ast = func cont (Local(address)) ast

type ThreeAddressCodeTranslationTests() =

    static member ``Simple relative expression translation``() =
        let ptr = Local(12u)
        testInMain (p("(3 == 4)").ParseRelExp()) (fun cont ast -> RelativeExpression.ToTac cont ptr ast)
            [Inst3(ptr, Tac.Eq, Constant(0u), Constant(1u))]

    static member ``Term translation``() =
        testInMain (p("3 * 4").ParseTerm()) (useTmpPtr 12u Term.ToTac)
            [Inst3(Local(12u), Tac.Mul, Constant(0u), Constant(1u))]  
            
    static member ``If else statement translation``() =
        testInMain (p("if (1 == 3) System.out(3); else System.out(4);").ParseSt()) Statement.ToTac
            [
                Inst3(Local(12u), Tac.Eq, Constant(0u), Constant(1u))
                IfFalse(Local(12u), Label(1))
                Write(Constant(2u))
                Goto(Label(2))
                Labeled(Label(1))
                Write(Constant(3u))
                Labeled(Label(2))
            ]

    static member ``While statement``() =
        testInMain (p("while (1 == 3) System.out(10);").ParseSt()) Statement.ToTac
            [
                Labeled(Label(1))
                Inst3(Local(12u), Tac.Eq, Constant(0u), Constant(1u))
                IfFalse(Local(12u), Label(2))
                Write(Constant(2u))
                Goto(Label(1))
                Labeled(Label(2))
            ]

    static member ``Nested function calls``() =
        let expected = 
            [
                Push(Constant(0u))
                Call(Label(1), 1)
                Tac.Assign(Global(0u), Result)
            ]

        match p("
            class Foo
            {
                int i; 
                void main()
                { 
                    i = func(3);
                } 
            
                int func(int i)
                {
                    return i;
                }
            }").ParsePrg() with
            | Program(_, MainFunction(FunctionBody(_, call :: rest)), _) ->    
                testInMain call Statement.ToTac expected
            | _ -> 
                Fail("Unexpected parsing result")


    static member ``Efficient assignment of expression``() =
        let expected = 
            [
                Inst3(Global(0u), Tac.Operator.Add, Constant(0u), Constant(1u))
            ]

        match p("
            class Foo
            {
                int i; 
                void main()
                { 
                    i = 10 + 10;
                } 
            }").ParsePrg() with
            | Program(_, MainFunction(FunctionBody(_, call :: rest)), _) ->    
                testInMain call Statement.ToTac expected
            | _ -> 
                Fail("Unexpected parsing result")

    static member ``Short circuiting of logical expressions``() =
        testInMain (p("(4 == 10) && (4 != 10) || ('c' == 'b')").ParseLExp()) (useTmpPtr 12u LogicalExpression.ToTac)
            [
                // 4 == 10
                Inst3(Local(12u), Tac.Eq, Constant(0u), Constant(1u))
                IfFalse(Local(12u), Label(1))
                // && (4 != 10)
                Inst3(Local(12u), Tac.NotEq, Constant(2u), Constant(3u))
                // ||
                Labeled(Label(1))
                IfFalse(Local(12u), Label(2))
                Goto(Label(3))
                Labeled(Label(2))
                // 'c' == 'b'
                Inst3(Local(12u), Tac.Eq, Constant(4u), Constant(5u))
                Labeled(Label(3))
            ]

    static member ``Relative expression sides are stored in different addresses``() =
        let expected = 
            [
                Push(Constant(0u))
                Call(Label(1), 1)
                Tac.Assign(Local(12u), Result)
                Push(Constant(1u))
                Call(Label(1), 1)
                Tac.Assign(Local(16u), Result)
                Inst3(Local(12u), Tac.Eq, Local(12u) , Local(16u))
            ]

        match p("
            class Foo
            {
                int i; 
                void main()
                { 
                    if (func(4) == func(3));
                    else;
                } 
                
                int func(int i)
                {
                    return i;
                }
            }").ParsePrg() with
            | Program(_, MainFunction(FunctionBody(_, IfElse(logExp, _, _) :: rest)), _) ->    
                testInMain logExp (useTmpPtr 12u LogicalExpression.ToTac) expected
            | _ -> 
                Fail("Unexpected parsing result")

    static member ``Results of both sides of an expression are stored in different addresses``() =
        testInMain (p("3 * 4 + 5 * 6").ParseExp()) (useTmpPtr 16u Expression.ToTac)
            [
                Inst3(Local(12u), Tac.Mul, Constant(0u), Constant(1u))
                Inst3(Local(16u), Tac.Mul, Constant(2u), Constant(3u))
                Inst3(Local(16u), Tac.Add, Local(12u), Local(16u))
            ]
     
    static member ``Result of both sides of a term are stored in different addresses``() =
        testInMain (p("(5 + 5) * (6 + 3)").ParseTerm()) (useTmpPtr 16u Term.ToTac)
            [
                Inst3(Local(12u), Tac.Add, Constant(0u), Constant(1u))
                Inst3(Local(16u), Tac.Add, Constant(2u), Constant(3u))
                Inst3(Local(16u), Tac.Mul, Local(12u), Local(16u))
            ]
        
/// Runs all tests for the MinJ lexer
let RunAllTests() = 
    RunAllTests typeof<ThreeAddressCodeTranslationTests>
    Console.WriteLine("\nAll tests have been run")
    ignore(Console.Read())