using System;

namespace LevelObjects
{
    [Flags]
    public enum MoveType
    {
        Down = 1,
        DiagonalRight = 2,
        DiagonalLeft = 4,
    }
}