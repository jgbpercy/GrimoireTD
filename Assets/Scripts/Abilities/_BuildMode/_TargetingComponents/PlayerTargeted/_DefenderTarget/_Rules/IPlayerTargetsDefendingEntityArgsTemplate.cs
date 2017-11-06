using GrimoireTD.Defenders;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IPlayerTargetsDefenderArgsTemplate
    {
        PlayerTargetsDefenderArgs GenerateArgs(
            IDefender sourceDefender, 
            IDefender targetDefender
        );
    }
}