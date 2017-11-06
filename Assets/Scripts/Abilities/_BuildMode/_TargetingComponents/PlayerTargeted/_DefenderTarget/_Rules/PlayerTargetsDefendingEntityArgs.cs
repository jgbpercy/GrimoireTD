using GrimoireTD.Defenders;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class PlayerTargetsDefenderArgs
    {
        public readonly IDefender SourceDefender;

        public readonly IDefender TargetDefender;

        public readonly IReadOnlyMapData MapData;

        public PlayerTargetsDefenderArgs(
            IDefender sourceDefender,
            IDefender targetDefender,
            IReadOnlyMapData mapData
        )
        {
            SourceDefender = sourceDefender;
            TargetDefender = targetDefender;
            MapData = mapData;
        }
    }
}