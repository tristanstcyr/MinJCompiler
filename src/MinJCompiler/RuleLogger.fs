namespace MinJ

open System.IO

/// Helper class for logging rules to a stream.
type RuleLogger(writer : StreamWriter) =
    let mutable depth = 0

    /// Starts a new rule and increases the output index.
    member this.Push (description : string) =
        for i in 0..(depth - 1) do
            writer.Write " "
        writer.WriteLine description
        depth <- depth + 1

    /// Stops the current rule and decreases the output indent.
    member this.Pop() =
        depth <- depth - 1

