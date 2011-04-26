/// Scanner specific to the MinJ language.
module MinJ.Scanner

open Compiler

open System.Collections.Generic
open System

type Tokens = IEnumerator<Token>

let scan chars listingWriter : Tokens =
    Compiler.Scanner.Scan 
        <| MinJ.ScannerStateMachine.Create()
        <| chars
        <| listingWriter

let terminal (tokens : Tokens) ttyp = 
    match tokens.Current with
        | :? Terminal as cu when cu.Type = ttyp -> Some()
        | _ -> None

/// Returns an option with Some if the current lookahead
/// is a terminal and matches with anything in str
let terminals (tokens : Tokens) types =
    match tokens.Current with
        | :? Terminal as current ->
            let rec matchList str =
                match str with
                    | s :: t when s = current.Type -> Some(current)
                    | s :: t -> matchList t
                    | [] -> None
            matchList types
        | _ -> None

/// Returns the current token if its type matches tokenType
let matchType (this : Tokens) (tokenType : Type) =
    if tokenType.IsInstanceOfType(this.Current) then
        Some(this.Current)
    else
        None

/// Raises an UnexpectedToken exception if result is None
let private orRaise (this : Tokens) result =
    match result with
        | Some(d) -> d
        | None ->
            let head = this.Current
            raise <| CompilerException(
                [sprintf "Unexpected token \"%A\"" head, head.StartLocation])

/// Raises an exception if the current token is an Error.
let checkError (this : Tokens) =
    match this.Current with
        | :? Error as error -> 
            raise <| CompilerException([error.ToString(), error.StartLocation])
        | _ -> ()


/// Moves the scanner.stream to the next token no matter what
/// lookahead token happens to be there.
let pop (tokens : Tokens) = 
    if tokens.MoveNext() then
        checkError tokens

/// Pops the End token or raises an expection
let popEnd (tokens : Tokens) =
    match tokens.Current with
        | :? End ->
            pop tokens
        | _ ->
            raise <| CompilerException(["Unexpected token ", tokens.Current.StartLocation])

/// Pops a terminal of a specific type or raises an exception
let popTerminal (tokens : Tokens) tt =
    terminal tokens tt |> orRaise tokens
    pop tokens

/// Pops all terminals in a list or raises an exception
let popTerminals this tts = List.iter  (popTerminal this) tts

/// Pops an identifier or raises an exception
let popIdentifier tokens = 
    let i = matchType tokens typeof<Identifier> |> orRaise tokens
    pop tokens
    i :?> Identifier

/// Pops a number or raises an exception
let popNumber tokens =
    let n = matchType tokens typeof<Number> |> orRaise tokens
    pop tokens
    n :?> Number

/// Returns a function that returns true if the next tokens are
/// terminals and match any of the terminals in the list
let lookaheadTerminals this strs = fun () -> (terminals this strs).IsSome