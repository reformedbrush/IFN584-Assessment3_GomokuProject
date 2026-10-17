using GomokuPlus.Framework;

namespace GomokuPlus.GameLogic
{
    public class GomokuFogRules : StandardGomokuRules
    {
        public override string VariantName
        {
            get { return "GomokuFog"; }
        }

        public override string GetHelpText()
        {
            return base.GetHelpText() + "\n" +
                "  - Each player receives a visibility projection around their own stones.";
        }
    }
}
