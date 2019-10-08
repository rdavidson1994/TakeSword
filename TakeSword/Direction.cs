using System;
using System.Collections.Generic;
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
    public static class DirectionConverter
    {
        public static Direction Opposite(Direction direction)
        {
            return (Direction)(-(int)direction);
        }
    }
}
