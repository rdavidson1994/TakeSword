using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TakeSword
{
    public class GameString
    {
        public GameString(string[] literals, object?[] variables)
        {
            if (literals.Length != variables.Length + 1)
            {
                throw new ArgumentException("literals.Length != variables.Length + 1", nameof(variables));
            }
            Literals = literals;
            Variables = variables;
        }
        public string[] Literals { get; }
        public object?[] Variables { get; }
        public GameString(FormattableString formattableString)
        {
            Literals = Regex.Split(formattableString.Format, @"{\d+}");
            Variables = formattableString.GetArguments();
        }

        public static explicit operator GameString(FormattableString f)
        {
            return new GameString(f);
        }

        public static GameString operator +(GameString first, GameString second)
        {
            IEnumerable<string> firstExceptLast = first.Literals.Take(first.Literals.Length - 1);
            IEnumerable<string> secondExceptFirst = second.Literals.Skip(1);
            string newMiddle = first.Literals[first.Literals.Length - 1] + second.Literals[0];
            string[] newLiterals = firstExceptLast.Append(newMiddle).Concat(secondExceptFirst).ToArray();
            object?[] newVariables = first.Variables.Concat(second.Variables).ToArray();
            return new GameString(newLiterals, newVariables);
        }
    }

    public static class FormattableStringExtensions
    {
        public static GameString ToGameString(this FormattableString fString)
        {
            return new GameString(fString);
        }
    }

    public static class SyntaxTest
    {
        public static void Test ()
        {
            object a = new object();
            var x = new GameString($"hello {a}") + new GameString($"goodbye {a}");
        }
    }
}