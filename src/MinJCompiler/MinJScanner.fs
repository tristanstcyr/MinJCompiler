module MinJScanner
open Scanner
open System.Collections.Generic

module private MinJStateMachine =
    open Scanner
    open Tokens
    open MinJTokens

    

    /// Types of the tokens in MinJ
    type CharType = 
        | Letter | TerminalChar | Digit | Space | NewLine 
        | Plus | Minus | Mul | Div | Mod
        | Percent | Eq | Gt | Lt | Other | Ampersand | Pipe | SQuote | Eof
    
    (* Define sets that are used by the active patterns below for pattern matching. 
        All Sets are based on binary trees and therefore offer fast access time *)
    let identifierChars = Set.ofList (['a'..'z'] @ ['A'..'Z'] @ ['$'; '_'])
    let digits = Set.ofList ['0'..'9']
    let terminals = Set.ofList ['('; ')'; ';'; ','; '['; ']'; '{'; '}'; '.']
    let whiteSpace = Set.ofList [' '; '\t'; '\r']
    let numOps = Set.ofList ['+'; '-'; '*'; '%']

    /// Actives patterns for matching groups of characters. Active patterns
    /// are used to enhance pattern matching.
    let (|ContainedIn|_|) s c = if Set.contains c s then Some(ContainedIn) else None

    /// Precomputed array of character where indices map to CharTypes
    let charTypeArrayMap = 
        Array.init 127 <| fun i -> 
            match char(i) with
                | ContainedIn identifierChars -> Letter
                | ContainedIn terminals -> TerminalChar
                | ContainedIn digits -> Digit
                | ContainedIn whiteSpace -> Space
                | '\n' -> NewLine
                | '+' -> Plus
                | '-' -> Minus
                | '*' -> Mul
                | '/' -> Div
                | '=' -> Eq
                | '>' -> Gt
                | '<' -> Lt
                | '&' -> Ampersand
                | '|' -> Pipe
                | ''' -> SQuote
                | '%' -> Mod
                | '\u0004' -> Eof
                | _ -> Other
    
    let GetCharType (c : char) = 
        let intValue = int(c)
        if intValue >= (charTypeArrayMap.Length) then
            Other
        else
            charTypeArrayMap.[intValue]

    (* Returns an identifier or keyword token depending
       if it matches with what is in the keyword set *)
    let IdentifierOrToken t l =
        if keywords.Contains (t) then
            Keyword(t, l) :> Token
        else
            Identifier(t, l) :> Token

    /// Creates the MinJ lexer state machine. See documentation for a graphic
    /// representation of the machine. Look at just the code might not make thing really clear.
    let MinJStateMachine =
        
        (* Define the states *)
        let root = State(false)
        
        let div = State(true)
        div.TokenProducer <- fun s l -> NumOp("/", l) :> Token

        let terminal = State(true)
        terminal.TokenProducer <- fun s l -> Terminal(s, l) :> Token

        let numOp = State(true)
        numOp.TokenProducer <- fun s l -> NumOp(s, l) :> Token

        let comment = State(false)

        let identifier = State(true)
        identifier.TokenProducer <- fun s l -> IdentifierOrToken s l

        let or0 = State(true)
        let or1 = State(true)
        or1.TokenProducer <- fun s l -> LogOp("||", l) :> Token

        let notEqual0 = State(true)

        let assign = State(true)
        assign.TokenProducer <- fun s l -> Assign(l) :> Token

        let relProducer = fun s l -> RelOp(s, l) :> Token
        let rel0 = State(true)
        rel0.TokenProducer <- relProducer
        let rel1 = State(true)
        rel1.TokenProducer <- relProducer

        let and0 = State(true)
        let and1 = State(true)
        and1.TokenProducer <- fun s l -> LogOp("&&", l) :> Token

        let digit = State(true)
        digit.TokenProducer <- CreateNumber

        let charConst0 = State(true)
        let charConst1 = State(true)
        let charConst2 = State(true)
        charConst2.TokenProducer <- CreateCharConst

        (* Define transitions *)
        root.Transition <- fun c ->
            match GetCharType c with
                | Space | NewLine   -> Some root
                | Plus | Minus 
                | Mul | Mod         -> Some numOp
                | Letter            -> Some identifier
                | Div               -> Some div
                | Pipe              -> Some or0
                | Lt | Gt           -> Some rel0
                | Ampersand         -> Some and0
                | Digit             -> Some digit
                | SQuote            -> Some charConst0
                | Eq                -> Some assign
                | TerminalChar      -> Some terminal
                | _                 -> None
        div.Transition <- fun c ->
            match GetCharType c with
                | Div                   -> Some comment
                | _                     -> None
        comment.Transition <- fun c ->
            match GetCharType c with
                | NewLine | Eof         -> Some root
                | _                     -> Some comment
        or0.Transition <- fun c ->
            match GetCharType c with
                | Pipe                  -> Some or1
                | _                     -> None
        identifier.Transition <- fun c ->
            match GetCharType c with
                | Letter | Digit        -> Some identifier
                | _                     -> None
        notEqual0.Transition <- fun c ->
            match GetCharType c with
                | Eq                    -> Some rel1
                | _                     -> None
        assign.Transition <- fun c ->
            match GetCharType c with
                | Eq                    -> Some rel1
                | _ -> None
        rel0.Transition <-  fun c ->
            match GetCharType c with
                | Eq                    -> Some rel1
                | _                     -> None
        and0.Transition <- fun c ->
            match GetCharType c with
                | Ampersand             -> Some and1
                | _                     -> None
        digit.Transition <- fun c ->
            match GetCharType c with
                | Digit             -> Some digit
                | _                 -> None
        charConst0.Transition <- fun c->
            match c with
                | '''               -> None
                | _                 -> Some charConst1
        charConst1.Transition <- fun c ->
            match GetCharType c with
                | SQuote               -> Some charConst2
                | _                 -> Some charConst1

        root

open MinJStateMachine

/// Tokenizes a sequence of chars
let Tokenize chars = Tokenize MinJStateMachine chars