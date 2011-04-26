module Main
open TestFramework
open MinJ.ToTac.Tests
open System

RunAllTests(typeof<ThreeAddressCodeTranslationTests>)
ignore(Console.Read())