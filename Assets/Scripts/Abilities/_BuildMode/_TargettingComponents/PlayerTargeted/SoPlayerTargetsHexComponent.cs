using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "NewPlayerTargetsHexComponent", menuName = "Build Mode Abilities/Player Targeted/Hex Targeted")]
public class SoPlayerTargetsHexComponent : SoPlayerTargetedComponent, IPlayerTargetsHexComponent {

    [SerializeField]
    private BuildModeAbilityHexTargetingRuleService.RuleName targetingRule;

    [SerializeField]
    private BuildModeAbilityAutoTargetedRuleService.RuleName aoeRule;

    public override bool IsValidTarget(DefendingEntity sourceDefendingEntity, IBuildModeTargetable potentialTarget)
    {
        Coord potentialTargetCoord = potentialTarget as Coord;

        Assert.IsTrue(potentialTargetCoord != null);

        return BuildModeAbilityHexTargetingRuleService.RunRule(targetingRule, sourceDefendingEntity, potentialTargetCoord, range);
    }

    public override List<IBuildModeTargetable> FindTargets(Coord position)
    {
        return BuildModeAbilityAutoTargetedRuleService.RunRule(aoeRule, position);
    }
}
