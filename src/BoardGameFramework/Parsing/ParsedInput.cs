using BoardGameFramework.Core;
using BoardGameFramework.Exceptions;

namespace BoardGameFramework.Parsing
{
    public class ParsedInput
    {
        public InputKind Kind { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public string FileName { get; set; }
        public string Raw { get; set; }
    }

    public class InputParser
    {
        public static ParsedInput Parse(string raw)
        {
            if (raw == null)
                throw new InvalidCommandException("Command cannot be empty.");

            string text = Strip(raw);
            if (text.Length == 0)
                throw new InvalidCommandException("Command cannot be empty.");

            ParsedInput parsed = new ParsedInput();
            parsed.Raw = text;

            string upper = ToUpper(text);

            if (upper == "HELP" || upper == "?")
            {
                parsed.Kind = InputKind.Help;
                return parsed;
            }
            if (upper == "QUIT" || upper == "Q")
            {
                parsed.Kind = InputKind.Quit;
                return parsed;
            }
            if (upper == "UNDO")
            {
                parsed.Kind = InputKind.Undo;
                return parsed;
            }
            if (upper == "REDO")
            {
                parsed.Kind = InputKind.Redo;
                return parsed;
            }
            if (upper == "PASS")
            {
                parsed.Kind = InputKind.Pass;
                return parsed;
            }
            if (upper == "S" || upper == "SAVE")
            {
                parsed.Kind = InputKind.Save;
                parsed.FileName = "";
                return parsed;
            }
            if (StartsWith(upper, "SAVE "))
            {
                parsed.Kind = InputKind.Save;
                parsed.FileName = Strip(text.Substring(5));
                return parsed;
            }
            if (upper == "L" || upper == "LOAD")
            {
                parsed.Kind = InputKind.Load;
                parsed.FileName = "";
                return parsed;
            }
            if (StartsWith(upper, "LOAD "))
            {
                parsed.Kind = InputKind.Load;
                parsed.FileName = Strip(text.Substring(5));
                return parsed;
            }

            char prefix = upper[0];
            if (prefix == 'O')
                parsed.Kind = InputKind.PlaceOrdinary;
            else if (prefix == 'H')
                parsed.Kind = InputKind.PlaceHeavy;
            else if (prefix == 'E')
                parsed.Kind = InputKind.PlaceEraser;
            else if (prefix == 'P')
                parsed.Kind = InputKind.PlaceDisk;
            else
                throw new InvalidCommandException("'" + text + "' is not a recognised command.");

            string remainder = text.Substring(1);
            int colon = IndexOf(remainder, ':');
            if (colon < 0)
                throw new InvalidCommandException("'" + text + "' is missing the ':' separator.");

            string rowText = remainder.Substring(0, colon);
            string colText = remainder.Substring(colon + 1);
            int row;
            int col;
            if (!int.TryParse(rowText, out row))
                throw new InvalidCommandException("'" + text + "' does not have a valid row number.");
            if (!int.TryParse(colText, out col))
                throw new InvalidCommandException("'" + text + "' does not have a valid column number.");

            parsed.Row = row;
            parsed.Col = col;
            return parsed;
        }

        public static string[] SplitScript(string script)
        {
            if (script == null)
                return new string[0];

            string[] rawParts = script.Split(',');
            int count = 0;
            for (int i = 0; i < rawParts.Length; i++)
            {
                if (Strip(rawParts[i]).Length > 0)
                    count++;
            }

            string[] parts = new string[count];
            int index = 0;
            for (int i = 0; i < rawParts.Length; i++)
            {
                string item = Strip(rawParts[i]);
                if (item.Length > 0)
                {
                    parts[index] = item;
                    index++;
                }
            }
            return parts;
        }

        private static string Strip(string value)
        {
            if (value == null)
                return "";
            return value.Trim();
        }

        private static string ToUpper(string value)
        {
            return value.ToUpperInvariant();
        }

        private static bool StartsWith(string value, string prefix)
        {
            if (value.Length < prefix.Length)
                return false;
            for (int i = 0; i < prefix.Length; i++)
            {
                if (value[i] != prefix[i])
                    return false;
            }
            return true;
        }

        private static int IndexOf(string value, char symbol)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == symbol)
                    return i;
            }
            return -1;
        }
    }
}
