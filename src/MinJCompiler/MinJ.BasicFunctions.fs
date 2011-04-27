module MinJ.BasicFunctions

open Compiler
open MinJ

open System.IO

let printNumberFunctions = @"
    int _printNumber(int number)
	{
		if (number == 0)
			System.out('0');
		else
			if (number < 0)
			{
				System.out('-');
				number = -number;
			} else ;
			rec_printNumber(number);
		return 0;
	}
	
	int _rec_printNumber(int number)
	{
		int rest;
		int div;
		if (number != 0)
		{
			rest = number % 10;
			div = number / 10;
			rec_printNumber(div);
			rest = rest + 48;
			System.out(rest);
		}
		else ;
		return 0;
	}
    "

let AddBasicFunctions (input : string) =
    let lastBracketIndex = input.LastIndexOf("}")
    input.Insert(lastBracketIndex, printNumberFunctions)