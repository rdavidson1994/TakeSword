using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

namespace TakeSword
{
    public class FormatBuilder
    {
        private List<FormattableString> formattableStrings;
        public FormatBuilder()
        {
            formattableStrings = new List<FormattableString>();
        }
        public void Add(FormattableString formattableString)
        {
            formattableStrings.Add(formattableString);
        }
        public void AddLine(FormattableString formattableString)
        {
            if (formattableStrings.Count != 0)
            {
                formattableStrings.Add($"\n");
            }
            formattableStrings.Add(formattableString);
        }
        public FormattableString Build()
        {
            IEnumerable<object?> allArgs = formattableStrings.SelectMany(f => f.GetArguments());
            string joinedFormats = string.Join("", formattableStrings.Select(x => x.Format));
            return FormattableStringFactory.Create(joinedFormats, allArgs.ToArray());
        }
    }
}