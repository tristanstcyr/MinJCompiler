/// A really cheaply written test framework. At the moment, F# lacks some integration
/// with MSTest. I don't really want to bundle NUnit with this so we'll
/// survive with this for now.
module TestFramework

open System
open System.Reflection

/// Thrown when an assertion fails. This is caught by RunAllTests.
exception AssertionException of string

/// Fails the test with a message
let Fail m = raise(AssertionException(m))

/// Test fails if exp is false
let Assert m exp = if not exp then Fail(m)

/// Runs all functions in a class as tests. Some reflection stuff here,
/// not really interesting.
let RunAllTests (testClass : Type) =
    Array.ForEach((testClass.GetMethods()), (fun m -> 
        if m.IsStatic then
            let name = m.Name
            try
                let result = m.Invoke(null, null)
                Console.WriteLine("Pass : " + name)
            with | e -> 
                printfn "Fail : %s\n%s\n%s" name
                    <| match e.InnerException with
                        | AssertionException(string) -> string
                        | e -> e.Message
                    <| e.InnerException.StackTrace))