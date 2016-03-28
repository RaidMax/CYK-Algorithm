using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace CSC2710_HW4
{
    class Algorithm
    {
        public struct AlgorithmResult
        {
            public ArrayList[][] Matrix;
            public Boolean FindableExists;
            public Rule StartSymbol;
        }

        /// <summary>
        /// Get an AlgorithmResult that represents the application of the CYK algorithm
        /// to a ruleset and a given search string
        /// </summary>
        /// <param name="toFind">The string to look for in the rule list</param>
        /// <param name="ruleList">List of rules in Chomsky Normal Form</param>
        /// <param name="StartSymbol">The first rule in the rule set</param>
        /// <returns>Algorithm result including the CYK matrix representation and found status</returns>
        public static AlgorithmResult CYKAlgorithm(String toFind, Dictionary<Char, List<Rule>> ruleList, Rule StartSymbol)
        {
            int strLen          = toFind.Length;
            int ruleCount       = ruleList.Keys.Count;
            var matrixTable     = new ArrayList[strLen][];
            var Result          = new AlgorithmResult();

            for (int i = 0; i < strLen; i++)
            {
                // initialize the arraylist of terminals/non-terminals
                matrixTable[i] = new ArrayList[strLen];
                for (int j = 0; j < strLen; j++)
                    matrixTable[i][j] = new ArrayList();
            }

            for (int i = 0; i < strLen; i++)
            {
                // loop through all all the rule endpoints
                foreach (Char Key in ruleList.Keys)
                {
                    List<Rule> Rules = ruleList[Key];
                    // make sure the type of rule only has one terminal
                    foreach (Rule R in Rules)
                        if (R.GetType() == typeof(CaseTwo))
                            if (toFind[i] == R.EndPoints[0])
                                matrixTable[i][i].Add(Key);
                }
            }

            for (int l = 1; l < strLen; l++)
            { 
                for (int r = 0; r < strLen - l; r++)
                {
                    for (int t = 0; t < l; t++)
                    {
                        // set up our subset of  the matrix
                        var L = matrixTable[r][r + t];
                        var R = matrixTable[r + t + 1][r + l];
                        // loop through every rule key
                        foreach (Char Key in ruleList.Keys)
                        {
                            // loop through each rule list corresponding to the key
                            foreach (Rule Rule in ruleList[Key])
                            {
                                // make sure it's the type of rule that has two non-terminals
                                // we also don't want to insert a rule that already exists
                                if (Rule.GetType() == typeof(CaseOne))
                                    if (L.Contains(Rule.EndPoints[0]) && R.Contains(Rule.EndPoints[1]) && !matrixTable[r][r+l].Contains(Key))
                                        matrixTable[r][r + l].Add(Key);
                            }
                        }
                    }
                }
            }

            // if the start symbol is found in the top right of the matrix then the string exists in the cfl
            Result.Matrix           = matrixTable;
            Result.StartSymbol      = StartSymbol;
            Result.FindableExists   = Result.Matrix[0][matrixTable.Length - 1].Contains(StartSymbol.Name);
            return Result;
        }

        /// <summary>
        /// Convert the resulting arraylist from the CYK Algorithm into a readable format
        /// </summary>
        /// <param name="cykMatrix">Resulting matrix from application of CYK Algorithm</param>
        /// <returns>String reprentation of the CYK Algorithm table</returns>
        public static String PrettyPrint(ArrayList[][] cykMatrix)
        {
            // get the longest rule size (just for formatting purposes)
            int maxRuleLen = 0;
            for (int i = 0; i < cykMatrix.Length; i++)
                for (int j = 0; j < cykMatrix.Length; j++)
                    if (String.Join(",", cykMatrix[i][j].ToArray()).Length > maxRuleLen)
                        maxRuleLen = String.Join(",", cykMatrix[i][j].ToArray()).Length;

            StringBuilder Line  = new StringBuilder();
            String Spacer       = "                 ";

            for (int i = 0; i < cykMatrix.Length; i++)
            {
                Line.Append("[");
                for (int j = 0; j < cykMatrix.Length; j++)
                {
                    String ruleConcat = String.Join(",", cykMatrix[i][j].ToArray());
                    // this is a quick and dirty way to make it look good with monospaced font
                    // it also works in more cases than a static format alignment ie: {0,5}
                    String whiteSpace = Spacer.Substring(0, Math.Max(0, maxRuleLen - ruleConcat.Length));
                    Line.AppendFormat(" {{{0}}}", ruleConcat + whiteSpace);
                }
                Line.Append("  ]" + Environment.NewLine);
            }
            return Line.ToString();
        }
    }
}
