using System.IO;
using BoardGameFramework.Players;

namespace BoardGameFramework.Help
{
    public interface IHelpProvider
    {
        void WriteHelp(TextWriter output, Player player);
    }
}
