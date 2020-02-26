using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TakeSword
{
    public class GameString
    {
        public GameString(params object[] inputs)
        {
            // Should make this a factory, less logic in constructor
            if (inputs.Length % 2 == 0)
            {
                throw new ArgumentException(
                    "inputs must be an interspersed sequence " +
                    "of objects and strings, beginning and ending with strings.",
                    "inputs"
                );
            }
            IEnumerable<object> evenEntries = inputs.Where((_, i) => i % 2 == 0);
            IEnumerable<object> oddEntries = inputs.Where((_, i) => i % 2 != 0);
            Variables = oddEntries.ToArray();
            try
            {
                Literals = evenEntries.Cast<string>().ToArray();
            }
            catch (InvalidCastException ex) {
                throw new ArgumentException("Even entries of the inputs list must be strings", "inputs", ex);
            }
        }
        public GameString(string[] literals, object[] variables)
        {
            if (literals.Length != variables.Length + 1)
            {
                throw new ArgumentException("literals.Length != variables.Length + 1", "variables");
            }
            Literals = literals;
            Variables = variables;
        }
        public string[] Literals { get; }
        public object[] Variables { get; }
        public GameString(FormattableString formattableString)
        {
            Literals = Regex.Split(formattableString.Format, @"{\d+}");
            Variables = formattableString.GetArguments();
        }

        public static GameString operator +(GameString first, GameString second)
        {
            IEnumerable<string> firstExceptLast = first.Literals.Take(first.Literals.Length - 1);
            IEnumerable<string> secondExceptFirst = second.Literals.Skip(1);
            string newMiddle = first.Literals[first.Literals.Length - 1] + second.Literals[0];
            string[] newLiterals = firstExceptLast.Append(newMiddle).Concat(secondExceptFirst).ToArray();
            object[] newVariables = first.Variables.Concat(second.Variables).ToArray();
            return new GameString(newLiterals, newVariables);
        }
    }
}