using System;

namespace TakeSword
{
    public interface IUserInterface
    {
        bool UpdateMap(string mapData);
        bool PrintOutput(FormattableString output);
        string GetInput();
    }
}