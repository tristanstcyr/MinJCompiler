/// Scanner specific to the MinJ language.
[<AutoOpen>]
module MinJ.Scanner

#nowarn "40"

open Scanner

/// All types of characters that can be encountered.
type private CharType = 
        | Letter | TerminalChar | Digit | Space | NewLine 
        | Plus | Minus | Mul | Div | Mod
        | Percent | Eq | Gt | Lt | Other | Ampersand | Pipe | SQuote | Eof

/// Tokenizes a sequence of chars
let Tokenize chars = 
    
    /// The MinJ lexer state machine. See documentation for a graphic
    /// representation of the machine. Looking at just the code might not make thing really clear.
    let stateMachine =
        
        /// Function that returns the chracter type of a character in constant time.
        let getCharType = 
            /// Precomputed array of character where indices map to CharTypes
            /// This way, we can have constant time lookup of character types
            let charTypeArrayMap = 
                /// Active Pattern that check if a character is in a set
                let (|ContainedIn|_|) s c = if Set.contains c s then Some(ContainedIn) else None
            
                (* Define sets that are used by the active patterns below for pattern matching. 
                All Sets are based on binary trees and therefore offer fast access time *)
                let identifierChars = Set.ofList (['a'..'z'] @ ['A'..'Z'] @ ['$'; '_'])
                let digits = Set.ofList ['0'..'9']
                let terminals = Set.ofList ['('; ')'; ';'; ','; '['; ']'; '{'; '}'; '.']
                let whiteSpace = Set.ofList [' '; '\t'; '\r']
                let numOps = Set.ofList ['+'; '-'; '*'; '%']
            
                (* Create the array. Each index is matched against its corresponding characters type. *)
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

            (* Return the actually function that does a closure
               on the above map. *)
            fun c ->
                let intValue = int(c)
                if intValue >= (charTypeArrayMap.Length) then
                    Other
                else
                    charTypeArrayMap.[intValue]
        
        (* Build the state machine. Nothing too interesting here really. 
           
           Except notice which states where passes false or true false
           in their constructor. True means that the state contributes
           to the creation of a token, false means that it does not. *)

        let rec root = new State(false, fun c ->
            match getCharType c with
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
                | _                -> None)
        
        and div = State(true, (fun s l -> NumOp("/", l) :> Token), fun c -> 
            match getCharType c with
                | Div -> Some comment
                | _ -> None)
        
        and terminal = State(true, fun s l -> Terminal(s, l) :> Token)
        
        and numOp = State(true, (fun s l -> NumOp(s, l) :> Token))
        
        and comment = State(false, fun c -> 
            match getCharType c with
                | NewLine | Eof -> Some root
                | _ -> Some comment)
        
        and identifier = 
            State(true, CreateIdentifierOrToken, fun c -> 
            match getCharType c with
                | Letter | Digit -> Some identifier
                | _ -> None)
        
        and or0 = State(true, fun c ->
            match getCharType c with
                | Pipe -> Some or1
                | _ -> None)
        and or1 = State(true, fun s l -> LogOp("||", l) :> Token)
        
        and notEqual0 = State(true, fun c ->
            match getCharType c with
                | Eq -> Some rel1
                | _ -> None)
        
        and assign = State(true, (fun s l -> Assign(l) :> Token), fun c -> 
            match getCharType c with
                | Eq -> Some rel1
                | _ -> None)
        and relProducer = fun s l -> RelOp(s, l) :> Token
        
        and rel0 = State(true, relProducer, fun c ->
            match getCharType c with
                | Eq -> Some rel1
                | _ -> None)
        and rel1 = State(true, relProducer)
        
        and and0 = State(true, fun c ->
            match getCharType c with
                | Ampersand -> Some and1
                | _ -> None)
        and and1 = State(true, fun s l -> LogOp("&&", l) :> Token)
        
        and digit = State(true, (fun s l -> Number.Create s l), fun c ->
            match getCharType c with
                | Digit -> Some digit
                | _ -> None)
        
        and charConst0 = State(true, fun c->
            match c with
                | ''' -> None
                | _ -> Some charConst1)
        and charConst1 = State(true, fun c ->
            match getCharType c with
                | SQuote -> Some charConst2
                | _ -> Some charConst1)
        and charConst2 = State(true, fun s l -> CharConst.Create s l)
        
        root
    
    (* Do the tokenization *)
    stateMachine |> Tokenize <| chars