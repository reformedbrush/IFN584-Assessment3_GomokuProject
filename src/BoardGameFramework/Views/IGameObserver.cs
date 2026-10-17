using System.IO;
using BoardGameFramework.Board;

namespace BoardGameFramework.Views
{
    public interface IGameObserver
    {
        void OnStateChanged(GridBoard board, int viewerPlayer, string statusLine);
    }

    public interface IBoardView
    {
        void Render(GridBoard board, int viewerPlayer, TextWriter output);
    }

    public class FullBoardView : IBoardView
    {
        public void Render(GridBoard board, int viewerPlayer, TextWriter output)
        {
            WriteGrid(board, output, null);
        }

        public static void WriteGrid(GridBoard board, TextWriter output, bool[][] visible)
        {
            string header = "   ";
            for (int c = 1; c <= board.Size; c++)
                header = header + TwoDigits(c) + "  ";
            output.WriteLine(header);
            output.WriteLine("   " + HorizontalLine(board.Size));

            for (int r = 1; r <= board.Size; r++)
            {
                string rowText = TwoDigits(r) + " ";
                for (int c = 1; c <= board.Size; c++)
                {
                    char symbol;
                    if (visible != null && !visible[r - 1][c - 1])
                        symbol = '?';
                    else
                        symbol = GridBoard.SymbolFor(board.GetCell(r, c));
                    rowText = rowText + "| " + symbol + " ";
                }
                rowText = rowText + "|";
                output.WriteLine(rowText);
                output.WriteLine("   " + HorizontalLine(board.Size));
            }
        }

        private static string TwoDigits(int number)
        {
            string text = number.ToString();
            if (text.Length < 2)
                return " " + text;
            return text;
        }

        private static string HorizontalLine(int size)
        {
            string line = "";
            for (int c = 0; c < size; c++)
                line = line + "+---";
            return line + "+";
        }
    }

    public class FogBoardView : IBoardView
    {
        public void Render(GridBoard board, int viewerPlayer, TextWriter output)
        {
            bool[][] visible = BuildVisibility(board, viewerPlayer);
            FullBoardView.WriteGrid(board, output, visible);
        }

        public static bool[][] BuildVisibility(GridBoard board, int viewerPlayer)
        {
            bool[][] visible = new bool[board.Size][];
            for (int r = 0; r < board.Size; r++)
            {
                visible[r] = new bool[board.Size];
                for (int c = 0; c < board.Size; c++)
                    visible[r][c] = false;
            }

            for (int r = 1; r <= board.Size; r++)
            {
                for (int c = 1; c <= board.Size; c++)
                {
                    if (board.BelongsTo(r, c, viewerPlayer))
                    {
                        visible[r - 1][c - 1] = true;
                        for (int dr = -1; dr <= 1; dr++)
                        {
                            for (int dc = -1; dc <= 1; dc++)
                            {
                                int nr = r + dr;
                                int nc = c + dc;
                                if (board.IsInside(nr, nc))
                                    visible[nr - 1][nc - 1] = true;
                            }
                        }
                    }
                }
            }
            return visible;
        }
    }

    public class ConsoleObserver : IGameObserver
    {
        private IBoardView view;
        private TextWriter output;

        public ConsoleObserver(IBoardView view, TextWriter output)
        {
            this.view = view;
            this.output = output;
        }

        public IBoardView View
        {
            get { return view; }
        }

        public void OnStateChanged(GridBoard board, int viewerPlayer, string statusLine)
        {
            output.WriteLine();
            view.Render(board, viewerPlayer, output);
            if (statusLine != null && statusLine.Length > 0)
                output.WriteLine(statusLine);
        }
    }
}
