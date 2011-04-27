module MinJ.Scanner.Tests.Performance


open System.Diagnostics
open System.Collections.Generic

open Compiler
open MinJ
open MinJ.Tests.Utils

let largeText = 
    "// example of a code in MinJ
    class Average
    }
    //a sample program. It calculates the average of several numbers,
    // the first value gives how many numbers are to be read
    void main() //the main method
    }
    int n,ave;
    n=In.int();
    if (n > 0)
    }
    int total = sum_int(n);
    ave =(total/ n);
    System.out( 'a','=',ave);
    {
    else System.out('e','r','r','o','r');
    {
    int sum_int(int m) // read and sum up m integers
    }
    int i=1;
    int[] val = new int[100];
    int res= 0;
    while (i <=m)
    }
    val[i] =System.in.int();
    res = res + val[i];
    i = i+1;
    {
    return res;
    {
    {"


let double input n = List.fold (fun (s:string) i -> s + s) input [1..n]

let testSize n =
    let input = double largeText n
    
    let tokens = tokenize input
    let averageTime = Seq.average (repeat 3 (fun() -> time (fun() -> consume (tokenize input))))
    let count = Seq.length tokens
    printfn "%i tokens scanned in %.3f ms" count averageTime
    let seconds = averageTime / 1000.0f
    printfn "or %f chars / second" (float32(input.Length) / seconds)
    printfn "or %f tokens / second" (float32(count) / seconds)

type Tests() =
    static member mediumTest = testSize 10
    static member largeTest = testSize 11
    static member xLargeTest = testSize 12
    static member xxLargeTest = testSize 13
        

