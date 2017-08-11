using GrimoireTD.Economy;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IBuildModeAbilityTemplate : IAbilityTemplate
    {
        IEconomyTransaction Cost { get; }

        IBuildModeTargetingComponent TargetingComponent { get; }

        IBuildModeEffectComponent EffectComponent { get; }
    }
}