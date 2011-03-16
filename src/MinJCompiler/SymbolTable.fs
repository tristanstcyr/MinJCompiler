
namespace MinJ

open System.IO

open MinJ.Tokens
open System.Collections.Generic

(*
type SymbolTable<'AttributeType, 'IdentifierType>(parent : SymbolTable<'AttributeType, 'IdentifierType> option, 
                                                  attributesSetter : 'AttributeType -> 'IdentifierType -> unit) =
    
    let mutable children : SymbolTable<'AttributeType, 'IdentifierType> list = []
    let mutable declarations : Map<string, 'AttributeType> = Map.empty
    let mutable references : 'IdentifierType list = []
    
    let mutable errors : exn list = []

    member this.AddDeclaration i a =
        let name = i.ToString()
        match declarations.TryFind name with
            | Some(a) -> 
                errors <- ParsingError(sprintf "\"%s\" has already been declared" <| i.ToString(), i) 
                    :: errors
            | None -> 
                declarations <- declarations.Add(i.ToString(), a)
                this.AddReference i
    
    member this.AddReference i =
        let name = i.ToString()
        references <- i :: references

    member this.TryFind (i : Identifier) = 
        let name = i.ToString()
        match declarations.TryFind name with
            | Some(a) -> Some(a)
            | None -> 
                if parent.IsSome then
                    parent.Value.TryFind i
                else
                    None

    member this.Resolve() =
        let resolveReference errors i =
            match this.TryFind i with
                | Some(a) -> 
                    i.Attributes <- Some(a)
                    errors
                | None -> 
                    ParsingError(sprintf "\"%s\" was never declared" <| i.ToString(), i) :: errors
        
        let resolveChildReference errors (child : SymbolTable) = 
            child.Resolve() @ errors

        let localErrors = List.fold resolveReference [] references
        List.rev(errors) @ List.fold resolveChildReference localErrors children
                
    member this.Push() = 
        let child = SymbolTable(Some(this))
        children <- child :: children
        child

    member this.Pop() = parent.Value

    member this.PrintEntries (out : StreamWriter) =
        for declaration in declarations do
            out.WriteLine(sprintf "%A -> %A" declaration.Key declaration.Value)
        if parent.IsSome then
            parent.Value.PrintEntries out

    new() = SymbolTable(None)

    *)