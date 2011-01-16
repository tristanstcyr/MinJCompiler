module MinJScanner
open Scanner
open System.Collections.Generic

module private MinJStateMachine =
    open Scanner
    open Tokens
    open MinJTokens

    (* Define sets that are used by the active patterns below for pattern matching 
       All Sets are based on binary trees and therefore offer fast access time *)
    let identifierChars = Set.ofList (['a'..'z'] @ ['A'..'Z'] @ ['$'; '_'])
    let digits = Set.ofList ['0'..'9']
    let terminals = Set.ofList ['('; ')'; ';'; ','; '['; ']'; '{'; '}'; '.']
    let whiteSpace = Set.ofList [' '; '\n'; '\t'; '\r']
    let newLines = Set.ofList ['\r'; '\n']
    let numOps = Set.ofList ['+'; '-'; '*'; '%']

    (* Actives patterns for matching groups of characters. Active patterns
       Are used to enhance pattern matching. *)
    let Contains s c = if Set.contains c s then Some() else None
    let (|IsEmpty|_|) a = if Seq.isEmpty a then Some() else None
    let (|Letter|_|) c = Contains identifierChars c
    let (|Terminal|_|) c = Contains terminals c
    let (|Digit|_|) c = Contains digits c
    let (|Space|_|) c = Contains whiteSpace c
    let (|NewLine|_|) c = Contains newLines c
    let (|NumOp|_|) c = Contains numOps c
    
    (* Returns an identifier or keyword token depending
       if it matches with what is in the keyword set *)
    let IdentifierOrToken t l =
        if keywords.Contains (t) then
            Keyword(t, l) :> Token
        else
            Identifier(t, l) :> Token

    (* Creates the MinJ lexer state machine. See documentation for a graphic
       representation of the machine. Look at just the code might not make thing really clear.   
     *)
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
        identifier.TokenProducer <- fun s l -> IdentifierOrToken s l;

        let or0 = State(true)
        let or1 = State(true)
        or1.TokenProducer <- fun s l -> LogOp("||", l) :> Token;

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
        digit.TokenProducer <- fun s l -> Number(s, l) :> Token

        let charConst0 = State(true)
        let charConst1 = State(true)
        let charConst2 = State(true)
        charConst2.TokenProducer <- fun s l -> CharConst(s, l) :> Token

        (* Define transitions *)
        root.Transition <- fun c ->
            match c with
                | Space             -> Some root
                | NumOp             -> Some numOp
                | Letter            -> Some identifier
                | '/'               -> Some div
                | '|'               -> Some or0
                | '<' | '>'         -> Some rel0
                | '&'               -> Some and0
                | Digit             -> Some digit
                | '''               -> Some charConst0
                | '='               -> Some assign
                | Terminal          -> Some terminal
                | _                 -> None
        div.Transition <- fun c ->
            match c with
                | '/'                -> Some comment
                | _                  -> None
        comment.Transition <- fun c ->
            match c with
                | '\n' | '\u0004'    -> Some root
                | _                  -> Some comment
        or0.Transition <- fun c ->
            match c with
                | '|'               -> Some or1
                | _                 -> None
        identifier.Transition <- fun c ->
            match c with
                | Letter | Digit    -> Some identifier
                | _                 -> None
        notEqual0.Transition <- fun c ->
            match c with
                | '='               -> Some rel1
                | _ -> None
        assign.Transition <- fun c ->
            match c with
                | '='               -> Some rel1
                | _ -> None
        rel0.Transition <-  fun c ->
            match c with
                | '='               -> Some rel1
                | _                 -> None
        and0.Transition <- fun c ->
            match c with
                | '&'               -> Some and1
                | _                 -> None
        digit.Transition <- fun c ->
            match c with
                | Digit             -> Some digit
                | _                 -> None
        charConst0.Transition <- fun c->
            match c with
                | '''               -> None
                | _                 -> Some charConst1
        charConst1.Transition <- fun c ->
            match c with
                | '''               -> Some charConst2
                | _                 -> None

        root

open MinJStateMachine

(* Function that takes a sequence of chars and returns a sequence of Tokens *)
let Tokenize chars = Tokenize MinJStateMachine chars