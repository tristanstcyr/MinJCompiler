namespace MinJ

open System.IO

type IRuleLogger =
    abstract member Push : string -> unit
    abstract member Pop : unit -> unit

/// Helper class for logging rules to a stream.
type RuleLogger(writer : StreamWriter) =
    let mutable depth = 0

    interface IRuleLogger with
        /// Starts a new rule and increases the output index.
        member this.Push (description : string) =
            for i in 0..(depth - 1) do
                writer.Write " "
            writer.WriteLine description
            depth <- depth + 1

        /// Stops the current rule and decreases the output indent.
        member this.Pop() =
            depth <- depth - 1

type NullRuleLogger() =
    interface IRuleLogger with
        override this.Push s = ()
        override this.Pop() = ()