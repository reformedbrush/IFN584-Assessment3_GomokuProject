using System;

namespace GomokuPlus.Exceptions
{
    // Thrown when raw input text does not match the O/H/E[Row]:[Col] syntax.
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException(string message) : base(message)
        {
        }
    }
}
