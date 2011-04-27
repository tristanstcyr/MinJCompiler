module Main
open TestFramework
open System

RunAllTests(typeof<MinJ.Scanner.Tests.Performance.Tests>)
ignore(Console.Read())