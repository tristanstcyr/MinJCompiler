namespace MinJ.Ast

open MinJ.Tokens
open Scanner

type PrimitiveType = IntType | CharType
type MinJType =
    | Primitive of PrimitiveType
    | ArrayType of PrimitiveType

type VariableAttributes = {
    Type : MinJType;
}

type FunctionAttributes = {
    ReturnType : MinJType;
    ParameterTypes : MinJType list;
}

type FunctionIdentifier = | FunctionIdentifier of Identifier * FunctionAttributes option ref
    with member this.Type with get() = match this with FunctionIdentifier(id, attributes) -> attributes.Value
type VariableIdentifier = | VariableIdentifier of Identifier * VariableAttributes option ref
    with member this.Type with get() = match this with VariableIdentifier(id, attributes) -> attributes.Value

type Program =  
    | Program of Declaration list * MainFunction * FunctionDefinition list

and Declaration = 
        | VariableDeclaration of VariableIdentifier 
        | ArrayDeclaration of VariableIdentifier * PrimitiveType * Number

and MainFunction = 
    | MainFunction of Declaration list * Statement list

and FunctionDefinition = 
    | FunctionDefinition of PrimitiveType * Identifier * Parameter list 
        * Declaration list * Statement list

and Parameter = | Parameter of VariableIdentifier
    with member this.Type with get() = match this with | Parameter(id) -> id.Type

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

and VariableReference =
    | SimpleReference of VariableIdentifier
    | ArrayAccess of VariableIdentifier * Expression

and LogicalExpression =
    | LogicalRelativeExpression of RelativeExpression
    | AndExpression of RelativeExpression * LogicalExpression
    | OrExpression of RelativeExpression * LogicalExpression

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
and TermP = TermP of TermOp * Primitive * TermP option

and Primitive =
    | VariablePrimitive of VariableReference
    | NumberPrimitive of Number
    | CharPrimitive of CharConst
    | ParenPrimitive of Expression
    | MethodInvocationPrimitive of FunctionIdentifier * Element list

and Element =
    | VariableElement of VariableIdentifier * Expression option
    | NumberElement of Number
    | CharConstElement of CharConst