using GrimoireTD.Defenders;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class ValidMoveArgs : PlayerTargetsHexArgs
    {
        public readonly int Range;

        public ValidMoveArgs(
            IDefender sourceDefender,
            Coord targetCoord,
            int range
        ) : base(sourceDefender, targetCoord)
        {
            Range = range;
        }
    }
}