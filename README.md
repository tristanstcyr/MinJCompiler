F# MinJ Compiler
================

The F# MinJ Compiler is a university project for the Compiler Design class at Concordia University (COMP 442). The compiler's source language is a programming language called MinJ designed by professor [J. Opatrny](http://users.encs.concordia.ca/~opatrny/).

While this is not really a useful product in itself, it could serve as an interesting reference for anyone that wants to explore possible ways of  writing compilers in F#.

### Features
- Written in F#
- Custom lexer and recursive decent parser (i.e. not generated)
- Symbol resolution, type checking and some dead code detection.
- Compilation to intermediate code.
- Compilation to Moon assembly language. Moon is a virtual machine designed by programmer by professor [P. Grogono](http://users.encs.concordia.ca/~grogono/).

### Compilation Requirements
- Visual Studio 2010
- .NET 4.0