module MinJ.ToTac.Tests

open TestFramework
open MinJ.Tests.Utils
open Compiler
open Compiler.Tac
open MinJ
open MinJ.ToTac

open System.IO
open System

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
    func.TemporaryVariables.StartAddress <- 12u
    ignore <| (toTac func <| ast)
    let instructions = Seq.toList prog.Instructions
    Assert "Programs are not equal" (instructions = expected)

let useTmpPtr address func cont ast = func cont (Local(address)) ast

let p = makeParser

type ThreeAddressCodeTranslationTests() =

    static member ``Simple relative expression translation``() =
        let ptr = Local(12u)
        testInMain (p("(3 == 4)").ParseRelExp()) (fun cont ast -> RelativeExpression.ToTac cont ptr ast)
            [Inst3(ptr, Tac.Eq, Constant(0u), Constant(4u))]

    static member ``Term translation``() =
        testInMain (p("3 * 4").ParseTerm()) (useTmpPtr 12u Term.ToTac)
            [Inst3(Local(12u), Tac.Mul, Constant(0u), Constant(4u))]  
            
    static member ``If else statement translation``() =
        testInMain (p("if (1 == 3) System.out(3); else System.out(4);").ParseSt()) Statement.ToTac
            [
                Inst3(Local(12u), Tac.Eq, Constant(0u), Constant(4u))
                IfFalse(Local(12u), Label(2))
                Write(Constant(8u))
                Goto(Label(3))
                Labeled(Label(2))
                Write(Constant(12u))
                Labeled(Label(3))
            ]

    static member ``While statement``() =
        testInMain (p("while (1 == 3) System.out(10);").ParseSt()) Statement.ToTac
            [
                Labeled(Label(2))
                Inst3(Local(12u), Tac.Eq, Constant(0u), Constant(4u))
                IfFalse(Local(12u), Label(3))
                Write(Constant(8u))
                Goto(Label(2))
                Labeled(Label(3))
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
                Inst3(Global(0u), Tac.Operator.Add, Constant(0u), Constant(4u))
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
                Inst3(Local(12u), Tac.Eq, Constant(0u), Constant(4u))
                IfFalse(Local(12u), Label(2))
                // && (4 != 10)
                Inst3(Local(12u), Tac.NotEq, Constant(8u), Constant(12u))
                // ||
                Labeled(Label(2))
                IfFalse(Local(12u), Label(3))
                Goto(Label(4))
                Labeled(Label(3))
                // 'c' == 'b'
                Inst3(Local(12u), Tac.Eq, Constant(16u), Constant(20u))
                Labeled(Label(4))
            ]

    static member ``Relative expression sides are stored in different addresses``() =
        let label = Label(1)
        let expected = 
            [
                Push(Constant(0u))
                Call(label, 1)
                Tac.Assign(Local(12u), Result)
                Push(Constant(4u))
                Call(label, 1)
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
                Inst3(Local(12u), Tac.Mul, Constant(0u), Constant(4u))
                Inst3(Local(16u), Tac.Mul, Constant(8u), Constant(12u))
                Inst3(Local(16u), Tac.Add, Local(12u), Local(16u))
            ]

    static member ``Result of both sides of a term are stored in different addresses``() =
        testInMain (p("(5 + 5) * (6 + 3)").ParseTerm()) (useTmpPtr 16u Term.ToTac)
            [
                Inst3(Local(12u), Tac.Add, Constant(0u), Constant(4u))
                Inst3(Local(16u), Tac.Add, Constant(8u), Constant(12u))
                Inst3(Local(16u), Tac.Mul, Local(12u), Local(16u))
            ]