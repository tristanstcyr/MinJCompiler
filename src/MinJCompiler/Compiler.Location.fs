namespace Compiler

/// Defines a location in a file
type Location = {
    /// The line number
    Row: int;
    Col: int;
} with
    static member origin = {Row=1;Col=1;}
    member this.advanceRow = {Row = this.Row + 1; Col = 1}
    member this.advanceCol = {this with Col = this.Col + 1}