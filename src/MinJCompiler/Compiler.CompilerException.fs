namespace Compiler

/// A helpful message describing the error 
/// and the location of the error in the source.
type CompilationError = string * Location
/// Thrown when one or more compilation errors have occured that prevent
/// the compilation process for continuing.
exception CompilerException of CompilationError list
