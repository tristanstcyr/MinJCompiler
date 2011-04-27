[<AutoOpen>]
module Compiler.Tac.ToStream

open System.IO

type Instruction with
    static member ToStream (writer : TextWriter) instruction =
        match instruction with
            | Labeled(lbl) ->
                writer.Write(Label.ToString lbl)
            | _  ->
                writer.Write("\t")
                writer.WriteLine(Instruction.ToString instruction)

type Program with
    static member ToStream (writer : TextWriter) (Program(instructions, frameSizes, constants, globalSize) as program) =
        Seq.iter (Instruction.ToStream writer) instructions
        writer.Write('f')
        for frameSize in frameSizes do
            writer.Write('\t')
            writer.WriteLine(frameSize)
        writer.WriteLine(sprintf "Size\t%i" globalSize)
        writer.WriteLine("ST")
        program