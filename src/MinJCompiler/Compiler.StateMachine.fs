module Compiler.StateMachine

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
            raise(CompilerInternalException("State is not final an cannot produce a token"))
        tokenProducer.Value s l

    new(isDefiningToken, transition) = State(isDefiningToken, None, transition)
    new(isDefiningToken, producer : TokenProducer) = State(isDefiningToken, Some producer, State.NullTransition)
    new(isDefiningToken, producer : TokenProducer, transition) = State(isDefiningToken, Some producer, transition)
    new(isDefiningToken) = State(isDefiningToken, None, State.NullTransition)