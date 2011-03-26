namespace MinJ

open System.IO

open MinJ.Tokens
open System.Collections.Generic

/// <summary>Symbol table for MinJ</summary>
/// <param name="resolver">Function to be called when a symbol reference is solved</param>
type SymbolTable<'AttributeType, 'IdentifierType>(resolver) =

    /// Stores errors that are encountered while add definitions or resolving references
    let mutable errors = []
    /// Stores defined symbols
    let mutable defined : Dictionary<string, 'AttributeType> list = []
    /// Stores referenced symbols
    let mutable referenced : (Identifier * 'IdentifierType) list = []

    /// Convenience method for adding errors in the errors list
    member private this.AddError message token =
        errors <- ParsingError(message, token) :: errors

    member this.CountDefined() = List.sum <| List.map (fun (d : Dictionary<_,_>) -> d.Count) defined
            
    /// Defines a symbol and its attributes in the symbol table of the current scope
    member this.Define (idToken : Identifier) a =
        // Ge the current scope
        let scope = defined.Head
        // Add to the current scope if it doesn't already exist
        if scope.ContainsKey(idToken.Value) then
            this.AddError (sprintf "%s has already been defined" idToken.Value) idToken
        else
            scope.Add(idToken.Value, a)

    /// Pushes a new symbol scope to the stack.
    /// After this method is called, added definitions are insert in this new scope.
    member this.PushScope() =
        defined <- new Dictionary<string, 'AttributeType>() :: defined

    /// Helper method for searching the stack of symbol tables for a symbol
    member private this.SearchScope name =
        /// Recursively searches the list of symbol tables
        let rec tryFind name (scope : Dictionary<_,_> list) =
            match scope with
                // Base cased, no symbolm table remains and the symbol was not found
                | [] -> None
                | top :: rest ->
                    if top.ContainsKey(name) then
                        Some top.[name]
                    else
                        tryFind name rest
        tryFind name defined

    /// <summary>Removes the current deepest symbol table in the scope</summary>
    /// Removes all symbols defined since the previous call to PushScope.
    /// Resolves and removes all symbol references since the previous call to PushScope.
    /// Resolution involves a call to the resolver function passed in the constructor of the class.
    member this.PopAndResolveScope() =
        for id, astNode in referenced do
            let name = id.ToString()
            match this.SearchScope name with
                | Some(a) ->
                    resolver astNode a
                | None ->
                     this.AddError (sprintf "\"%s\" referenced but not declared" name) id
        referenced <- []
        defined <- defined.Tail

    /// Adds a reference to be resolved in the current scope.
    member this.Reference id a =
        referenced <- (id, a) :: referenced

    /// Prints all the defined symbols in the current scope. 
    /// Helper method for debugging.
    member this.PrintDefinedSymbols (stream : TextWriter) =
        for table in defined do
            for name in table.Keys do
                stream.WriteLine(sprintf "%A" table.[name])
    
    /// All the errors logged during definition and resolution of symbols
    member this.Errors with get() = errors
    