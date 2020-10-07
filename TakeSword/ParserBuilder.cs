using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TakeSword
{
    public interface IParser
    {
        Dictionary<string, string>? Match(string input);
    }
    public class ParserBuilder
    {
        private class Parser : IParser
        {
            private readonly Regex regex;
            public Parser(Regex regex)
            {
                this.regex = regex;
            }
            public Dictionary<string, string>? Match(string input)
            {
                var match = regex.Match(input);
                if (!match.Success)
                {
                    return null;
                }
                var lookupTable = new Dictionary<string, string>();
                foreach (string name in regex.GetGroupNames())
                {
                    if (int.TryParse(name, out int _))
                    {
                        //Unamed groups received automatic numeric names in C#
                        //We don't want these, so skip them.
                        continue;
                    }
                    lookupTable[name] = match.Groups[name].Value;
                }
                return lookupTable;
            }
        }
        private Dictionary<string, string> macros = new Dictionary<string, string>();

        //AddMacro is a fluent API
        public ParserBuilder AddMacro(string macro, params string[] synonyms)
        {
            if (!macro.All(char.IsUpper))
            {
                throw new InvalidOperationException($"Macro name '{macro}' should be UPPERCASE.");
            }
            string synonymGroup = "(" + string.Join("|", synonyms) + ")";
            macros[macro] = synonymGroup;
            return this;
        }
        public IParser CreateParser(string template)
        {
            List<string> regexWords = new List<string>();
            string[] words = template.Split();
            foreach (string word in words)
            {
                string regexWord;
                if (macros.TryGetValue(word, out string? foundString))
                {
                    //Translate macros
                    regexWord = foundString;
                }
                else if (word.All(char.IsUpper))
                {
                    //Treat other all caps words as named capture groups
                    regexWord = @"(?<" + word + @">.*)";
                }
                else
                {
                    //Treat all other words as literals
                    regexWord = word;
                }
                regexWords.Add(regexWord);
            }
            string regexString = "^" + string.Join(@"\s+", regexWords) + "$";
            Regex regex = new Regex(regexString, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return new Parser(regex);
        }
    }
}