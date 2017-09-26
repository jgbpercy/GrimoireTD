using System.Collections.Generic;
using GrimoireTD.Economy;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IBuildModeAbilityTemplate : IAbilityTemplate
    {
        IEconomyTransaction Cost { get; }

        IBuildModeTargetingComponent TargetingComponent { get; }

        IEnumerable<IBuildModeEffectComponentTemplate> EffectComponentTemplates { get; }
    }
}