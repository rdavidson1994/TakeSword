using System;

namespace TakeSword
{
    public class ConsoleUserInterface : IUserInterface
    {
        public IGameOutputFormatter Formatter { get; }

        public ConsoleUserInterface(IGameOutputFormatter formatter)
        {
            Formatter = formatter;
        }
        public bool UpdateMap(string mapData)
        {
            Console.WriteLine(mapData);
            return true;
        }

        public bool PrintOutput(FormattableString output)
        {
            string plainTextOutput = Formatter.FormatString(output);
            Console.WriteLine(plainTextOutput);
            return true;
        }
        public string GetInput()
        {
            return Console.ReadLine();
        }
    }
}