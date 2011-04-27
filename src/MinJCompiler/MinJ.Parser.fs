module MinJ.Parser

open System
open System.IO
open System.Collections.Generic

open Compiler
open MinJ.Scanner

module Errors =
    let UnexpectedEof (token : Token) =
        "Syntax error. The end of the file was encountered unexpectedly. Did you forget a closing bracket?", token.StartLocation

/// The MinJ Parser.
/// <param name="scanner">The scanner that provides the tokens</param>
/// <param name="debugOutput">A stream for logging debug information such as the content of the symbol table</param>
/// <param name="ruleLogger">The helper object for logging the grammar used to parse</param>
type Parser(tokens : IEnumerable<Token>, 
            debugOutput : StreamWriter,
            ruleLogger : IRuleLogger) =     
    
    let scanner = tokens.GetEnumerator()

    /// A symbol table of encountered variable identifiers
    let variables = SymbolTable<VariableAttributes, VariableIdentifier>(fun id a -> id.Attributes <- Some a)
    /// A symbol table of encountered variable identifiers
    let functions = SymbolTable<FunctionAttributes, FunctionIdentifier>(fun id a -> id.Attributes <- Some a)

    /// Runs a parsing function as long as the lookahead returns true.
    /// Returns the list of generated Ast nodes in order they were encountered
    let kleeneClosure lookahead parser =
        let rec kleeneClosure rules =
            if lookahead() then
                kleeneClosure <| parser() :: rules
            else
                rules
        List.rev <| kleeneClosure []

    /// Raises a CompileException with a message saying that the current token was unexpected.
    let raiseUnexpected() =
        match scanner.Current with
            | :? End as token -> 
                raise <| CompilerException([Errors.UnexpectedEof token])
            | _  as token -> 
                raise <| CompilerException([Errors.UnexpectedToken scanner.Current])

    /// Prints debuging information about a function while parsing
    let printFunctionDebug funcId =
        // Output function debug information
        debugOutput.WriteLine(sprintf "Symbol table after parsing %A" funcId)
        variables.PrintDefinedSymbols debugOutput
        debugOutput.WriteLine()


    /// Starts parsing the input characters provided by the scanner.
    /// Returns the abstract syntax tree.
    /// This should only be called once.
    member this.Parse() =
        let prg, errors = 
            try
                // Setup the scanner
                scanner.MoveNext() |> ignore
                checkError scanner
                // Parse
                Some(this.ParsePrg()), []
            with
                | CompilerException(errors) -> 
                    None, errors

        /// Combine and raise a CompilerException if any errors were encountered while parsing
        if prg.IsNone || variables.Errors.Length > 0 || functions.Errors.Length > 0 then
            raise <| CompilerException(variables.Errors @ functions.Errors @ errors)
        prg.Value

    /// "< prg > −− > class i {{< decl >} < main f > {< f unct def >}"
    member this.ParsePrg() =
        ruleLogger.Push "< prg > −− > class i {{< decl >} < main f > {< f unct def >}}"

        // Create a news scope for variables and functions
        variables.PushScope()
        functions.PushScope()


        // Parse
        popTerminal scanner Class
        let i = popIdentifier scanner
        popTerminal scanner OCurly
        let decls = kleeneClosure (lookaheadTerminals scanner [IntT;CharT]) (fun () -> this.ParseDecl GlobalVariable)
        let mainF = this.ParseMainF()
        let funcDefs = kleeneClosure (lookaheadTerminals scanner [IntT;CharT]) this.ParseFunctDef
        popTerminal scanner CCurly
        popEnd scanner

        ruleLogger.Pop()

        // Pop the class scope and resolve all references.
        // For MinJ, only function references should need to be resolved here.
        variables.PopAndResolveScope()
        functions.PopAndResolveScope()

        Ast.Program(decls, mainF, funcDefs)
    
    /// "< decl > −−> < type > <decl'>"
    member this.ParseDecl (scope : VariableScope) : Ast.VariableDeclaration =
        ruleLogger.Push "< decl > −−> < type > <decl'>"
        
        let typ = this.ParseType()
        let declP = this.ParseDeclP scope typ

        ruleLogger.Pop()

        declP

    /// "<decl’> --> i; | [] i = new <type> [ n ];"
    member this.ParseDeclP (scope : VariableScope) typ : VariableDeclaration =
        
        match scanner.Current with
            | Identifier i ->
                ruleLogger.Push "<decl’> --> i;"
                
                pop scanner
                popTerminal scanner SemiCol

                ruleLogger.Pop()

                let attributes = {
                    Definition=i;
                    Type=Primitive(typ);
                    Scope=scope;
                    MemoryAddress=0u;
                }
                variables.Define i attributes
                Ast.NonArrayVariableDeclaration({Token=i;Attributes=Some attributes})

            | _ ->
                ruleLogger.Push "<decl'> --> [] i = new <type> [ n ];"

                popTerminals scanner [OSquare;CSquare]    
                let i = popIdentifier scanner
                popTerminals scanner [Assign;New]
                let typ2 = this.ParseType()
                popTerminal scanner OSquare
                let n = popNumber scanner
                popTerminals scanner [CSquare;SemiCol]
                
                ruleLogger.Pop()

                let attributes = {
                    Definition=i;
                    Type=ArrayType(typ);
                    Scope=scope;
                    MemoryAddress=0u;
                }
                let varId = {Token=i;Attributes= Some attributes}
                variables.Define i attributes
                Ast.ArrayVariableDeclaration(varId, typ2, int(n.Value))

    /// "<main_f> −−> void main(){{< decl >} < st list > }"
    member this.ParseMainF() =
        ruleLogger.Push  "<main_f> −−> void main(){{< decl >} < st list > }"

        variables.PushScope()

        popTerminals scanner [VoidT;Main;OParen;CParen;OCurly]
        let decls = kleeneClosure (lookaheadTerminals scanner [IntT;CharT]) (fun () -> this.ParseDecl(LocalVariable))
        let stList = this.ParseStList()
        popTerminal scanner CCurly
        
        ruleLogger.Pop()

        printFunctionDebug "main"
        variables.PopAndResolveScope()

        Ast.MainFunction(FunctionBody(decls, stList))

    /// "< funct_def > −−> < type > i ( < par list > ){{< decl >} < st list > }"
    member this.ParseFunctDef() =
        ruleLogger.Push "< funct_def > −−> < type > i ( < par list > ){{< decl >} < st list > }"

        variables.PushScope()

        let typ = this.ParseType()
        let i = popIdentifier scanner
        popTerminal scanner OParen
        let parList = this.ParseParList()
        let parameterTypes = List.map (fun (p : Parameter) -> p.Attributes.Type, p.Attributes.Definition :> Token) parList

        let attributes = {
            Definition=i;
            ReturnType = Primitive(typ);
            ParameterTypes = parameterTypes;
            Index = functions.CountDefined() + 1;
         }
        functions.Define i attributes
                
        popTerminals scanner [CParen;OCurly]
        let decls = kleeneClosure (lookaheadTerminals scanner [IntT;CharT]) (fun () -> this.ParseDecl LocalVariable)
        let stList = this.ParseStList()
        popTerminal scanner CCurly

        ruleLogger.Pop()

        printFunctionDebug i
        variables.PopAndResolveScope()

        Ast.FunctionDefinition({Token=i;Attributes=Some attributes}, parList, FunctionBody(decls, stList))

    /// "< par_list > --> e | < p type > i{, < p type > i}"
    member this.ParseParList() : Parameter list =
        match scanner.Current with
            | Terminal CParen -> 
                ruleLogger.Push "< par_list > --> e"
                ruleLogger.Pop()
                []

            | _ ->
                ruleLogger.Push "<par_list> −−> < p type > i{, < p type > i}"

                let typ = this.ParsePType()
                let i = popIdentifier scanner
                
                let attributes = {
                    Definition=i;
                    Type=typ;
                    Scope=ParameterVariable;
                    MemoryAddress=0u;
                }
                variables.Define i attributes
                let p = Ast.Parameter({Token=i;Attributes= Some attributes})

                match scanner.Current with
                    | Terminal Comma -> 
                        pop scanner
                        p :: this.ParseParList()
                    | _ -> [p]
    
    /// "<p_type> --> <type> <p_type’>"
    member this.ParsePType() =
        ruleLogger.Push "<p_type> --> <type> <p_type’>"

        let pTyp = this.ParsePTypeP(this.ParseType())

        ruleLogger.Pop()

        pTyp
        
    /// "<p_type’> --> [] | e"
    member this.ParsePTypeP typ : MinJType =
        match scanner.Current with
            | Terminal OSquare ->
                ruleLogger.Push "<p_type’> --> []"
                ruleLogger.Pop()

                pop scanner
                popTerminal scanner CSquare
                ArrayType(typ)
            
            | _ ->
                ruleLogger.Push "<p_type’> --> e"
                ruleLogger.Pop()

                Primitive typ

    /// "<type> --> int | char"
    member this.ParseType() : PrimitiveType  = 

        let pTyp = 
            match scanner.Current with
                | Terminal IntT -> 
                    ruleLogger.Push "<type> --> int"
                    pop scanner
                    Ast.IntType
                | Terminal CharT ->
                    ruleLogger.Push "<type> --> char"
                    pop scanner
                    Ast.CharType
                | _ as token ->
                    raise <| CompilerException([Errors.UnexpectedToken token])

        ruleLogger.Pop()

        pTyp

    /// "<st> −−> <comp_st> | i <st'> | while <l_exp><st> | if <lexp> <stmt> else <stmt> |  return <exp>; | System.out. (<v_list>); | ;"
    member this.ParseSt() =
        match scanner.Current with
            | Terminal OCurly ->
                ruleLogger.Push "<st> −−> <comp_st>"   
                
                let compSt = this.ParseCompSt()

                ruleLogger.Pop()

                compSt
            
            | Identifier i ->
                ruleLogger.Push  "<st> −−> i <st'>"

                pop scanner
                let stP = this.ParseStP i

                ruleLogger.Pop()

                stP
            
            | Terminal If ->
                ruleLogger.Push  "<st> −−> if <lexp> <stmt> else <stmt>"   

                pop scanner
                let lExp = this.ParseLExp()
                let stIf = this.ParseSt()
                popTerminal scanner Else
                let stElse = this.ParseSt()

                ruleLogger.Pop()

                Ast.IfElse(lExp, stIf, stElse)
            
            | Terminal While ->
                ruleLogger.Push "<st> −−> while <l_exp><st>"   
                pop scanner
                let lExp = this.ParseLExp()
                let body = this.ParseSt()

                ruleLogger.Pop()

                Ast.WhileStatement(lExp, body)
            
            | Terminal Return ->
                ruleLogger.Push "<st> −−> return <exp>;"
                
                pop scanner
                let exp = this.ParseExp()
                popTerminal scanner SemiCol

                ruleLogger.Pop()

                Ast.ReturnStatement exp
            
            | Terminal System ->
                ruleLogger.Push  "<st> −−> System.out. (<v_list>);"

                pop scanner
                popTerminals scanner [Period;Out;OParen]
                let vList = this.ParseVList()
                popTerminals scanner [CParen;SemiCol]

                ruleLogger.Pop()

                Ast.SystemOutInvocation vList
            
            | Terminal SemiCol ->
                ruleLogger.Push "<st> −−> ;"
                
                pop scanner

                ruleLogger.Pop()

                Ast.EmptyStatement
            
            | _ ->
                raiseUnexpected()
    
    /// "<st'> −−> <asg_st> | (v_list)"
    member this.ParseStP (i : Identifier) =
        match scanner.Current with
            | AnyTerminalOf [OSquare;Assign] ->
                ruleLogger.Push "<st'> −−> <asg_st>"
                
                let asgSt = this.ParseAsgSt i

                ruleLogger.Pop()

                asgSt

            | _ ->
                ruleLogger.Push "<st'> −−> (v_list)"

                popTerminal scanner OParen
                let vList = this.ParseVList()
                popTerminals scanner [CParen;SemiCol]
                
                ruleLogger.Pop()

                let funcId : FunctionIdentifier = {Token=i;Attributes=None}
                functions.Reference i funcId
                Ast.MethodInvocationStatement(funcId, vList)

    /// "<comp_st> −−> { <st_list> }"
    member this.ParseCompSt() =
        ruleLogger.Push "<comp_st> −−> { <st_list> }"
        
        popTerminal scanner OCurly
        let stList = this.ParseStList()
        popTerminal scanner CCurly
        
        ruleLogger.Pop()
        
        Ast.Block stList

    /// "<set> −−> <st> { <st> }"
    member this.ParseStList() =
        ruleLogger.Push "<set> −−> <st> { <st> }"
        let st = this.ParseSt()
        let rest = kleeneClosure (fun () ->  
            (terminals scanner [OParen;If;Else;While;Return;System;SemiCol]).IsSome 
                || scanner.Current :? Identifier) this.ParseSt
        
        ruleLogger.Pop()

        st :: rest

    /// "<set> −−> <st> { <st> }"
    member this.ParseAsgSt (i : Identifier) =
        ruleLogger.Push "<set> −−> <st> { <st> }"
        let var = this.ParseVar i
        popTerminal scanner Assign
        let asgStP = this.ParseAsgStP var
        popTerminal scanner SemiCol

        ruleLogger.Pop()

        Ast.AssignmentStatement asgStP

    /// "<var> −−> i <index>"
    member this.ParseVar() =
        this.ParseVar(popIdentifier scanner)

    member this.ParseVar i =
        ruleLogger.Push "<var> −−> i <index>"

        let index = this.ParseIndex()
        
        ruleLogger.Pop()
        
        let varId = {Token=i;Attributes=None}
        variables.Reference i varId
        Ast.VariableReference(varId, index)
        
    /// "<asg_st'> --> System.in.<type>() | <exp>"
    member this.ParseAsgStP var : Ast.Assignment =
        match scanner.Current with
            | Terminal System ->
                ruleLogger.Push "<asg_st'> --> System.in.<type>()"

                pop scanner
                popTerminals scanner [Period;In;Period]
                let typ = this.ParseType()
                popTerminals scanner [OParen;CParen]

                ruleLogger.Pop()

                Ast.SystemInAssignment(var, typ)
            
            | _ -> 
                ruleLogger.Push "<asg_st'> --> <exp>"

                let exp = this.ParseExp()
                
                ruleLogger.Pop()

                Ast.ExpressionAssignment(var, exp)

    /// "<index> --> [<exp>] | e"
    member this.ParseIndex() : Ast.Expression option =
        match scanner.Current with
            | Terminal OSquare ->
                ruleLogger.Push "<index> --> [<exp>]"

                pop scanner
                let exp = this.ParseExp()
                popTerminal scanner CSquare

                ruleLogger.Pop()

                Some(exp)
            | _ ->
                ruleLogger.Push "<index> --> e"
                ruleLogger.Pop()

                None

    /// "<l_exp> --> <rel_exp> <l_exp’>"
    member this.ParseLExp() =
        ruleLogger.Push "<l_exp> --> <l_exp'> { || <l_exp> }"
        let logExpLeft = this.ParseAnd()
        match scanner.Current with
            | Terminal Or ->
                pop scanner
                let logExpRight = this.ParseLExp()
                Ast.LogicalExpression(logExpLeft, Ast.OrOp, logExpRight)
            | _ ->
                logExpLeft

    member this.ParseAnd() =
        ruleLogger.Push "<l_exp’> --> <exp> { && <l_exp'> }"
        let logLeft = Ast.SingletonLogicalExpression(this.ParseRelExp())
        match scanner.Current with
            | Terminal And ->
                pop scanner
                let logRight = this.ParseAnd()
                ruleLogger.Pop()
                Ast.LogicalExpression(logLeft, Ast.AndOp, logRight)
            | _ ->
                ruleLogger.Pop()
                logLeft

    /// "<rel_exp> --> ( < exp >< rel op >< exp > )"
    member this.ParseRelExp() : Ast.RelativeExpression =
        popTerminal scanner OParen
        let expLeft = this.ParseExp()

        ruleLogger.Push "<rel_exp> --> ( < exp >< rel op >< exp > )"

        let operator = 
            match scanner.Current with
                | Terminal Greater -> Ast.Gt
                | Terminal Lesser -> Ast.Lt
                | Terminal Equal -> Ast.Eq
                | Terminal LesserEqual -> Ast.LtEq
                | Terminal GreaterEqual -> Ast.GtEq
                | Terminal NotEqual -> Ast.NotEq
                | _ -> raiseUnexpected()
        
        pop scanner
        let expRight = this.ParseExp()

        popTerminal scanner CParen

        ruleLogger.Pop()

        Ast.RelativeExpression(expLeft, operator, expRight)

    /// "<exp> --> <term><exp'> | -<term><exp'>"
    member this.ParseExp() =
        match scanner.Current with
            | Terminal OParen
            | Identifier _
            | Number _
            | CharConst _ ->
                ruleLogger.Push "<exp> --> <term><exp'>"
                
                let exp = Ast.Expression(false, this.ParseTerm(), this.ParseExpP())

                ruleLogger.Pop()

                exp

            | Terminal Sub ->
                ruleLogger.Push "<exp> --> -<term><exp'>"

                pop scanner
                let neg = Ast.Expression(true, this.ParseTerm(), this.ParseExpP())

                ruleLogger.Pop()

                neg

            | _ ->
                raiseUnexpected()

    /// "<exp'> --> <add_op><term><exp'> | e"
    member this.ParseExpP() =
        match scanner.Current with
            | Terminal Add
            | Terminal Sub as token ->
                ruleLogger.Push "<exp'> --> <add_op><term><exp'>"

                pop scanner
                let term = this.ParseTerm()
                let expP = this.ParseExpP()
                
                ruleLogger.Pop()
                
                match token with
                    | Terminal Add -> 
                        Some <| Ast.ExpressionPrime(AddOp, term, expP)
                    | Terminal Sub -> 
                        Some <| Ast.ExpressionPrime(SubOp, term, expP)
                    | _ -> 
                        raise <| CompilerInternalException("Expected a Add or Sub at this point in the code")
            | _ ->
                ruleLogger.Push "<exp'> --> e"
                ruleLogger.Pop()
                None

    /// "<term> --> <prim><term'>"
    member this.ParseTerm() =
        ruleLogger.Push "<term> --> <prim><term'>"

        let prim = this.ParsePrim()
        let termP = this.ParseTermP()

        ruleLogger.Pop()

        Ast.Term(prim, termP)
        
    /// "<term'> --> <mult_op><prim><term'>"
    member this.ParseTermP() =
        let operator = 
            match scanner.Current with
                | Terminal Mul ->
                    ruleLogger.Push "<term'> --> <mult_op><prim><term'>"
                    Some Ast.MulOp
                | Terminal Div ->
                    ruleLogger.Push "<term'> --> <mult_op><prim><term'>"
                    Some Ast.DivOp
                | Terminal Mod ->
                    ruleLogger.Push "<term'> --> <mult_op><prim><term'>"
                    Some Ast.ModOp
                | _ ->
                    ruleLogger.Push "<term'> --> e"
                    None
        let ast = 
            match operator with
                | Some(op) ->
                    pop scanner
                    let prim = this.ParsePrim()
                    let termP = this.ParseTermP()
                    Some <| Ast.TermP(op, prim, termP)
                | None ->
                    None
                
        ruleLogger.Pop()
        ast

    /// "<prim> --> i <prim'> | n | c | (<exp>)"
    member this.ParsePrim() =
        match scanner.Current with
            | Identifier i ->
                ruleLogger.Push "<prim> --> i <prim'>"
                
                pop scanner
                let ast = this.ParsePrimP i

                ruleLogger.Pop()

                ast
            
            | Number n ->
                ruleLogger.Push "<prim> --> n"
                ruleLogger.Pop()

                pop scanner
                Ast.NumberPrimitive n
            
            | CharConst cc ->
                ruleLogger.Push "<prim> --> 'c'"
                ruleLogger.Pop()

                pop scanner
                Ast.CharPrimitive cc
            
            | Terminal OParen ->
                ruleLogger.Push "<prim> --> (<exp>)"

                pop scanner
                let exp = this.ParseExp()
                popTerminal scanner CParen

                ruleLogger.Pop()

                Ast.ParenPrimitive(exp)
            
            | _ ->
                raiseUnexpected()

    /// "<prim'> --> (<v_list>) | <index>"
    member this.ParsePrimP i =
        match scanner.Current with
            
            | Terminal OParen ->
                ruleLogger.Push "<prim'> --> (<v_list>)"

                pop scanner
                let vList = this.ParseVList()
                popTerminal scanner CParen

                ruleLogger.Pop()

                let funcId : FunctionIdentifier = {Token=i;Attributes=None}
                functions.Reference i funcId
                Ast.MethodInvocationPrimitive(funcId, vList)
            
            | _ ->
                ruleLogger.Push "<prim'> --> <index>"

                let index = this.ParseIndex()

                ruleLogger.Pop()

                let varId = {Token=i;Attributes=None}
                variables.Reference i varId
                Ast.VariablePrimitive(Ast.VariableReference(varId, index))

    /// "<v_list> --> <elem><v_list'> | ,<v_list> | e"
    member this.ParseVList() =
        ruleLogger.Push "<v_list> --> <elem><v_list'>"
        
        let elem = this.ParseElem()
        
        ruleLogger.Pop()

        match scanner.Current with
            
            | Terminal Comma ->
                pop scanner
                ruleLogger.Push "<v_list'> --> ,<v_list'>"
                
                let ast = elem :: this.ParseVList()
                
                ruleLogger.Pop()
                
                ast

            | _ ->
                ruleLogger.Push "<v_list'> --> e"
                ruleLogger.Pop()

                [elem]

    /// "<elem> --> i <index> | 'c' | n"
    member this.ParseElem() =
        match scanner.Current with
            | Identifier i ->
                ruleLogger.Push "<elem> --> i <index>"

                pop scanner
                
                let index = this.ParseIndex()

                ruleLogger.Pop()

                let varId = {Token=i;Attributes=None}
                variables.Reference i varId

                Ast.VariableElement(Ast.VariableReference(varId, index))

            | CharConst cc ->
                ruleLogger.Push "<elem> --> 'c'"
                ruleLogger.Pop()

                pop scanner
                Ast.CharConstElement cc

            | Number n ->
                ruleLogger.Push "<elem> --> n"
                ruleLogger.Pop()
                
                ruleLogger.Pop()
                pop scanner
                Ast.NumberElement n

            | _ ->
                raiseUnexpected()

let parse debugOutput ruleLogger tokens = (new Parser(tokens, debugOutput, ruleLogger)).Parse()