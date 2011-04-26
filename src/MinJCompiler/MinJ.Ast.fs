[<AutoOpen>]
module MinJ.Ast

open Compiler
open MinJ

(* 
   Abstract syntax tree of the MinJ grammar. 
   These nodes do not map 1:1 with the grammar rules to simplify compilation 
   and type checking later one 
*)

/// Primitive types.
type PrimitiveType = IntType | CharType
/// Either an array or a simple type
type MinJType =
    | Primitive of PrimitiveType
    | ArrayType of PrimitiveType

/// Used to define the scope of a variable reference.
/// This is used to know which space to address in
/// three address code.
type VariableScope = 
    | GlobalVariable 
    | ParameterVariable 
    | LocalVariable

/// Attributes for variable identifiers.
type VariableAttributes = {
    /// The identifier token of the definition.
    /// This is useful for error reporting.
    Definition : Identifier;
    /// The type of the variable
    Type : MinJType;
    /// The scope in which the variable was declared.
    /// This is useful during intermediate code generation
    /// for determine in which address space the variable can
    /// be addressed.
    Scope : VariableScope;
    /// The address where the variable can be referenced.
    /// It is mutable because it is only determind during 
    /// the intermediate code generation.
    mutable MemoryAddress : uint32;
}

/// Attributes for function identifiers.
type FunctionAttributes = {
    /// The identifier token of the definition.
    /// This is useful for error reporting.
    Definition : Identifier;
    /// The return type of the function.
    ReturnType : MinJType;
    /// The types and identfier tokens of the parameters.
    ParameterTypes : (MinJType * Token) list;
    /// A unique index of the function. The main function
    /// always has an index of 1. All other functions are
    /// assigned an index in the order in which they are encountered.
    Index : int;
}

/// A placeholder for attaching function attributes to an AST node.
type FunctionIdentifier = {
    Token : Identifier;
    /// Attributes for the referenced function
    /// are inserted here once they are resolved.
    mutable Attributes : FunctionAttributes option;
}

/// A placeholder for attaching function attributes to an AST node.
type VariableIdentifier = {
    /// The identifier token
    Token : Identifier;
    /// Attributes for this variables are inserted here
    /// once they are resolved.
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

/// A non-null expression indicates array access.
and VariableReference = | VariableReference of VariableIdentifier * Expression option

and LogicalOperator = | OrOp | AndOp

and LogicalExpression =
    | SingletonLogicalExpression of RelativeExpression
    | LogicalExpression of LogicalExpression * LogicalOperator * LogicalExpression

and RelOperator = Lt | Gt | Eq | LtEq | GtEq | Not | NotEq

and RelativeExpression = 
    | RelativeExpression of Expression * RelOperator * Expression

/// 
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