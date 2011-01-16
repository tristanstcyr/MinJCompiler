module Scanner
open Tokens
open System.Collections.Generic

(* Essentially a function that takes a char and gives
   the next state or no state if there's no match *)
type Transition = char -> State option  

(* A functional produces a token given a string. Different
   states may have different producers *)
and TokenProducer = string -> Location -> Token

(* Represents a state in a state machine *)
and State(isDefiningToken : bool) =
    let mutable transition : Transition = fun a -> None
    let mutable tokenProducer : TokenProducer option = None
   
    (* True if this state produces a token *)
    member this.IsFinal with get() = tokenProducer.IsSome

    (* True if this state participates in the definition of a token *)
    member this.IsDefiningToken with get() = isDefiningToken
    
    (* Setter for the transition function. This function
       Determines what the next state will be in function of the character *)
    member this.Transition with set(nt) = transition <- nt
    
    (* Returns the next state in function of the character *)
    member this.NextState c = transition c

    (* Setter for a function that produces a token for this state *)
    member this.TokenProducer with set(tp) = tokenProducer <- Some(tp)
    
    (* Produces a token given the token's string value and its location *)
    member this.ProduceToken(s, l) = 
        if tokenProducer.IsNone then
            raise(System.Exception("Not token producer was defined"))
        tokenProducer.Value s l
        
(* Function that takes a sequence of chars and a state machine
   and returns a sequence of Tokens *)
let Tokenize (rootState: State) (characters : IEnumerable<char>) =
    
    (* Some mutable variables *)
    let location = ref OriginLocation
    let tokenStart = ref OriginLocation
    let currentToken = ref []

    (* Some helper functions *)
    let JoinChars c = List.fold (fun s c -> (s + c.ToString())) "" c
    (* Helper function for creating tokens from the current state *)
    let MakeToken (state : State) =
        state.ProduceToken(List.rev !currentToken |> JoinChars, !tokenStart)
    (* Helper funtion for create error tokens *)
    let MakeError msg = Error(msg, !location) :> Token
    (* Helper funciton for advancing the location depending
        on the current character *)
    let AdvanceLocation c =
        location := match c with
                    | '\n' -> AdvanceRow !location
                    | _ -> AdvanceCol !location
            
    (* Setup the enumerator. Manipulating this enumerator creates some side-effects. *)
    let enum = characters.GetEnumerator()
    if enum.MoveNext() then

        (* This function does most of the work. It recursively
            calls itself and yields tokens as is travels across states *)
        let rec Scan (currentState : State) = seq {
                    
            let c = enum.Current
            let nextState = currentState.NextState c
                 
            (* We've reached the longest token *)
            if nextState.IsNone then

                (* If the state is final, we can produce a token *)
                if currentState.IsFinal then
                            
                    yield  MakeToken currentState
                    currentToken := []
                    yield! Scan rootState
                        
                else
                    (* It's not final, we've got only part of a token or an invalid character *)
                    yield MakeError("Unexpected character " + c.ToString())
                    
            else (* We can move to another state *)
                        
                let nextState = nextState.Value

                (* If the next state is defining a token, then
                    we mark this as the start of the next token if
                    the previous state was not so.

                    We also start recording that token
                    *)
                if nextState.IsDefiningToken then
                    if not currentState.IsDefiningToken then
                        tokenStart := !location
                    currentToken := c :: !currentToken
                        
                else 
                    (* We're not defining a token anymore so lets clear what we have
                        this can happen when we encounter the '//' for a comment for example. *)
                    currentToken := []

                (* Move forward in the character stream if any characters are left *)
                if not <| enum.MoveNext() then
                            
                    (* We're done with the stream of characters.
                        Case 1: If we haven't collected any character.
                        We just return.
                               
                        Case 2: If we're on a final state and we have characters.
                        We generate a token.
                               
                        Case 3: If we're not on a final state but we have characters,
                        that's a problem. It means we only have part of a token.
                        We yield an error token. *)
                    if not <| Seq.isEmpty !currentToken then
                        if nextState.IsFinal then
                            yield MakeToken nextState
                        else
                            yield MakeError("Unexpected end of file")
                else

                    (* We ain't done yet! 
                        Advance the location and recursively yield
                        the remaining tokens *)
                    AdvanceLocation c
                    yield! Scan nextState
        }
        Scan rootState
    else
        Seq.empty