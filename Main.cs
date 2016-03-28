///////////////////////////////////////////
// Author: Michael Snyder                //
// Class: CSC2710                        //
// Start: 3/22/16                        //
// Purpose: CYK Algorithm Implementation //
///////////////////////////////////////////
using System;

namespace CSC2710_HW4
{
    class Start
    {
        public static void Main(String[] args)
        {
#if DEBUG == false
            if (args.Length == 0)
            {
                Console.WriteLine("usage: <filename>");
                return;
            }

            else
            {
#else
            args = new String[] { "Inputs\\input.txt" };
#endif
            InputParsing Parser     = new InputParsing(args[0]);
            var Result              = Parser.Parse();

            if (Result.Success)
            {
                foreach (String toFind in Result.Findable)
                {
                    Algorithm.AlgorithmResult processedFindable = Algorithm.CYKAlgorithm(toFind, Result.Rules, Result.StartSymbol);

                    if (processedFindable.FindableExists)
                    {
                        Console.WriteLine("Yes");
                        Console.WriteLine(Algorithm.PrettyPrint(processedFindable.Matrix));
                    }
                    else
                        Console.WriteLine("No");               
                }
            }
            else
                Console.WriteLine("Invalid input format.");
#if DEBUG == false
            }
#endif
        }
    }
}
