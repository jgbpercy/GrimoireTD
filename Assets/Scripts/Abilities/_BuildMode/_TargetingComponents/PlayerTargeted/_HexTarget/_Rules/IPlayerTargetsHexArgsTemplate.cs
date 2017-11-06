using GrimoireTD.Defenders;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IPlayerTargetsHexArgsTemplate
    {
        PlayerTargetsHexArgs GenerateArgs(
            IDefender sourceDefender,
            Coord targetCoord
        );
    }
}