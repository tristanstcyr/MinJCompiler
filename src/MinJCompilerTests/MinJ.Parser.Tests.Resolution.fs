module MinJ.Parser.Tests.Resolution

open TestFramework
open MinJ.Tests.Utils
open Compiler
open MinJ

open System
open System.IO


type ParserIdentifierResolutionTests() =
    static member TestNoErrors = 
        parseWithErrorCount "class X { int field; void main() {;} }" 0
    static member TestPrimError = 
        parseWithErrorCount "class X { int field; void main() { i=3;} }" 1

    static member TestFunctionForwardReference = 
        parseWithErrorCount "class X { void main() { func(3); } int func(int i) {;} }" 0
    static member TestElemError = 
        parseWithErrorCount "class X { int field; void main() { int i; i=func(x); } int func(int x) {;} }" 1
    static member TestVarError = 
        parseWithErrorCount "class X { int field; void main() { int i; a=func(3); } int func(int x) {;} }" 1
    static member TestField = 
        parseWithErrorCount "class X { int field; void main() { int i; field=func(3); } int func(int x) {;} }" 0
    
    static member TestParameters =
        parseWithErrorCount "class X { void main() { ; } int func(int x) { x=3;} }" 0

    static member TestDuplicateField =
        parseWithErrorCount "class X { int field; int field; void main() { ; } }" 1
    static member TestDuplicateParameter =
        parseWithErrorCount "class X { void main() { ; } int func(int x, int x) { ;} }" 1
    static member TestDuplicateLocal =
        parseWithErrorCount "class X { void main() { ; } int func(int x) { int i; int i; ;} }" 1

    static member TestFieldShadowing = 
        parseWithErrorCount "class X { int field; void main() { int field; field=func(3); } int func(int x) {;} }" 0
    static member TestNoParameterShadowing =
        parseWithErrorCount "class X { void main() { ; } int func(int x) { int x;;} }" 1

    static member TestFunctionTableClearing = 
        parseWithErrorCount "class X { void main() { int i; i=func(3); } int func(int x) { i=3;} }" 1