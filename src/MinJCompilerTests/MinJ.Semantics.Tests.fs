module MinJ.Semantics.Tests
open TestFramework

open Compiler
open MinJ
open MinJ.Parser

open System.IO
open System

let p str expectError = 
    let memStream = new StreamWriter(new MemoryStream())
    let prg = parse memStream (RuleLogger(memStream)) (MinJ.Scanner.tokenize (NullListingWriter()) str)
    try
        Semantics.verify prg |> ignore
        if expectError then
            Fail "Expected an error but got none"
    with
        | CompilerException(errors) ->
            if (not expectError) || Seq.length errors > 1 then
                Fail (sprintf "Got more errors than expected")
    

type Tests() =

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

    static member ``Statements after a return statement detected as dead``() =
        p "class X { void main() {;} int foo() { int i; return 3; i = 4; } }" true