/// Scanner specific to the MinJ language.
[<AutoOpen>]
module MinJ.Scanner

open Scanner
open System.Collections.Generic
open MinJ.ScannerStateMachine

/// Convenice method for creating a MinJScanner.
let createMinJScanner chars listingWriter = 
    new Scanner(createMinJStateMachine(), chars, listingWriter) :> IEnumerator<Token>