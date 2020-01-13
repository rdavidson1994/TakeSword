using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TakeSword
{
    public enum Direction
    {
        None = 0,
        North = 1,
        South = -1,
        East = 2,
        West = -2,
        Up = 3,
        Down = -3
    }

    public static class DirectionExtensions
    {
        public static Direction Opposite(this Direction direction)
        {
            return (Direction)(-(int)direction);
        }
    }

    public static class DirectionConverter
    {
        static Dictionary<string, Direction> letterToDirection = new Dictionary<string, Direction>
        {
            {"n", Direction.North },
            {"s", Direction.South },
            {"e", Direction.East },
            {"w", Direction.West },
            {"u", Direction.Up },
            {"d", Direction.Down },
        };
        static string[] directionNames = { "north", "south", "east", "west", "up", "down" };
        public static Direction FromString(string str)
        {
            string key;
            if (str.Length != 1 && directionNames.Contains(str))
            {
                key = str[0].ToString();
            }
            else
            {
                key = str;
            }
            if (letterToDirection.TryGetValue(key, out Direction direction))
            {
                return direction;
            }
            return Direction.None;
        }
    }
}
