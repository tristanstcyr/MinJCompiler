[<AutoOpen>]
module Compiler.Tac.ToStream

open System.IO

type Instruction with
    static member ToStream (writer : TextWriter) instruction =
        match instruction with
            | Labeled(lbl) ->
                writer.Write(lbl.ToString())
            | _  ->
                writer.Write("\t")
                writer.WriteLine(instruction.ToString())

type Program with
    static member ToStream (writer : TextWriter) (Program(instructions, frameSizes, constants, globalSize)) =
        Seq.iter (Instruction.ToStream writer) instructions
        writer.Write('f')
        for frameSize in frameSizes do
            writer.Write('\t')
            writer.WriteLine(frameSize)
        writer.WriteLine(sprintf "Size\t%i" globalSize)
        writer.WriteLine("ST")