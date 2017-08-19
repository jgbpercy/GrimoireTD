using GrimoireTD.DefendingEntities;
using UnityEngine;
using UnityEngine.Assertions;

namespace GrimoireTD.Abilities.BuildMode
{
    [CreateAssetMenu(fileName = "NewPlayerTargetsDefendingEntityComponent", menuName = "Build Mode Abilities/Player Targeted/DefendingEntity Targeted")]
    public class SoPlayerTargetsDefendingEntityComponent : SoPlayerTargetedComponent, IPlayerTargetsDefendingEntityComponent
    {
        [SerializeField]
        private BuildModeAbilityDefendingEntityTargetingRuleService.RuleName targetingRule;

        public override bool IsValidTarget(IDefendingEntity sourceDefendingEntity, IBuildModeTargetable potentialTarget)
        {
            IDefendingEntity potentialTargetDefendingEntity = potentialTarget as IDefendingEntity;

            Assert.IsTrue(potentialTargetDefendingEntity != null);

            return BuildModeAbilityDefendingEntityTargetingRuleService.RunRule(targetingRule, sourceDefendingEntity, potentialTargetDefendingEntity);
        }
    }
}