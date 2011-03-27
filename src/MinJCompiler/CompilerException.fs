namespace Compiler
open Scanner.Tokens

type CompilationError = string * Location
exception CompilerException of CompilationError seq
