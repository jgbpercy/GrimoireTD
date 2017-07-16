using System.Collections.Generic;
using UnityEngine;

public class SoAutoTargetedComponent : SoBuildModeTargetingComponent, IAutoTargetedComponent {

    [SerializeField]
    private BuildModeAbilityAutoTargetedRuleService.RuleName targetingRule;

    public override List<IBuildModeTargetable> FindTargets(Coord position)
    {
        return BuildModeAbilityAutoTargetedRuleService.RunRule(targetingRule, position);
    }
}
