using System.Collections.Generic;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface IDefendModeAbilityTemplate : IAbilityTemplate
    {
        float BaseCooldown { get; }

        IDefendModeTargetingComponentTemplate TargetingComponentTemplate { get; }

        IEnumerable<IDefendModeEffectComponentTemplate> EffectComponentTemplates { get; }
    }
}