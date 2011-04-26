[<AutoOpen>]
module Compiler.Utils

open System.IO

/// Opens a file into a sequence of chars.
/// This is where some of the magic of loading characters on demand happens.
let openAsCharSeq path = seq {
    use reader = new StreamReader(File.OpenRead(path))
    while not reader.EndOfStream do
        yield char(reader.Read())
} 