using System.Collections.Generic;
using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities.DefendMode
{
    public class CDefendModeTargetingComponent : IDefendModeTargetingComponent
    {
        public IDefendModeTargetingComponentTemplate DefendModeTargetingComponentTemplate { get; }

        public CDefendModeTargetingComponent(IDefendModeTargetingComponentTemplate template)
        {
            DefendModeTargetingComponentTemplate = template;
        }

        public virtual IReadOnlyList<IDefendModeTargetable> FindTargets(IDefender attachedToDefender)
        {
            return DefendModeTargetingRuleService.RunRule(
                DefendModeTargetingComponentTemplate.TargetingRule.GenerateArgs(
                    attachedToDefender 
                )
            );
        }
    }
}