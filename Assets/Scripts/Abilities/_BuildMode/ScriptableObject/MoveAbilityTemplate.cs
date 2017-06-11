using UnityEngine.Assertions;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMoveAbility", menuName = "Build Mode Abilities/Move")]
public class MoveAbilityTemplate : HexTargetedBuildModeAbilityTemplate {

    public override Ability GenerateAbility(DefendingEntity attachedToDefendingEntity)
    {
        Assert.IsTrue(unitPresenceRequirement == HexOccupationRequirement.NONE);

        return new MoveAbility(this, attachedToDefendingEntity);
    }

}
