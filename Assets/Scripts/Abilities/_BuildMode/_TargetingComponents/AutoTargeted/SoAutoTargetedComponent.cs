using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SoAutoTargetedComponent : SoBuildModeTargetingComponent, IAutoTargetedComponent
    {
        [SerializeField]
        private SoBuildModeAutoTargetedArgsTemplate targetingRule;

        public override IReadOnlyList<IBuildModeTargetable> FindTargets(Coord position, IReadOnlyMapData mapData)
        {
            return BuildModeAbilityAutoTargetedRuleService.RunRule(targetingRule.GenerateArgs(position, mapData));
        }
    }
}
