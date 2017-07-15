using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTargetedComponent : BMTargetingComponent {

    [SerializeField]
    private BuildModeAbilityAutoTargetedRuleService.RuleName targetingRule;

    public override List<IBuildModeTargetable> FindTargets(Coord position)
    {
        return BuildModeAbilityAutoTargetedRuleService.RunRule(targetingRule, position);
    }
}
