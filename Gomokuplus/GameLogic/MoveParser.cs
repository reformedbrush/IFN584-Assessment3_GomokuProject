using System;
using GomokuPlus.Exceptions;
using GomokuPlus.Models;

namespace GomokuPlus.GameLogic
{
    // Turns raw text like "O3:4" into a Move object. Parsed entirely by
    // hand with basic string methods covered in class (Length, Substring) -
    // no regular expressions and no other string library methods.
    public class MoveParser
    {
        public static Move Parse(string command)
        {
            if (command == null || command.Length == 0)
                throw new InvalidCommandException("Command cannot be empty.");

            char prefixChar = command[0];
            MoveType type;

            if (prefixChar == 'O' || prefixChar == 'o')
                type = MoveType.Ordinary;
            else if (prefixChar == 'H' || prefixChar == 'h')
                type = MoveType.Heavy;
            else if (prefixChar == 'E' || prefixChar == 'e')
                type = MoveType.Eraser;
            else
                throw new InvalidCommandException("'" + command + "' is not a recognised command. Use O/H/E[Row]:[Col], e.g. O3:4.");

            string remainder = command.Substring(1);

            // Find the ':' separator by hand.
            int colonPosition = -1;
            for (int i = 0; i < remainder.Length; i++)
            {
                if (remainder[i] == ':')
                {
                    colonPosition = i;
                    break;
                }
            }

            if (colonPosition < 0)
                throw new InvalidCommandException("'" + command + "' is missing the ':' separator. Use O/H/E[Row]:[Col], e.g. O3:4.");

            string rowText = remainder.Substring(0, colonPosition);
            string colText = remainder.Substring(colonPosition + 1);

            int row;
            int col;

            if (!int.TryParse(rowText, out row))
                throw new InvalidCommandException("'" + command + "' does not have a valid row number.");

            if (!int.TryParse(colText, out col))
                throw new InvalidCommandException("'" + command + "' does not have a valid column number.");

            return new Move(type, row, col);
        }
    }
}
