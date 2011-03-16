namespace MinJ

open MinJ.Ast

open System.Collections.Generic
open System.IO

type SymbolTable() =
    
    let mutable errors : exn list = []
    let localReferences : List<VariableIdentifier> = new List<VariableIdentifier>()
    let mutable definedLocals : Map<string, VariableAttributes> = Map.empty
    let mutable definedFields : Map<string, VariableAttributes> = Map.empty
    
    let mutable definedFunctions : Map<string, FunctionAttributes> = Map.empty
    let mutable functionReferences : FunctionIdentifier list = []

    member this.Errors with get() = errors

    member private this.AddError(message, i) =
        errors <- ParsingError(message, i) :: errors

    member private this.ResolveLocalReferences() =
        for localRef in localReferences do
            match localRef with
                | VariableIdentifier(i, attributesRef) ->
                    match this.TryFindVariable i with
                        | Some(attributes) ->
                            attributesRef := Some(attributes)
                        | None ->
                            this.AddError(sprintf "Variable \"%s\" referenced but not declared" <| i.ToString(), i)

    member private this.TryFindVariable (i : Identifier) =
        let name = i.ToString()
        match definedLocals.TryFind name with
            | Some(a) -> Some a
            | None -> 
                match definedFields.TryFind name with
                    | Some(f) -> Some f
                    | None -> None

    member this.ClearAndResolveLocals() =
        this.ResolveLocalReferences()
        localReferences.Clear()
        definedLocals <- Map.empty

    member this.ClearAndResolveFunctions() =
        for ref in functionReferences do
            match ref with
                | FunctionIdentifier(i, attributesRef) ->
                    match definedFunctions.TryFind <| i.ToString() with
                        | Some(attributes) ->
                            attributesRef := Some(attributes)
                        | None ->
                            this.AddError(sprintf "Variable \"%s\" referenced but not declared" <| i.ToString(), i)

    member this.AddVariableReference i = localReferences.Add i

    member this.DefineField (varId : VariableIdentifier) =
        match varId with
            | VariableIdentifier(i, attributes) ->
                let name = i.ToString()
                match definedFields.TryFind name with
                    | Some(_) -> 
                        raise <| ParsingError(sprintf "A field named \"%s\" has already been declared" <| i.ToString(), i)
                    | None -> 
                        definedFields <- definedFields.Add(name, attributes.Value.Value)

    member this.DefineLocal i attributes =
        let name = i.ToString()
        match definedLocals.TryFind name with
            | Some(_) ->
                raise <| ParsingError(sprintf "A local named \"%s\" has already been declared" <| i.ToString(), i)
            | None ->
                definedLocals <- definedLocals.Add(name, attributes)

    member this.ReferenceFunction i =
        functionReferences <- i :: functionReferences

    member this.DefineFunction (i : Identifier) newAttributes =
        let name = i.ToString()
        match definedFunctions.TryFind name with
            | Some(_) ->
                this.AddError(sprintf "A functionw with name \"%s\" has already been declared" <| i.ToString(), i) 
            | _ ->
                definedFunctions <- definedFunctions.Add(name, newAttributes)
        

    member this.PrintContent (writer : TextWriter) =
        let printTable (table : Map<string, _>) =
            for t in table do
                writer.WriteLine(sprintf "%s -> %A" t.Key t.Value)
        printTable definedLocals
        printTable definedFields
        printTable definedFunctions