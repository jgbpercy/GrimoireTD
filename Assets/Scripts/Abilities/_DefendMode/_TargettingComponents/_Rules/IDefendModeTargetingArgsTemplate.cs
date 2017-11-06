using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface IDefendModeTargetingArgsTemplate
    {
        DefendModeTargetingArgs GenerateArgs(
            IDefender attachedToDefender
        );
    }
}