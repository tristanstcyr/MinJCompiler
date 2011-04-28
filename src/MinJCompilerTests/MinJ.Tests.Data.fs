module MinJ.Tests.Data

let mainText = "
    void main() 
    {
		int result;
		System.out('F','i','b','(','2','5',')',':');
		System.out('\n');
		if (result == 75025)
		{
			System.out('P','A','S','S','\n');
		}
		else
		{
			System.out('F','A','I','L','\n');
		}
    }
"

let makeFunction name = " int " + name + "
    (int i)
	{
		int a;
		int b;
		if (i == 0)
		{
			return i;
		}
		else
		{
			if (i == 1)
			{
				return 0;
			}
			else
			{
				a = i - 1;
				b = i - 2;
				return 0;
			}
		}
	}
"

let makeFunctions baseName count =
    let rec makeFunctions acc id =
        if id < count then
            makeFunctions (acc + makeFunction (baseName+string(id))) (id + 1)
        else
            acc
    makeFunctions "" 0

let makeClass main functions = 
    "// example of a code in MinJ 
    class Average {" + main + functions + "}"