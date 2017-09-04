using UnityEngine;
using UnityEngine.Assertions;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    [CreateAssetMenu(fileName = "NewPlayerTargetsDefendingEntityComponent", menuName = "Build Mode Abilities/Player Targeted/DefendingEntity Targeted")]
    public class SoPlayerTargetsDefendingEntityComponent : SoPlayerTargetedComponent, IPlayerTargetsDefendingEntityComponent
    {
        [SerializeField]
        private SoBuildModeDefendingEntityTargetedArgsTemplate targetingRule;

        public override bool IsValidTarget(IDefendingEntity sourceDefendingEntity, IBuildModeTargetable potentialTarget, IReadOnlyMapData mapData)
        {
            IDefendingEntity potentialTargetDefendingEntity = potentialTarget as IDefendingEntity;

            Assert.IsTrue(potentialTargetDefendingEntity != null);

            return BuildModeDefendingEntityTargetingRuleService.RunRule(targetingRule.GenerateArgs(sourceDefendingEntity, potentialTargetDefendingEntity, mapData));
        }
    }
}