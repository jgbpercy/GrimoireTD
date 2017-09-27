using System.Collections.Generic;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Creeps;

namespace GrimoireTD.Abilities.DefendMode
{
    public class CDefendModeTargetingComponent : IDefendModeTargetingComponent
    {
        public IDefendModeTargetingComponentTemplate Template { get; }

        public CDefendModeTargetingComponent(IDefendModeTargetingComponentTemplate template)
        {
            Template = template;
        }

        public virtual IReadOnlyList<IDefendModeTargetable> FindTargets(IDefendingEntity attachedToDefendingEntity, IReadOnlyCreepManager creepManager)
        {
            return DefendModeTargetingRuleService.RunRule(Template.TargetingRule.GenerateArgs(attachedToDefendingEntity, creepManager));
        }
    }
}