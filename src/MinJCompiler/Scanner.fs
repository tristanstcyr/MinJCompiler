/// A language agnostic scanner.
[<AutoOpen>]
module Scanner.Scanner
open System.Collections.Generic
open System.IO
open Tokens

/// Essentially a function that takes a char and gives
/// the next state or no state if there's no match
type Transition = char -> State option  

/// Produces a token given a string. Different states may have different producers
and TokenProducer = string -> Location -> Token

/// Represents a state in the scanner process
and State(isDefiningToken : bool, tokenProducer : TokenProducer option, transition : Transition) =
   
   /// A transition that goes nowhere regardless of the character
    static member private NullTransition x = None

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
    new(isDefiningToken, producer : TokenProducer) = State(isDefiningToken, Some producer, State.NullTransition)
    new(isDefiningToken, producer : TokenProducer, transition) = State(isDefiningToken, Some producer, transition)
    new(isDefiningToken) = State(isDefiningToken, None, State.NullTransition)

/// Function that takes a sequence of chars and a state machine
/// and returns a sequence of Tokens *)
type Scanner(rootState: State, characters : IEnumerable<char>, listing : IListingWriter) =
    
    (* Some mutable variables *)
    let mutable location = ref OriginLocation
    let mutable tokenStart = ref OriginLocation
    let mutable currentToken = ref []
    (* Setup the enumerator. Manipulating this enumerator creates some side-effects. *)
    let charsEnum = characters.GetEnumerator()

    /// Helper function for creating tokens from the current state
    let MakeToken (state : State) =
        let JoinChars c = List.fold (fun s c -> (s + c.ToString())) "" c
        let token = state.ProduceToken(List.rev !currentToken |> JoinChars, !tokenStart)
        currentToken := []
        token
    
    /// Helper funciton for advancing the location depending
    /// on the current character. 
    /// Also updates the listing.
    let AdvanceLocation c =
        location := match c with
                    | '\n' -> 
                        listing.AdvanceLine()
                        AdvanceRow !location
                    | _ ->
                        listing.AddChar(c)
                        AdvanceCol !location
    
    let tokens =
        if not <| charsEnum.MoveNext() then
            (* The char sequence was empty *)
            Seq.empty
        else
            
            /// Most of the work is done here. The funciton recursively
            /// calls itself and yields tokens as is travels across states
            let rec Scan (currentState : State) = seq {
                    
                let c = charsEnum.Current
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
                    if not <| charsEnum.MoveNext() then
                            
                        (* We're done with the stream of characters.
                            Case 1: If we haven't collected any character.
                            Just return.
                               
                            Case 2: If we're on a final state and we have characters.
                            Generate a token.
                               
                            Case 3: If we're not on a final state but we have characters,
                            that's a problem. It means we only have part of a token.
                            We yield an error token. *)
                        if Seq.isEmpty !currentToken then
                            yield End(!location) :> Token
                        else
                            if nextState.IsFinal then
                                yield MakeToken nextState
                                yield End(!location) :> Token
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
            Seq.map (fun t -> listing.AddToken(t); t) <| Scan rootState
    
    /// Enumeration is delegated to this IEnumerator
    let tokensEnum = tokens.GetEnumerator()

    interface System.Collections.IEnumerator with
        member this.MoveNext() = tokensEnum.MoveNext()
        member this.Reset() = tokensEnum.Reset()
        member this.Current with get() = tokensEnum.Current :> obj
    interface IEnumerator<Token> with
        member this.Current with get() = tokensEnum.Current
        member this.Dispose() = tokensEnum.Dispose()
        

open System.IO
/// Opens a file into a sequence of chars.
/// This is where some of the magic of loading characters on demand happens.
let ToCharSeq path = seq {
    use reader = new StreamReader(File.OpenRead(path))
    while not reader.EndOfStream do
        yield char(reader.Read())
} 