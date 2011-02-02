module MinJ.Parser
open Scanner
open Parser
open LazyList
open System

let parse (tokens : seq<Token>) =
    
    let (|Keyword|_|) str (token : Token) = 
        if token :? Keyword && token.ToString() = str then 
            Some() 
        else 
            None
    let (|Terminal|_|) str (token : Token) = 
        if token :? Terminal && token.ToString() = str then 
            Some() 
        else 
            None
        
    let prg (input : LazyList<Token>) =           
        match input with
            | Cons (Keyword "class", Cons (Terminal "{", rest)) -> Some()
            | _ -> None   

    match LazyList.ofSeq tokens with
        | Prg -> Some()
        | _ -> None