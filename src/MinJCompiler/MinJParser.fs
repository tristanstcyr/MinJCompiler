module MinJ.Parser
open Scanner
open System
open System.IO
open System.Collections.Generic

/// Thrown when a token was encountered but another token was expected.
/// This indicates that there is a syntax error in the input.
exception UnexpectedToken of Token
/// Thrown when the end of the source arrived earlier than expected.
exception UnexpectedEnd
/// Thrown by the parser when it encounters an Error token.
exception TokenizationError of Error

/// Instances of this class form a hierarchy of rules that can be used
/// for printing the rules that were using 
type Rule = | Rule of string * Rule option list

// Add a method to print the Rule class to the TextWriter class
type public TextWriter with
    member public this.Write(rule : Rule) =
        let rec printRule depth rule =
            match rule with
                | Some(Rule(str, subrules)) ->
                    if depth > 0 then 
                        for i in [1..depth] do 
                            this.Write "  "
                    this.WriteLine str
                    for subrule in subrules do
                        printRule (depth + 1) subrule 
                | None -> ()
        printRule 0 <| Some(rule)

(* Some active patterns for matching tokens *)
let private (|Terminal|_|) tt (token : Token) =
        match token with
            | :? Terminal as t when t.Type = tt -> Some(Terminal)
            | _ -> None
let private (|CharConst|_|) (token : Token) =
    match token with
        | :? CharConst as cc -> Some(cc)
        | _ -> None
let private (|Number|_|) (token : Token) =
    if token :? Number then
        Some(token :?> Number)
    else
        None
let private (|Identifier|_|) (token : Token) = 
    if token :? Identifier then
        Some(token :?> Identifier)
    else
        None
/// Matches any of a list of terminal types
let private (|AnyTerminalOf|_|) strs token =
    let rec matchTerminal strs =
        if List.isEmpty strs then 
            None
        else
            let str = List.head strs
            match token with
                | Terminal str -> Some(AnyTerminalOf)
                | _ -> matchTerminal <| List.tail strs
    matchTerminal strs


/// The MinJ Parser.
type Parser(scanner : IEnumerator<Token>) =
  
    let terminal ttyp = 
        match scanner.Current with
            | :? Terminal as cu when cu.Type = ttyp -> Some()
            | _ -> None

    /// Returns an option with Some if the current lookahead
    /// is a terminal and matches with anything in str
    let terminals types =
        match scanner.Current with
            | :? Terminal as current ->
                let rec matchList str =
                    match str with
                        | s :: t when s = current.Type -> Some(current)
                        | s :: t -> matchList t
                        | [] -> None
                matchList types
            | _ -> None
    
    /// Returns the current token is its type matches tokenType
    let matchType (tokenType : Type) =
        if tokenType.IsInstanceOfType(scanner.Current) then
            Some(scanner.Current)
        else
            None
    
    /// Raises an UnexpectedToken exception if result is None
    let orRaise result =
        match result with
            | Some(d) -> d
            | None -> raise <| UnexpectedToken(scanner.Current)

    /// Raises an exception if the current token is an Error.
    let checkError() =
        if scanner.Current :? Error then
            raise <| TokenizationError(scanner.Current :?> Error)

    /// Moves the scanner.stream to the next token no matter what
    /// lookahead token happens to be there.
    let pop() = 
        if scanner.MoveNext() then
            checkError()
    
    let raiseUnexpected() =
        match scanner.Current with
            | :? End -> raise <| UnexpectedEnd
            | _ -> raise <| UnexpectedToken scanner.Current

    /// Pops the End token or raises an expection
    let popEnd() =
        match scanner.Current with
            | :? End ->
                pop()
            | _ ->
                raise <| UnexpectedToken(scanner.Current)
    
    /// Pops a terminal of a specific type or raises an exception
    let popTerminal tt =
        terminal tt |> orRaise
        pop()

    /// Pops all terminals in a list or raises an exception
    let popTerminals tts = for s in tts do popTerminal s

    /// Pops an identifier or raises an exception
    let popIdentifier() = 
        let i = matchType typeof<Identifier> |> orRaise
        pop()
        i :?> Identifier

    /// Pops a number or raises an exception
    let popNumber() =
        let n = matchType typeof<Number> |> orRaise
        pop()
        n :?> Number
      
    /// Runs a parsing function as long as the lookahead
    /// returns true.
    let kleeneClosure lookahead parser =
        let rec kleeneClosure rules =
            if lookahead() then
                kleeneClosure <| parser() :: rules
            else
                rules
        kleeneClosure []
            
    /// Returns a function that returns true if the next tokens are
    /// terminals and match any of the terminals in the list
    let lookaheadTerminals strs = fun () -> (terminals strs).IsSome

    (* Initiate the scanner *)
    do scanner.MoveNext() |> ignore
    (* Check if the first token is an error *)
    do checkError()

    (* The following methods have 1-1 to correspondance with the rules of the grammar 
       most of what is going on here is simple and very repetitive. All rules return
       a rule of option. Rule form a hierarchy which are then used for printing. *)

    member this.ParsePrg() =
        popTerminal Class
        let i = popIdentifier()
        popTerminal OCurly
        let decls = kleeneClosure (lookaheadTerminals [IntT;CharT]) this.ParseDecl
        let mainF = this.ParseMainF()
        let funcDef = kleeneClosure (lookaheadTerminals [IntT;CharT]) this.ParseFunctDef
        popTerminal CCurly
        popEnd()
        Some<|Rule("< prg > −− > class i {{< decl >} < main f > {< funct def >}", 
            decls @ mainF :: funcDef)
    
    member this.ParseDecl() =
        let typ = this.ParseType()
        let declP = this.ParseDeclP()
        Some<|Rule("<decl> --> <type> <decl'>", [typ;declP])

    member this.ParseDeclP() =
        match matchType typeof<Identifier> with
            | Some(i) ->
                pop()
                popTerminal SemiCol
                Some<|Rule("<decl'> --> i;", [])
            | None ->
                popTerminals [OSquare;CSquare]
                let i = popIdentifier()
                popTerminals [Assign;New]
                let typ2 = this.ParseType()
                popTerminal OSquare
                let n = popNumber()
                popTerminals [CSquare;SemiCol]
                Some<|Rule("<decl'> --> [] i = new <type> [ n ];", [typ2])

    member this.ParseMainF() =
        popTerminals [VoidT;Main;OParen;CParen;OCurly]
        let decls = kleeneClosure (lookaheadTerminals [IntT;CharT]) this.ParseDecl
        let stList = this.ParseStList()
        popTerminal CCurly
        Some<|Rule("<main_f> --> void main(){{<decl>}<st_list>}", decls@[stList])

    member this.ParseFunctDef() =
        let typ = this.ParseType()
        let i = popIdentifier()
        popTerminal OParen
        let parList = this.ParseParList()
        popTerminals [CParen;OCurly]
        let decls = kleeneClosure (lookaheadTerminals [IntT;CharT]) this.ParseDecl
        let stlist = this.ParseStList()
        popTerminal CCurly
        Some<|Rule("<funct_def> --> <type> i ( <par_list> ) { { <decl> } <st_list> }", 
            typ::parList::decls @ [stlist])

    member this.ParseParList() =
        match terminal CParen with
            | Some(_) ->
                Some<|Rule("<par_list> --> e", [])
            | None ->
                let pType1 = this.ParsePType()
                let i1 = popIdentifier()
                let pType2, id = 
                    match terminal Comma with
                        | Some(_) -> 
                            pop();
                            this.ParsePType(), Some(popIdentifier())
                        | None -> None, None
                Some(Rule("<par_list> --> < p_type > i{, <p_type> i}", [pType1;pType2]))
    
    member this.ParsePType() =
        let typ = this.ParseType()
        let pTypeP = this.ParsePTypeP()
        Some<|Rule("<p_type> --> <type> <p_type'>", [typ;pTypeP])
        
    member this.ParsePTypeP() =
        match scanner.Current with
            | Terminal OSquare ->
                pop()
                popTerminal CSquare
                Some<|Rule("<p_type'> --> []", [])
            | _ ->
                Some<|Rule("<p_type'> --> e", [])

    member this.ParseType() =
        match scanner.Current with
            | Terminal IntT -> 
                pop()
                Some(Rule("<type> --> int", []))
            | Terminal CharT ->
                pop()
                Some(Rule("<type> --> char", []))
            | _ ->
                raise <| UnexpectedToken scanner.Current

    member this.ParseSt() =
        match scanner.Current with
            | Terminal OCurly ->
                let compSt = this.ParseCompSt()
                Some(Rule("<st> --> <comp_st>", [compSt]))
            | Identifier i ->
                pop()
                let stp = this.ParseStP()
                Some(Rule("<st> --> i <st'>", [stp]))
            | Terminal If ->
                pop()
                let lExp = this.ParseLExp()
                let stIf = this.ParseSt()
                popTerminal Else
                let stElse = this.ParseSt()
                Some(Rule("<st> --> if <l_exp> <st> else <st>", [lExp;stIf;stElse]))
            | Terminal While ->
                pop()
                let lExp = this.ParseLExp()
                let body = this.ParseSt()
                Some(Rule("<st> --> while <l_exp> <st>", [lExp;body]))
            | Terminal Return ->
                pop()
                let exp = this.ParseExp()
                popTerminal SemiCol
                Some(Rule("<st> --> return <exp>;", [exp]))
            | Terminal System ->
                pop()
                popTerminals [Period;Out;OParen]
                let vList = this.ParseVList()
                popTerminals [CParen;SemiCol]
                Some(Rule("<st> --> System.out(<vlist>);", [vList]))
            | Terminal SemiCol ->
                pop()
                Some(Rule("<st> --> ;", []))
            | _ ->
                raiseUnexpected()
    
    member this.ParseStP() =
        match scanner.Current with
            | AnyTerminalOf [OSquare;Assign] ->
                let asgSt = this.ParseAsgSt()
                Some(Rule("<st'> --> <asg_st>;", [asgSt]))
            | _ ->
                popTerminal OParen
                let vList = this.ParseVList()
                popTerminals [CParen;SemiCol]
                Some(Rule("<st'> --> (<v_list>);", [vList]))

    member this.ParseCompSt() =
        popTerminal OCurly
        let stList = this.ParseStList()
        popTerminal CCurly
        Some(Rule("<comp_st> --> <st_list>;", [stList]))

    member this.ParseStList() =
        let st = this.ParseSt()
        let rest = kleeneClosure (fun () ->  
            (terminals [OParen;If;Else;While;Return;System;SemiCol]).IsSome || scanner.Current :? Identifier) this.ParseSt
        Some<|Rule("<st_list> --> <st> {<st>}", st::rest)

    member this.ParseAsgSt() =
        let index = this.ParseIndex()
        popTerminal Assign
        let asgStP = this.ParseAsgStP()
        popTerminal SemiCol
        Some(Rule("<asg_st> --> <index> = <asg_st'>", [index;asgStP]))
        
    member this.ParseAsgStP() =
        match scanner.Current with
            | Terminal System ->
                pop()
                popTerminals [Period;In;Period]
                let typ = this.ParseType()
                popTerminals [OParen;CParen;]
                Some(Rule("<asg_st'> --> System.in.<type>();", [typ]))
            | _ -> 
                let exp = this.ParseExp()
                Some(Rule("<asg_st'> --> <exp>;", [exp]))

    member this.ParseVar() =
        let i = popIdentifier()
        let index = this.ParseIndex()
        Some(Rule("<var> --> i <index>;", [index]))

    member this.ParseIndex() =
        match scanner.Current with
            | Terminal OSquare ->
                pop()
                let exp = this.ParseExp()
                popTerminal CSquare
                Some(Rule("<index> --> [ <exp> ]", [exp]))
            | _ ->
                Some(Rule("<index> --> e", []))

    member this.ParseLExp() =
        let relExp = this.ParseRelExp()
        let lexpP = this.ParseLExpP()
        Some(Rule("<l_exp> --> <rel_exp><l_exp'>", [relExp;lexpP]))

    member this.ParseLExpP() =
        match scanner.Current with
            | AnyTerminalOf [And;Or] ->
                let logop = this.ParseLogOp()
                let lexp = this.ParseLExp()
                Some(Rule("<l_exp'> --> <log_op><l_exp>", [logop;lexp]))
            | _ ->
                Some(Rule("<l_exp'> --> e", []))

    member this.ParseLogOp() =
        match scanner.Current with
            | Terminal And ->
                pop()
                Some(Rule("<log_op> --> &&", []))
            | Terminal Or ->
                pop()
                Some(Rule("<log_op> --> ||", []))
            | _ ->
                raiseUnexpected()

    member this.ParseRelExp() =
        popTerminal OParen
        let expleft = this.ParseExp()
        let relop = this.ParseRelOp()
        let expright = this.ParseExp()
        popTerminal CParen
        Some(Rule("<rel_exp> --> (<exp><rel_exp><exp>)", [expleft;relop;expright]))

    member this.ParseRelOp() =
        match scanner.Current with
            | AnyTerminalOf [Greater;Lesser;Equal;LesserEqual;GreaterEqual;Not;NotEqual] ->
                let t = scanner.Current
                pop()
                Some(Rule("<rel_op> --> "+t.ToString(), []))
            | _ ->
                raiseUnexpected()

    member this.ParseExp() =
        match scanner.Current with
            | Terminal OParen
            | Identifier _
            | Number _
            | CharConst _ ->
                let term = this.ParseTerm()
                let expP = this.ParseExpP()
                Some(Rule("<exp> --> <term><exp'>", [term;expP]))
            | Terminal Sub ->
                pop()
                let term = this.ParseTerm()
                let expP = this.ParseExpP()
                Some(Rule("<exp> --> - <term><exp'>", [term;expP]))
            | _ ->
                raiseUnexpected()

    member this.ParseExpP() =
        match scanner.Current with
            | AnyTerminalOf [Sub;Add] ->
                let addOp = this.ParseAddOp()
                let term = this.ParseTerm()
                let expP = this.ParseExpP()
                Some(Rule("<exp'> --> - <add_op><term><exp'>", [addOp;term;expP]))
            | _ ->
                Some(Rule("<exp> --> e", []))

    member this.ParseTerm() =
        let prim = this.ParsePrim()
        let termP = this.ParseTermP()
        Some(Rule("<term> --> <prim><term'>", [prim;termP]))
        
    member this.ParseTermP() =
        match scanner.Current with
            | AnyTerminalOf [Mul;Div;Mod] ->
                let multOp = this.ParseMultOp()
                let prim = this.ParsePrim()
                let termP = this.ParseTermP()
                Some(Rule("<term'> --> <multi_op><prim><term'>", [multOp;prim;termP]))
            | _ ->
                Some(Rule("<term'> --> e", []))

    member this.ParsePrim() =
        Some<| match scanner.Current with
                | Identifier i ->
                    pop()
                    let primP = this.ParsePrimP()
                    Rule("<prim> --> i <prim'>", [primP])
                
                | Number n ->
                    pop()
                    Rule("<prim> --> n", [])

                | CharConst cc ->
                    pop()
                    Rule("<prim> --> 'c'", [])

                | Terminal OParen ->
                    pop()
                    let exp = this.ParseExp()
                    popTerminal CParen
                    Rule("<prim> --> (<exp>)", [exp])

                | _ ->
                    raiseUnexpected()

    member this.ParsePrimP() =
        match scanner.Current with
            | Terminal OParen ->
                pop()
                let vlist = this.ParseVList()
                popTerminal CParen
                Some(Rule("<prim'> --> (<v_list>)", [vlist]))

            | _ ->
                let index = this.ParseIndex()
                Some(Rule("<prim> --> <index>", [index]))
    
    member this.ParseAddOp() =
        match scanner.Current with
            | AnyTerminalOf [Add;Sub] ->
                let t = scanner.Current
                pop()
                Some(Rule("<add_op> --> "+t.ToString(), []))

            | _ ->
                raiseUnexpected()

    member this.ParseMultOp() =
        match scanner.Current with
            | AnyTerminalOf [Mul;Div;Mod] ->
                let t = scanner.Current
                pop()
                Some(Rule("<add_op> --> "+t.ToString(), []))

            | _ ->
                raiseUnexpected()

    member this.ParseVList() =
        let elem = this.ParseElem()
        match scanner.Current with
            | Terminal Comma ->
                let vlist = this.ParseVList()
                Some(Rule("<vlist> --> <elem> , <v_list>", [elem;vlist]))
            | _ ->
                Some(Rule("<vlist> --> <elem>", [elem]))

    member this.ParseElem() =
        match scanner.Current with
            | Identifier i ->
                pop()
                let index = this.ParseIndex()
                Some(Rule("<elem> --> i <index>", [index]))
            | CharConst cc ->
                pop()
                Some(Rule("<elem> --> 'c'", []))
            | Number n ->
                pop()
                Some(Rule("<elem> --> n", []))
            | _ ->
                raiseUnexpected()

/// Convenience function for calling the parser. This is equivalent to 
/// Parser(tokens).ParsePrg().Value
let parse tokens = Parser(tokens).ParsePrg().Value