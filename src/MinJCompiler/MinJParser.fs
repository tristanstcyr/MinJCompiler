module MinJ.Parser

open System
open System.IO
open System.Collections.Generic

open Compiler
open Scanner
open MinJ.Scanner
open MinJ.Tokens
open MinJ
open MinJ.Ast

/// The MinJ Parser.
/// <param name="scanner">The scanner that provides the tokens</param>
/// <param name="debugOutput">A stream for logging debug information such as the content of the symbol table</param>
/// <param name="ruleLogger">The helper object for logging the grammar used to parse</param>
type Parser(scanner : MinJScanner, 
            debugOutput : StreamWriter,
            ruleLogger : RuleLogger) =
  
    /// Runs a parsing function as long as the lookahead
    /// returns true.
    let kleeneClosure lookahead parser =
        let rec kleeneClosure rules =
            if lookahead() then
                kleeneClosure <| parser() :: rules
            else
                rules
        List.rev <| kleeneClosure []
         
    let variables = SymbolTable<VariableAttributes, VariableIdentifier>(fun id a -> id.Attributes <- Some a)
    let functions = SymbolTable<FunctionAttributes, FunctionIdentifier>(fun id a -> id.Attributes <- Some a)

    let raiseUnexpected() =
        match scanner.Current with
            | :? End -> raise <| CompilerException(["Unexpected end ", scanner.Current.StartLocation])
            | _ -> raise <| CompilerException([sprintf "Unexpected token \"%A\"" scanner.Current, scanner.Current.StartLocation])

    let printFunctionDebug funcId =
        // Output function debug information
        debugOutput.WriteLine(sprintf "Symbol table after parsing %A" funcId)
        variables.PrintDefinedSymbols debugOutput
        debugOutput.WriteLine()

    /// Initializes the parser. This is no in the constructor to avoid throwing
    /// an exception if the scanner doesn't have any tokens or an error.
    member this.Init() =
        (* Initiate the scanner *)
        scanner.MoveNext() |> ignore
        (* Check if the first token is an error *)
        scanner.checkError()

    /// Parses.
    /// Returns the abstract syntax tree
    /// Throws 
    member this.Parse() =
        let prg, errors = 
            try
                this.Init()
                Some(this.ParsePrg()), Seq.empty
            with
                | CompilerException(errors) -> 
                    None, errors
        if prg.IsNone || variables.Errors.Length > 0 || functions.Errors.Length > 0 then
            raise <| CompilerException(Seq.concat [Seq.ofList variables.Errors; Seq.ofList functions.Errors;errors])
        prg.Value

    /// "< prg > −− > class i {{< decl >} < main f > {< f unct def >}"
    member this.ParsePrg() =
        ruleLogger.Push "< prg > −− > class i {{< decl >} < main f > {< f unct def >}}"

        // Create a news scope for variables and functions
        variables.PushScope()
        functions.PushScope()

        // Parse
        scanner.PopTerminal Class
        let i = scanner.PopIdentifier()
        scanner.PopTerminal OCurly
        let decls = kleeneClosure (scanner.LookaheadTerminals [IntT;CharT]) (fun () -> this.ParseDecl GlobalVariable)
        let mainF = this.ParseMainF()
        let funcDefs = kleeneClosure (scanner.LookaheadTerminals [IntT;CharT]) this.ParseFunctDef
        scanner.PopTerminal CCurly
        scanner.PopEnd()

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
                
                scanner.Pop()
                scanner.PopTerminal SemiCol

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

                scanner.PopTerminals [OSquare;CSquare]    
                let i = scanner.PopIdentifier()
                scanner.PopTerminals [Assign;New]
                let typ2 = this.ParseType()
                scanner.PopTerminal OSquare
                let n = scanner.PopNumber()
                scanner.PopTerminals [CSquare;SemiCol]
                
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

        scanner.PopTerminals [VoidT;Main;OParen;CParen;OCurly]
        let decls = kleeneClosure (scanner.LookaheadTerminals [IntT;CharT]) (fun () -> this.ParseDecl(LocalVariable))
        let stList = this.ParseStList()
        scanner.PopTerminal CCurly
        
        ruleLogger.Pop()

        printFunctionDebug "main"
        variables.PopAndResolveScope()

        Ast.MainFunction(FunctionBody(decls, stList))

    /// "< funct_def > −−> < type > i ( < par list > ){{< decl >} < st list > }"
    member this.ParseFunctDef() =
        ruleLogger.Push "< funct_def > −−> < type > i ( < par list > ){{< decl >} < st list > }"

        variables.PushScope()

        let typ = this.ParseType()
        let i = scanner.PopIdentifier()
        scanner.PopTerminal OParen
        let parList = this.ParseParList()
        let parameterTypes = List.map (fun (p : Parameter) -> p.Attributes.Type, p.Attributes.Definition :> Token) parList

        let attributes = {
            Definition=i;
            ReturnType = Primitive(typ);
            ParameterTypes = parameterTypes;
            Index = functions.CountDefined() + 1;
         }
        functions.Define i attributes
                
        scanner.PopTerminals [CParen;OCurly]
        let decls = kleeneClosure (scanner.LookaheadTerminals [IntT;CharT]) (fun () -> this.ParseDecl LocalVariable)
        let stList = this.ParseStList()
        scanner.PopTerminal CCurly

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
                let i = scanner.PopIdentifier()
                
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
                        scanner.Pop();
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

                scanner.Pop()
                scanner.PopTerminal CSquare
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
                    scanner.Pop()
                    Ast.IntType
                | Terminal CharT ->
                    ruleLogger.Push "<type> --> char"
                    scanner.Pop()
                    Ast.CharType
                | _ ->
                    raise <| CompilerException(
                        [
                        sprintf "Unexpected token \"%A\"" scanner.Current, 
                        scanner.Current.StartLocation])

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

                scanner.Pop()
                let stP = this.ParseStP i

                ruleLogger.Pop()

                stP
            
            | Terminal If ->
                ruleLogger.Push  "<st> −−> if <lexp> <stmt> else <stmt>"   

                scanner.Pop()
                let lExp = this.ParseLExp()
                let stIf = this.ParseSt()
                scanner.PopTerminal Else
                let stElse = this.ParseSt()

                ruleLogger.Pop()

                Ast.IfElse(lExp, stIf, stElse)
            
            | Terminal While ->
                ruleLogger.Push "<st> −−> while <l_exp><st>"   
                scanner.Pop()
                let lExp = this.ParseLExp()
                let body = this.ParseSt()

                ruleLogger.Pop()

                Ast.WhileStatement(lExp, body)
            
            | Terminal Return ->
                ruleLogger.Push "<st> −−> return <exp>;"
                
                scanner.Pop()
                let exp = this.ParseExp()
                scanner.PopTerminal SemiCol

                ruleLogger.Pop()

                Ast.ReturnStatement exp
            
            | Terminal System ->
                ruleLogger.Push  "<st> −−> System.out. (<v_list>);"

                scanner.Pop()
                scanner.PopTerminals [Period;Out;OParen]
                let vList = this.ParseVList()
                scanner.PopTerminals [CParen;SemiCol]

                ruleLogger.Pop()

                Ast.SystemOutInvocation vList
            
            | Terminal SemiCol ->
                ruleLogger.Push "<st> −−> ;"
                
                scanner.Pop()

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

                scanner.PopTerminal OParen
                let vList = this.ParseVList()
                scanner.PopTerminals [CParen;SemiCol]
                
                ruleLogger.Pop()

                let funcId : FunctionIdentifier = {Token=i;Attributes=None}
                functions.Reference i funcId
                Ast.MethodInvocationStatement(funcId, vList)

    /// "<comp_st> −−> { <st_list> }"
    member this.ParseCompSt() =
        ruleLogger.Push "<comp_st> −−> { <st_list> }"
        
        scanner.PopTerminal OCurly
        let stList = this.ParseStList()
        scanner.PopTerminal CCurly
        
        ruleLogger.Pop()
        
        Ast.Block stList

    /// "<set> −−> <st> { <st> }"
    member this.ParseStList() =
        ruleLogger.Push "<set> −−> <st> { <st> }"
        let st = this.ParseSt()
        let rest = kleeneClosure (fun () ->  
            (scanner.terminals [OParen;If;Else;While;Return;System;SemiCol]).IsSome 
                || scanner.Current :? Identifier) this.ParseSt
        
        ruleLogger.Pop()

        st :: rest

    /// "<set> −−> <st> { <st> }"
    member this.ParseAsgSt (i : Identifier) =
        ruleLogger.Push "<set> −−> <st> { <st> }"
        let var = this.ParseVar i
        scanner.PopTerminal Assign
        let asgStP = this.ParseAsgStP var
        scanner.PopTerminal SemiCol

        ruleLogger.Pop()

        Ast.AssignmentStatement asgStP

    /// "<var> −−> i <index>"
    member this.ParseVar() =
        this.ParseVar(scanner.PopIdentifier())

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

                scanner.Pop()
                scanner.PopTerminals [Period;In;Period]
                let typ = this.ParseType()
                scanner.PopTerminals [OParen;CParen]

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

                scanner.Pop()
                let exp = this.ParseExp()
                scanner.PopTerminal CSquare

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
                scanner.Pop()
                let logExpRight = this.ParseLExp()
                Ast.LogicalExpression(logExpLeft, Ast.OrOp, logExpRight)
            | _ ->
                logExpLeft

    member this.ParseAnd() =
        ruleLogger.Push "<l_exp’> --> <exp> { && <l_exp'> }"
        let logLeft = Ast.SingletonLogicalExpression(this.ParseRelExp())
        match scanner.Current with
            | Terminal And ->
                scanner.Pop()
                let logRight = this.ParseAnd()
                ruleLogger.Pop()
                Ast.LogicalExpression(logLeft, Ast.AndOp, logRight)
            | _ ->
                ruleLogger.Pop()
                logLeft

    /// "<rel_exp> --> ( < exp >< rel op >< exp > )"
    member this.ParseRelExp() : Ast.RelativeExpression =
        scanner.PopTerminal OParen
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
        
        scanner.Pop()
        let expRight = this.ParseExp()

        scanner.PopTerminal CParen

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

                scanner.Pop()
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

                scanner.Pop()
                let term = this.ParseTerm()
                let expP = this.ParseExpP()
                
                ruleLogger.Pop()
                
                match token with
                    | Terminal Add -> 
                        Some <| Ast.ExpressionPrime(AddOp, term, expP)
                    | Terminal Sub -> 
                        Some <| Ast.ExpressionPrime(SubOp, term, expP)
                    | _ -> raise <| Exception("Programming error")
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
                    scanner.Pop()
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
                
                scanner.Pop()
                let ast = this.ParsePrimP i

                ruleLogger.Pop()

                ast
            
            | Number n ->
                ruleLogger.Push "<prim> --> n"
                ruleLogger.Pop()

                scanner.Pop()
                Ast.NumberPrimitive n
            
            | CharConst cc ->
                ruleLogger.Push "<prim> --> 'c'"
                ruleLogger.Pop()

                scanner.Pop()
                Ast.CharPrimitive cc
            
            | Terminal OParen ->
                ruleLogger.Push "<prim> --> (<exp>)"

                scanner.Pop()
                let exp = this.ParseExp()
                scanner.PopTerminal CParen

                ruleLogger.Pop()

                Ast.ParenPrimitive(exp)
            
            | _ ->
                raiseUnexpected()

    /// "<prim'> --> (<v_list>) | <index>"
    member this.ParsePrimP i =
        match scanner.Current with
            
            | Terminal OParen ->
                ruleLogger.Push "<prim'> --> (<v_list>)"

                scanner.Pop()
                let vList = this.ParseVList()
                scanner.PopTerminal CParen

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
                scanner.Pop()
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

                scanner.Pop()
                
                let index = this.ParseIndex()

                ruleLogger.Pop()

                let varId = {Token=i;Attributes=None}
                variables.Reference i varId

                Ast.VariableElement(Ast.VariableReference(varId, index))

            | CharConst cc ->
                ruleLogger.Push "<elem> --> 'c'"
                ruleLogger.Pop()

                scanner.Pop()
                Ast.CharConstElement cc

            | Number n ->
                ruleLogger.Push "<elem> --> n"
                ruleLogger.Pop()
                
                ruleLogger.Pop()
                scanner.Pop()
                Ast.NumberElement n

            | _ ->
                raiseUnexpected()