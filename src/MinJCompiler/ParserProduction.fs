module Parser.Production
open Scanner
open MinJ

(*
type Program = Declaration option * MainFunction * FunctionDefinition option
and Declaration = 
        | Declaration of TypeReference * identifier 
        | ArrayDeclaration of TypeReference * Identifier * Number 
and MainFunction = Declaration * Statement list
and FunctionDefinition = TypeReference * Identifier * Parameter list * Declaration option * Statement list
and ParameterType =
    | ParameterType of TypeReference
    | ArrayParameterType of TypeReference
and TypeReference = IntType | CharType
and Statement = 
    | Block of Statement list 
    | AssignmentStatement of Assignment
    | IfElse of LogicalExpression * Statement * Statement
    | WhileStatement of LogicalExpression * Statement
    | ReturnStatement of Expression
    | MethodInvocation of Element list
    | SystemOutInvocation of Element list
and Assignment = 
    | ExpressionAssignment of VariableReference * Expression
    | SystemInAssignment of VariableReference * TypeReference
and VariableReference =
    | VariableReference of Identifier
    | ArrayAccess of Identifier * Expression
and LogicalExpression =
    | RelativeExpression of RelativeExpression
    | LogicalExpression of RelativeExpression * LogOp * LogicalExpression
and RelativeExpression = Expression * RelOp * Expression
and Expression =
    | Expression of Term * Addition option
    | Negation of Expression
and Addition = AddOperator * Term * Expression
and Term = Primitive * Multiplication option
and Multiplication = MulOp * Primitive * Addition
and Primitive =
    | ArrayAccess of Identifier * Expression
    | NumberPrimitive of Number
    | CharPrimitive of CharConst
    | ParenPrimitive of Expression
    | MethodInvocation of Identifier 

*)