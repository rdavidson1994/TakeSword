using System;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections.Generic;

namespace TakeSword
{
    public static class UtilityFunctions
    {
        public static FormattableString FormatConcat(FormattableString first, FormattableString second)
        {
            object[] newArguments = first.GetArguments().Concat(second.GetArguments()).ToArray();
            return FormattableStringFactory.Create(first.Format + second.Format, newArguments);
        }

        public static T RandomChoice<T>(this IEnumerable<T> enumerable)
        {
            var randgen = new Random();
            T output = enumerable.ElementAt(randgen.Next(0, enumerable.Count()));
            return output;
        }
    }
}