module MinJ.Parser

open System
open System.IO
open System.Collections.Generic

open Scanner
open MinJ.Scanner
open MinJ.Tokens
open MinJ
open MinJ.Ast

/// The MinJ Parser.
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
        kleeneClosure []
          
    let mutable symbolTable = SymbolTable()

    let raiseUnexpected() =
        match scanner.Current with
            | :? End -> raise <| UnexpectedEnd
            | _ -> raise <| UnexpectedToken scanner.Current

    let printFunctionDebug funcId =
        // Output function debug information
        debugOutput.WriteLine(sprintf "Symbol table after parsing %A" funcId)
        symbolTable.PrintContent debugOutput
        debugOutput.WriteLine()

    member this.Init() =
        (* Initiate the scanner *)
        scanner.MoveNext() |> ignore
        (* Check if the first token is an error *)
        scanner.checkError()

    member this.Parse() =
        let ast, errors = 
            try
                this.Init()
                let prg = this.ParsePrg()
                Some prg, []
            with
                | e -> None, [e]

        ast, symbolTable.Errors @ errors

    member this.ParsePrg() =
        ruleLogger.Push "< prg > −− > class i {{< decl >} < main f > {< f unct def >}}"

        scanner.PopTerminal Class
        let i = scanner.PopIdentifier()
        scanner.PopTerminal OCurly
        let decls = kleeneClosure (scanner.LookaheadTerminals [IntT;CharT]) (fun () -> this.ParseDecl true)
        let mainF = this.ParseMainF()
        let funcDefs = kleeneClosure (scanner.LookaheadTerminals [IntT;CharT]) this.ParseFunctDef
        scanner.PopTerminal CCurly
        scanner.PopEnd()

        ruleLogger.Pop()

        symbolTable.ClearAndResolveFunctions()
        Ast.Program(decls, mainF, funcDefs)
    
    member this.ParseDecl (isField : bool) : Ast.Declaration =
        ruleLogger.Push "< decl > −−> < type > <decl'>"
        
        let typ = this.ParseType()
        let declP = this.ParseDeclP typ isField

        ruleLogger.Pop()

        declP

    member this.ParseDeclP typ isField =
        
        match scanner.Current with
            | Identifier i ->
                ruleLogger.Push "<decl’> --> i"
                
                scanner.Pop()
                scanner.PopTerminal SemiCol

                ruleLogger.Pop()

                let attr = {Type=Primitive(typ)}
                let varAttributes = VariableIdentifier(i, ref <| Some attr)
                if isField then
                    symbolTable.DefineField varAttributes
                else
                    symbolTable.DefineLocal i attr
                Ast.VariableDeclaration(varAttributes)

            | _ ->
                ruleLogger.Push "<decl'> --> [] i = new <type> [ n ];"

                scanner.PopTerminals [OSquare;CSquare]    
                let i = scanner.PopIdentifier()
                scanner.PopTerminals [Assign;New]
                let typ2 = this.ParseType()
                scanner.PopTerminal OSquare
                let n = scanner.PopNumber()
                scanner.PopTerminals [CSquare;SemiCol]

                let varAttributes = VariableIdentifier(i, ref <| Some {Type=ArrayType(typ)})
                symbolTable.DefineField varAttributes

                ruleLogger.Pop()

                Ast.ArrayDeclaration(varAttributes, typ, n)

    member this.ParseMainF() =
        ruleLogger.Push  "<main_f> −−> void main(){{< decl >} < st list > }"

        scanner.PopTerminals [VoidT;Main;OParen;CParen;OCurly]
        let decls = kleeneClosure (scanner.LookaheadTerminals [IntT;CharT]) (fun () -> this.ParseDecl false)
        let stList = this.ParseStList()
        scanner.PopTerminal CCurly
        
        printFunctionDebug "main"
        symbolTable.ClearAndResolveLocals()

        ruleLogger.Pop()

        Ast.MainFunction(decls, stList)

    member this.ParseFunctDef() =
        ruleLogger.Push "< funct_def > −−> < type > i ( < par list > ){{< decl >} < st list > }"

        let typ = this.ParseType()
        let i = scanner.PopIdentifier()
        scanner.PopTerminal OParen
        let parList = this.ParseParList()
        let parameterTypes = List.map (fun (p : Parameter) -> p.Type.Value.Type) parList

        symbolTable.DefineFunction i {
            ReturnType = Primitive(typ);
            ParameterTypes = parameterTypes
        }
                
        scanner.PopTerminals [CParen;OCurly]
        let decls = kleeneClosure (scanner.LookaheadTerminals [IntT;CharT]) (fun () -> this.ParseDecl false)
        let stList = this.ParseStList()
        scanner.PopTerminal CCurly

        printFunctionDebug i
        // Pop var scope
        symbolTable.ClearAndResolveLocals()

        ruleLogger.Pop()

        Ast.FunctionDefinition(typ, i, parList, decls, stList)

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
                let p = Ast.Parameter(VariableIdentifier(i, ref <| Some {Type=typ}))

                symbolTable.DefineLocal i {Type=typ}

                match scanner.Current with
                    | Terminal Comma -> 
                        scanner.Pop();
                        p :: this.ParseParList()
                    | _ -> [p]
    
    member this.ParsePType() =
        ruleLogger.Push "<p_type> --> <type> <p_type’>"

        let pTyp = this.ParsePTypeP(this.ParseType())

        ruleLogger.Pop()

        pTyp
        
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
                    raise <| UnexpectedToken scanner.Current

        ruleLogger.Pop()

        pTyp

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
                ruleLogger.Push  "<st> −−> i <st'>"   

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

                let funcId = FunctionIdentifier(i, ref None)
                symbolTable.ReferenceFunction funcId
                Ast.MethodInvocationStatement(funcId, vList)

    member this.ParseCompSt() =
        ruleLogger.Push "<comp_st> −−> { <st_list> }"
        
        scanner.PopTerminal OCurly
        let stList = this.ParseStList()
        scanner.PopTerminal CCurly
        
        ruleLogger.Pop()
        
        Ast.Block stList

    member this.ParseStList() =
        ruleLogger.Push "<set> −−> <st> { <st> }"
        let st = this.ParseSt()
        let rest = kleeneClosure (fun () ->  
            (scanner.terminals [OParen;If;Else;While;Return;System;SemiCol]).IsSome || scanner.Current :? Identifier) this.ParseSt
        
        ruleLogger.Pop()

        st :: rest

    member this.ParseAsgSt (i : Identifier) =
        ruleLogger.Push "<set> −−> <st> { <st> }"
        let var = this.ParseVar i
        scanner.PopTerminal Assign
        let asgStP = this.ParseAsgStP var
        scanner.PopTerminal SemiCol

        ruleLogger.Pop()

        Ast.AssignmentStatement asgStP

    member this.ParseVar() =
        this.ParseVar <| scanner.PopIdentifier()

    member this.ParseVar i =
        ruleLogger.Push "<var> −−> i <index>"

        let index = this.ParseIndex()
        
        ruleLogger.Pop()
        
        let varId = VariableIdentifier(i, ref None)
        match index with
            | Some(exp) ->
                symbolTable.AddVariableReference varId
                Ast.ArrayAccess(varId, exp)
            | None ->
                symbolTable.AddVariableReference varId
                Ast.SimpleReference(varId)
        
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

    member this.ParseLExp() =
        ruleLogger.Push "<l_exp> --> <rel_exp> <l_exp’>"

        let lExp = this.ParseLExpP <| this.ParseRelExp()

        ruleLogger.Pop()

        lExp

    member this.ParseLExpP relExp =
        match scanner.Current with
            | Terminal And ->
                ruleLogger.Push "<l_exp’> --> <log_op> <l_exp>"
                
                scanner.Pop()
                let lExp = this.ParseLExp()

                ruleLogger.Pop()

                Ast.AndExpression(relExp, lExp)
            
            | Terminal Or ->
                ruleLogger.Push "<l_exp’> --> <log_op> <l_exp>"

                scanner.Pop()
                let lExp = this.ParseLExp()

                ruleLogger.Pop()

                Ast.OrExpression(relExp, this.ParseLExp())
            
            | _ ->
                ruleLogger.Push "<l_exp’> --> e"
                ruleLogger.Pop()

                Ast.LogicalRelativeExpression relExp

    member this.ParseRelExp() : Ast.RelativeExpression =
        scanner.PopTerminal OParen
        let expLeft = this.ParseExp()

        ruleLogger.Push "<rel_exp> --> ( < exp >< rel op >< exp > )>"

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

    member this.ParseExp() =
        match scanner.Current with
            | Terminal OParen
            | Identifier _
            | Number _
            | CharConst _ ->
                ruleLogger.Push "<exp> --> <term><exp'>"
                
                let exp = Ast.Expression(this.ParseTerm(), this.ParseExpP())

                ruleLogger.Pop()

                exp

            | Terminal Sub ->
                ruleLogger.Push "<exp> --> -<term><exp'>"

                scanner.Pop()
                let neg = Ast.Negation(this.ParseTerm(), this.ParseExpP())

                ruleLogger.Pop()

                neg

            | _ ->
                raiseUnexpected()

    member this.ParseExpP() =
        match scanner.Current with
            | Terminal Add ->
                ruleLogger.Push "<exp'> --> <add_op><term><exp'>"

                scanner.Pop()
                let term = this.ParseTerm()
                let expP = this.ParseExpP()
                
                ruleLogger.Pop()
                
                Some <| Ast.AdditionExpP(term, expP)
            | Terminal Sub ->
                ruleLogger.Push "<exp'> --> <add_op><term><exp'>"

                scanner.Pop()
                let term = this.ParseTerm()
                let expP = this.ParseExpP()

                ruleLogger.Pop()

                Some <| Ast.AdditionExpP(term, expP)
            | _ ->
                ruleLogger.Push "<exp'> --> e"
                ruleLogger.Pop()

                None

    member this.ParseTerm() =
        ruleLogger.Push "<term> --> <prim><term'>"

        let prim = this.ParsePrim()
        let termP = this.ParseTermP()

        ruleLogger.Pop()

        Ast.Term(prim, termP)
        
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

    member this.ParsePrimP i =
        match scanner.Current with
            
            | Terminal OParen ->
                ruleLogger.Push "<prim'> --> (<v_list>)"

                scanner.Pop()
                let vList = this.ParseVList()
                scanner.PopTerminal CParen

                ruleLogger.Pop()

                let funcId = FunctionIdentifier(i, ref None)
                symbolTable.ReferenceFunction funcId
                Ast.MethodInvocationPrimitive(funcId, vList)
            
            | _ ->
                ruleLogger.Push "<prim'> --> <index>"

                let index = this.ParseIndex()

                ruleLogger.Pop()

                let varId = VariableIdentifier(i, ref None)
                symbolTable.AddVariableReference varId
                Ast.VariablePrimitive(Ast.VariableReference(varId), index)

    member this.ParseVList() =
        ruleLogger.Push "<v_list> --> <elem><v_list'>"
        
        let elem = this.ParseElem()
        
        ruleLogger.Pop()

        match scanner.Current with
            
            | Terminal Comma ->
                ruleLogger.Push "<v_list'> --> ,<v_list'>"
                
                let ast = elem :: this.ParseVList()
                
                ruleLogger.Pop()
                
                ast

            | _ ->
                ruleLogger.Push "<v_list'> --> e"
                ruleLogger.Pop()

                [elem]

    member this.ParseElem() =
        match scanner.Current with
            | Identifier i ->
                ruleLogger.Push "<elem> --> i <index>"

                scanner.Pop()
                
                let index = this.ParseIndex()

                ruleLogger.Pop()

                let varId = VariableIdentifier(i, ref None)
                symbolTable.AddVariableReference varId

                Ast.VariableElement(varId, index)

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

/// Convenience function for calling the parser. This is equivalent to 
/// Parser(tokens).ParsePrg().Value
let parse tokens = Parser(tokens).ParsePrg()