using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace CSC2710_HW4
{
    class InputParsing
    {
        public struct ParseResult
        {
            public Boolean Success;
            public Dictionary<Char, List<Rule>> Rules;
            public List<Char> Alphabet;
            public List<String> Findable;
            public Rule StartSymbol;
        }

        private String _fileName;
        private String[] _fileLines;
        
        public InputParsing(String fileName)
        {
            // we have to declare these outside because of scoping in the try/catch block below
            _fileName               = fileName;
            FileStream inputStream  = null;
            Byte[] lineBuff         = null;
            int bytesRead           = 0;

            // only catching exceptions here because crashing on a bad filename looks ugly
            try
            {
                inputStream = File.OpenRead(fileName);
                lineBuff = new Byte[inputStream.Length];
                bytesRead = inputStream.Read(lineBuff, 0, lineBuff.Length);
            }

            catch (Exception)
            {
                Console.WriteLine("Error reading input file.");
                Environment.Exit(-1);
            }

            // make sure the whole file was read
            if (bytesRead == inputStream.Length)
            {
                // make sure to get rid of any empty lines
                _fileLines = Regex.Split(Encoding.Default.GetString(lineBuff), Environment.NewLine);
                _fileLines = _fileLines.Where(x => !String.IsNullOrEmpty(x)).ToArray();
            }
            else
                throw new IOException();
        }

        /// <summary>
        /// Parse the contents of a text file given to the class's constructor
        /// </summary>
        /// <returns>A ParseResult including the success or failure of the parsing</returns>
        public ParseResult Parse()
        {
            var Result          = new ParseResult();
            Result.Rules        = new Dictionary<Char, List<Rule>>();
            Result.Alphabet     = new List<Char>();
            Result.Findable     = new List<String>();
            Result.Success      = false;

            // hardcoding indexes here because we are expecting a valid file
            // this would be terrible practice in virtual any other situation
            // no exception handling because we're expecting a valid input file
            foreach (String S in _fileLines[1].Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                Result.Alphabet.Add(S[0]);

            int ruleCount   = int.Parse(_fileLines[2]);
            var rawRules    = new Dictionary<String, String[]>();

            for (int i = 3; i < 3 + ruleCount; i++)
            {
                String ruleName         = _fileLines[i].Split(':')[0];
                String[] ruleEndpoints  = _fileLines[i].Split(':')[1].Trim().Split(',');

                // get rid of some whitespace
                for (int j = 0; j < ruleEndpoints.Length; j++)
                    ruleEndpoints[j] = ruleEndpoints[j].Trim();

                rawRules.Add(ruleName, ruleEndpoints);
            }

            // how many strings to find
            // ignoring parsing exceptions
            int findableCount = Int32.Parse(_fileLines[3 + ruleCount]);
            for (int i = 4 + ruleCount; i < 4 + ruleCount + findableCount; i++)
            {
                Boolean validFindable = true;
                foreach (Char C in _fileLines[i])
                    if (!Result.Alphabet.Contains(C))
                        validFindable = false;

                // add the findable string to our list only if it contains characters in the input alphabet
                if (validFindable)
                    Result.Findable.Add(_fileLines[i]);
                else
                    Console.WriteLine("Skipping findable value with characters outside input alphabet");
            }
               
            foreach (String ruleName in rawRules.Keys)
            {
                Rule newRule = null;
                foreach (String Rule in rawRules[ruleName])
                {
                    if (Rule.Length == 2)
                    {
                        Char firstEndpoint  = Rule[0];
                        Char secondEndpoint = Rule[1];
                        newRule             = new CaseOne(ruleName[0], firstEndpoint, secondEndpoint);
                    }

                    else if (Rule.Length == 1)
                    {
                        if (Rule[0] != '*')
                            newRule = new CaseTwo(ruleName[0], Rule[0]);
                        else
                            newRule = new CaseThree(ruleName[0]);     
                    }

                    if (Result.Rules.ContainsKey(ruleName[0]))
                        Result.Rules[ruleName[0]].Add(newRule);
                    else
                        Result.Rules.Add(ruleName[0], new List<Rule>() { newRule });

                    if (Result.Rules.Count == 1)
                        Result.StartSymbol = newRule;
                }
            }

            Result.Success = true;
            return Result;
        }
    }
}
