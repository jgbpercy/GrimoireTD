using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "NewPlayerTargetsDefendingEntityComponent", menuName = "Build Mode Abilities/Player Targeted/DefendingEntity Targeted")]
public class SoPlayerTargetsDefendingEntityComponent : SoPlayerTargetedComponent, IPlayerTargetsDefendingEntityComponent {

    [SerializeField]
    private BuildModeAbilityDefendingEntityTargetingRuleService.RuleName targetingRule;

    public override bool IsValidTarget(DefendingEntity sourceDefendingEntity, IBuildModeTargetable potentialTarget)
    {
        DefendingEntity potentialTargetDefendingEntity = potentialTarget as DefendingEntity;

        Assert.IsTrue(potentialTargetDefendingEntity != null);

        return BuildModeAbilityDefendingEntityTargetingRuleService.RunRule(targetingRule, sourceDefendingEntity, potentialTargetDefendingEntity);
    }
}
