﻿module MinJ.Scanner.Tests.Performance


open System.Diagnostics
open System.Collections.Generic

open Compiler
open MinJ
open MinJ.Tests.Utils
open MinJ.Tests

let testSize n =
    let functions = Data.makeFunctions "test" n
    let input = Data.makeClass Data.mainText functions
    let tokens = tokenize input
    let averageTime = Seq.average (repeat 10 (fun() -> time (fun() -> consume (tokenize input))))
    let count = Seq.length tokens
    printfn "%i tokens scanned in %.3f ms" count averageTime
    let seconds = averageTime / 1000.0f
    printfn "or %f chars / second" (float32(input.Length) / seconds)
    printfn "or %f tokens / second" (float32(count) / seconds)

type Tests() =
    static member warmUp = testSize 25
    static member mediumTest = testSize 25
    static member largeTest = testSize 50
    static member xLargeTest = testSize 100
    static member xxLargeTest = testSize 200
        

