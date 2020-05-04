using System;
using System.Runtime.CompilerServices;
using System.Linq;

namespace TakeSword
{
    public class UtilityFunctions
    {
        public static FormattableString FormatConcat(FormattableString first, FormattableString second)
        {
            object[] newArguments = first.GetArguments().Concat(second.GetArguments()).ToArray();
            return FormattableStringFactory.Create(first.Format + second.Format, newArguments);
        }
    }
}