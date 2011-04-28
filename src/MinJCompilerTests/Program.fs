module Main
open TestFramework
open System

RunAllTests(typeof<MinJ.ToTac.Tests.ThreeAddressCodeTranslationTests>)
ignore(Console.Read())