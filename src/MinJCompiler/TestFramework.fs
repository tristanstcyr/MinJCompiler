module TestFramework

(* A really modest test framework. At the moment, F# lacks some integration
   with MSTest. I don't really want to bundle NUnit with this so we'll
   survive with this for now. *)
open System
open System.Reflection

(* Thrown when an assertion fails. This is caught by RunAllTests. *)
exception AssertionException of string

let Assert m exp = if not exp then raise(AssertionException(m))

(* Runs all functions in a class as tests. Some reflection stuff here,
   not really interesting. *)
let RunAllTests (testClass : Type) =
    Array.ForEach((testClass.GetMethods()), (fun m -> 
            if m.IsStatic then
                let name = m.Name
                try
                    let result = m.Invoke(null, null)
                    Console.WriteLine("Pass : " + name)
                with
                    | e -> 
                        printfn "Fail : %s" name
                        match e.InnerException with
                            | AssertionException(string) ->
                                printfn "%s" string
                            | e -> 
                                printfn "%s" e.Message
                        printfn "%s" e.InnerException.StackTrace
            ))