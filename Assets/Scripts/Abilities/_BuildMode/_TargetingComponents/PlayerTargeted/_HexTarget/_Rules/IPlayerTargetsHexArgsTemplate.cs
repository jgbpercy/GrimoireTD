using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IPlayerTargetsHexArgsTemplate
    {
        PlayerTargetsHexArgs GenerateArgs(
            IDefendingEntity sourceEntity,
            Coord targetCoord,
            IReadOnlyMapData mapData
        );
    }
}