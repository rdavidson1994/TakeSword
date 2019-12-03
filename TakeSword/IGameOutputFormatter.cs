using System;

namespace TakeSword
{
    public interface IGameOutputFormatter
    {
        IVerbalAI VerbalAI { get; set; }

        string FormatString(FormattableString formattableString);
    }
}