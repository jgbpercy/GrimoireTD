using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Creeps;

namespace GrimoireTD.Abilities.DefendMode
{
    [CreateAssetMenu(fileName = "NewDefendModeTargetingComponent", menuName = "Defend Mode Abilities/Targeting Components/Targeting Component")]
    public class SoDefendModeTargetingComponent : ScriptableObject, IDefendModeTargetingComponent
    {
        [SerializeField]
        private SoDefendModeTargetingArgsTemplate targetingRule;

        public IDefendModeTargetingArgsTemplate TargetingRule
        {
            get
            {
                return targetingRule;
            }
        }

        public virtual IReadOnlyList<IDefendModeTargetable> FindTargets(IDefendingEntity attachedToDefendingEntity, IReadOnlyCreepManager creepManager)
        {
            return DefendModeTargetingRuleService.RunRule(targetingRule.GenerateArgs(attachedToDefendingEntity, creepManager));
        }
    }
}