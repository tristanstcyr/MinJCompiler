(* Abstract syntax tree of the MinJ grammar. 
   These nodes do not map 1:1 with the grammar rules to simplify compilation 
   and type checking later one 
*)
namespace MinJ.Ast

open MinJ.Tokens
open Scanner

type PrimitiveType = IntType | CharType
type MinJType =
    | Primitive of PrimitiveType
    | ArrayType of PrimitiveType

type VariableScope = 
    | GlobalVariable 
    | ParameterVariable 
    | LocalVariable

type VariableAttributes = {
    Definition : Identifier;
    Type : MinJType;
    Scope : VariableScope;
    mutable MemoryAddress : int;
}

type FunctionAttributes = {
    Definition : Identifier;
    ReturnType : MinJType;
    ParameterTypes : (MinJType * Token) list;
    Index : int;
}

type FunctionIdentifier = {
    Token : Identifier;
    mutable Attributes : FunctionAttributes option;
}

type VariableIdentifier = {
    Token : Identifier;
    mutable Attributes : VariableAttributes option;
}

type Program =  
    | Program of VariableDeclaration list * MainFunction * FunctionDefinition list

and VariableDeclaration = 
    | NonArrayVariableDeclaration of VariableIdentifier
    | ArrayVariableDeclaration of VariableIdentifier * PrimitiveType * int

and FunctionBody = | FunctionBody of VariableDeclaration list * Statement list

and MainFunction = 
    | MainFunction of FunctionBody

and FunctionDefinition =
    | FunctionDefinition of FunctionIdentifier * Parameter list * FunctionBody 

and Parameter = | Parameter of VariableIdentifier
    with member this.Attributes with get() = match this with | Parameter(id) -> id.Attributes.Value

and Statement = 
    | Block of Statement list 
    | AssignmentStatement of Assignment
    | IfElse of LogicalExpression * Statement * Statement
    | WhileStatement of LogicalExpression * Statement
    | ReturnStatement of Expression
    | MethodInvocationStatement of FunctionIdentifier * Element list
    | SystemOutInvocation of Element list
    | EmptyStatement

and Assignment = 
    | ExpressionAssignment of VariableReference * Expression
    | SystemInAssignment of VariableReference * PrimitiveType

/// A non-null expression indicates array access
and VariableReference = | VariableReference of VariableIdentifier * Expression option

and LogicalOperator = | OrOp | AndOp

and LogicalExpression =
    | LogicalRelativeExpression of RelativeExpression
    | LogicalExpression of RelativeExpression * LogicalOperator * LogicalExpression

and RelOperator = Lt | Gt | Eq | LtEq | GtEq | Not | NotEq

and RelativeExpression = 
    | RelativeExpression of Expression * RelOperator * Expression

and Expression =
    | Expression of bool * Term * ExpressionPrime option

and ExpressionOp = AddOp | SubOp

and ExpressionPrime = 
    | ExpressionPrime of ExpressionOp * Term * ExpressionPrime option

and Term = | Term of Primitive * TermP option

and TermOp = MulOp | DivOp | ModOp
and TermP = | TermP of TermOp * Primitive * TermP option

and Primitive =
    | VariablePrimitive of VariableReference
    | NumberPrimitive of Number
    | CharPrimitive of CharConst
    | ParenPrimitive of Expression
    | MethodInvocationPrimitive of FunctionIdentifier * Element list

and Element =
    | VariableElement of VariableReference
    | NumberElement of Number
    | CharConstElement of CharConst