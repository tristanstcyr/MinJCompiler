namespace MinJ

open System.IO

type RuleLogger(writer : StreamWriter) =
    let mutable depth = 0

    member this.Push (description : string) =
        for i in 0..(depth - 1) do
            writer.Write " "
        writer.WriteLine description
        depth <- depth + 1

    member this.Pop() =
        depth <- depth - 1

