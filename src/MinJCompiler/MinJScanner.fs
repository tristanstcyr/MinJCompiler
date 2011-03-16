/// Scanner specific to the MinJ language.
[<AutoOpen>]
module MinJ.Scanner

open Scanner
open System.Collections.Generic
open System
open MinJ.ScannerStateMachine

/// Thrown when a token was encountered but another token was expected.
/// This indicates that there is a syntax error in the input.
exception UnexpectedToken of Token
/// Thrown when the end of the source arrived earlier than expected.
exception UnexpectedEnd
/// Thrown by the parser when it encounters an Error token.
exception TokenizationError of Error

type MinJScanner(scanner : IEnumerator<Token>) =
    
    member this.MoveNext() = scanner.MoveNext()

    member this.Current with get() = scanner.Current

    member this.terminal ttyp = 
        match scanner.Current with
            | :? Terminal as cu when cu.Type = ttyp -> Some()
            | _ -> None

    /// Returns an option with Some if the current lookahead
    /// is a terminal and matches with anything in str
    member this.terminals types =
        match scanner.Current with
            | :? Terminal as current ->
                let rec matchList str =
                    match str with
                        | s :: t when s = current.Type -> Some(current)
                        | s :: t -> matchList t
                        | [] -> None
                matchList types
            | _ -> None
    
    /// Returns the current token is its type matches tokenType
    member this.matchType (tokenType : Type) =
        if tokenType.IsInstanceOfType(scanner.Current) then
            Some(scanner.Current)
        else
            None
    
    /// Raises an UnexpectedToken exception if result is None
    member private this.orRaise result =
        match result with
            | Some(d) -> d
            | None -> raise <| UnexpectedToken(scanner.Current)

    /// Raises an exception if the current token is an Error.
    member this.checkError() =
        if scanner.Current :? Error then
            raise <| TokenizationError(scanner.Current :?> Error)

    /// Moves the scanner.stream to the next token no matter what
    /// lookahead token happens to be there.
    member this.Pop() = 
        if scanner.MoveNext() then
            this.checkError()

    /// Pops the End token or raises an expection
    member this.PopEnd() =
        match scanner.Current with
            | :? End ->
                this.Pop()
            | _ ->
                raise <| UnexpectedToken(scanner.Current)
    
    /// Pops a terminal of a specific type or raises an exception
    member this.PopTerminal tt =
        this.terminal tt |> this.orRaise
        this.Pop()

    /// Pops all terminals in a list or raises an exception
    member this.PopTerminals tts = for s in tts do this.PopTerminal s

    /// Pops an identifier or raises an exception
    member this.PopIdentifier() = 
        let i = this.matchType typeof<Identifier> |> this.orRaise
        this.Pop()
        i :?> Identifier

    /// Pops a number or raises an exception
    member this.PopNumber() =
        let n = this.matchType typeof<Number> |> this.orRaise
        this.Pop()
        n :?> Number

    /// Returns a function that returns true if the next tokens are
    /// terminals and match any of the terminals in the list
    member this.LookaheadTerminals strs = fun () -> (this.terminals strs).IsSome

    interface IDisposable with
        member this.Dispose() = scanner.Dispose()
      

/// Convenice method for creating a MinJScanner.
let createMinJScanner chars listingWriter = 
    new MinJScanner(new Scanner(createMinJStateMachine(), chars, listingWriter) :> IEnumerator<Token>)