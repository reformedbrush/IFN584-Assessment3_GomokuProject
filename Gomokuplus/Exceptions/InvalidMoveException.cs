using System;

namespace GomokuPlus.Exceptions
{
    // Thrown whenever a parsed move breaks a game rule: off-grid coordinates,
    // an occupied cell, an empty inventory, or an illegal Eraser target.
    public class InvalidMoveException : Exception
    {
        public InvalidMoveException(string message) : base(message)
        {
        }
    }
}
