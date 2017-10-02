using System.Collections.Generic;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Creeps;

namespace GrimoireTD.Abilities.DefendMode
{
    public class CDefendModeTargetingComponent : IDefendModeTargetingComponent
    {
        public IDefendModeTargetingComponentTemplate DefendModeTargetingComponentTemplate { get; }

        public CDefendModeTargetingComponent(IDefendModeTargetingComponentTemplate template)
        {
            DefendModeTargetingComponentTemplate = template;
        }

        public virtual IReadOnlyList<IDefendModeTargetable> FindTargets(IDefendingEntity attachedToDefendingEntity, IReadOnlyList<ICreep> creepLit)
        {
            return DefendModeTargetingRuleService.RunRule(
                DefendModeTargetingComponentTemplate.TargetingRule.GenerateArgs(
                    attachedToDefendingEntity, 
                    creepLit
                )
            );
        }
    }
}