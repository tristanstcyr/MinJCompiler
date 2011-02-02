/// A language agnostic scanner.
module Scanner
open System.Collections.Generic

/// Defines a location in a file
type Location = {
    /// The line number
    Row: int;
    Col: int;
}

(* Some helpers for Location *)
/// The beginning of a file
let OriginLocation = {Row=1;Col=1;}
let AdvanceRow l = {Row = l.Row + 1; Col = 1}
let AdvanceCol l = {l with Col = l.Col + 1}

(* Lets define some base token types that are useful for any language *)
/// Base of all tokens
type Token(str : string, startloc : Location) = 
    member this.StartLocation with get() = startloc
    override this.ToString() = str
/// Represents an error during the lexing phase
type Error(str, startloc : Location) =
    inherit Token(str, startloc)
type Keyword(str, startloc : Location) = 
    inherit Token(str, startloc)
/// A single character terminal symbol
type Terminal(str, startloc : Location) = inherit Token(str, startloc)

/// A transition that goes nowhere regardless of the character
let NullTransition x = None

/// Essentially a function that takes a char and gives
/// the next state or no state if there's no match
type Transition = char -> State option  

/// Produces a token given a string. Different states may have different producers
and TokenProducer = string -> Location -> Token

/// Represents a state in the scanner process
and State(isDefiningToken : bool, tokenProducer : TokenProducer option, transition : Transition) =
   
    /// True if this state produces a token
    member this.IsFinal with get() = tokenProducer.IsSome

    /// True if this state participates in the definition of a token
    member this.IsDefiningToken with get() = isDefiningToken
    
    /// Returns the next state in function of the character or None
    member this.NextState c = transition c
    
    /// Produces a token given the token's string value and its location
    member this.ProduceToken(s, l) = 
        if tokenProducer.IsNone then
            raise(System.Exception("State is not final an cannot produce a token"))
        tokenProducer.Value s l

    new(isDefiningToken, transition) = State(isDefiningToken, None, transition)
    new(isDefiningToken, producer : TokenProducer) = State(isDefiningToken, Some producer, NullTransition)
    new(isDefiningToken, producer : TokenProducer, transition) = State(isDefiningToken, Some producer, transition)
    new(isDefiningToken) = State(isDefiningToken, None, NullTransition)
        
/// Function that takes a sequence of chars and a state machine
/// and returns a sequence of Tokens *)
let Tokenize (rootState: State) (characters : IEnumerable<char>) =
    
    (* Some mutable variables *)
    let location = ref OriginLocation
    let tokenStart = ref OriginLocation
    let currentToken = ref []

    /// Helper function for creating tokens from the current state
    let MakeToken (state : State) =
        let JoinChars c = List.fold (fun s c -> (s + c.ToString())) "" c
        let token = state.ProduceToken(List.rev !currentToken |> JoinChars, !tokenStart)
        currentToken := []
        token
    
    /// Helper funciton for advancing the location depending
    /// on the current character
    let AdvanceLocation c =
        location := match c with
                    | '\n' -> AdvanceRow !location
                    | _ -> AdvanceCol !location
            
    (* Setup the enumerator. Manipulating this enumerator creates some side-effects. *)
    let enum = characters.GetEnumerator()
    
    if not <| enum.MoveNext() then
        (* The char sequence was empty *)
        Seq.empty
    else
        /// Most of the work is done here. The funciton recursively
        /// calls itself and yields tokens as is travels across states
        let rec Scan (currentState : State) = seq {
                    
            let c = enum.Current
            let nextState = currentState.NextState c
                 
            (* We've reached the longest token *)
            if nextState.IsNone then

                (* If the state is final, we can produce a token *)
                if currentState.IsFinal then
     
                    let token = MakeToken currentState
                    yield token
                    
                    match token with
                        | :? Error -> () // Do not continue
                        | _ -> yield! Scan rootState // Continue
                    
                else
                    (* It's not final, we've got only part of a token or an invalid character *)
                    yield Error("Unexpected character " + c.ToString(), !location) :> Token
                    
            else (* We can move to the next state *)
                        
                let nextState = nextState.Value

                (* If the next state is defining a token, then
                   we mark this as the start of the next token if
                   the previous state was not so. *)
                if nextState.IsDefiningToken then
                    if not currentState.IsDefiningToken then
                        tokenStart := !location

                    (* Record this character, it's part of token *)
                    currentToken := c :: !currentToken
                        
                else 
                    (* We're not defining a token anymore so lets clear what we have
                        this can happen when we encounter the '//' for a comment for example. *)
                    currentToken := []

                (* Move forward in the character stream if any characters are left *)
                if not <| enum.MoveNext() then
                            
                    (* We're done with the stream of characters.
                        Case 1: If we haven't collected any character.
                        Just return.
                               
                        Case 2: If we're on a final state and we have characters.
                        Generate a token.
                               
                        Case 3: If we're not on a final state but we have characters,
                        that's a problem. It means we only have part of a token.
                        We yield an error token. *)
                    if not <| Seq.isEmpty !currentToken then
                        if nextState.IsFinal then
                            yield MakeToken nextState
                        else
                            yield Error("Unexpected end of file", !location) :> Token
                else

                    (* We ain't done yet! 
                       Advance the location and recursively yield
                       the remaining tokens *)
                    AdvanceLocation c
                    yield! Scan nextState
        }

        (* Start with the root case *)
        Scan rootState