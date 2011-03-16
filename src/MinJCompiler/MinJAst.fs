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

type VariableAttributes = {
    Name : string;
    Type : MinJType;
}

type FunctionAttributes = {
    Name : string;
    ReturnType : MinJType;
    ParameterTypes : MinJType list;
}

type FunctionIdentifier = | FunctionIdentifier of FunctionAttributes option ref
    with member this.Attributes with get() = match this with FunctionIdentifier(attributes) -> attributes.Value.Value
type VariableIdentifier = | VariableIdentifier of VariableAttributes option ref
    with member this.Attributes with get() = match this with VariableIdentifier(attributes) -> attributes.Value.Value

type Program =  
    | Program of VariableDeclaration list * MainFunction * FunctionDefinition list

and VariableDeclaration = 
    | NonArrayVariableDeclaration of VariableIdentifier
    | ArrayVariableDeclaration of VariableIdentifier * PrimitiveType * int64

and MainFunction = 
    | MainFunction of VariableDeclaration list * Statement list

and FunctionDefinition = 
    | FunctionDefinition of PrimitiveType * Identifier * Parameter list 
        * VariableDeclaration list * Statement list

and Parameter = | Parameter of VariableIdentifier
    with member this.Attributes with get() = match this with | Parameter(id) -> id.Attributes

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
    | Expression of Term * ExpressionPrime option
    | Negation of Term * ExpressionPrime option

and ExpressionPrime =
    | AdditionExpP of Term * ExpressionPrime option
    | SubstractionExpP of Term * ExpressionPrime option

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