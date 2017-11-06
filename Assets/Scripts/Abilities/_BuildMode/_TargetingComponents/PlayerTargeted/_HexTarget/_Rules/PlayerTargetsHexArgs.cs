using GrimoireTD.Defenders;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class PlayerTargetsHexArgs
    {
        public readonly IDefender SourceDefender;

        public readonly Coord TargetCoord;

        public PlayerTargetsHexArgs(
            IDefender sourceDefender, 
            Coord targetCoord
        )
        {
            SourceDefender = sourceDefender;
            TargetCoord = targetCoord;
        }
    }
}