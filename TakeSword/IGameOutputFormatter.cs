using System;

namespace TakeSword
{
    public interface IGameOutputFormatter
    {
        IVerbalAI<PhysicalActor> VerbalAI { get; set; }

        string FormatString(FormattableString formattableString);
    }
}