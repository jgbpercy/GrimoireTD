using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    [CreateAssetMenu(fileName = "NewPlayerTargetsHexComponent", menuName = "Build Mode Abilities/Player Targeted/Hex Targeted")]
    public class SoPlayerTargetsHexComponent : SoPlayerTargetedComponent, IPlayerTargetsHexComponent
    {
        [SerializeField]
        private SoBuildModeHexTargetedArgsTemplate targetingRuleArgsTemplate;

        [SerializeField]
        private SoBuildModeAutoTargetedArgsTemplate aoeRule;

        public override bool IsValidTarget(IDefendingEntity sourceDefendingEntity, IBuildModeTargetable potentialTarget, IReadOnlyMapData mapData)
        {
            Coord potentialTargetCoord = potentialTarget as Coord;

            Assert.IsTrue(potentialTargetCoord != null);

            return BuildModeHexTargetingRuleService.RunRule(targetingRuleArgsTemplate.GenerateArgs(sourceDefendingEntity, potentialTargetCoord, mapData));
        }

        public override IReadOnlyList<IBuildModeTargetable> FindTargets(Coord position, IReadOnlyMapData mapData)
        {
            return BuildModeAbilityAutoTargetedRuleService.RunRule(aoeRule.GenerateArgs(position, mapData));
        }
    }
}