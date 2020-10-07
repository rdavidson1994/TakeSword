using System;

namespace TakeSword
{
    public interface IGameOutputFormatter
    {
        IVerbalAI<PhysicalActor> VerbalAI { set; }

        string FormatString(FormattableString formattableString);
    }
}