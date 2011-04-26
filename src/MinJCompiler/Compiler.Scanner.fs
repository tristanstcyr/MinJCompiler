namespace Compiler

open System.Collections.Generic

/// Function that takes a sequence of chars and a state machine
/// and returns a sequence of Tokens *)
type Scanner(rootState: State, characters : char seq, listing : IListingWriter) =
    
    (* Some mutable variables *)
    let mutable location = ref Location.origin
    let mutable tokenStart = ref Location.origin
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
                        location.Value.advanceRow
                    | _ ->
                        listing.AddChar(c)
                        location.Value.advanceCol
    
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

    member private this.Tokens with get() = tokensEnum

    static member Scan rootState characters listing =
        let scanner = Scanner(rootState, characters, listing)
        scanner.Tokens