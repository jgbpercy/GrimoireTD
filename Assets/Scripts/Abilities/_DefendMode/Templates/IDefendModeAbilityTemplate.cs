using System.Collections.Generic;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface IDefendModeAbilityTemplate : IAbilityTemplate
    {
        float BaseCooldown { get; }

        IDefendModeTargetingComponent TargetingComponent { get; }

        IEnumerable<IDefendModeEffectComponentTemplate> EffectComponentTemplates { get; }
    }
}