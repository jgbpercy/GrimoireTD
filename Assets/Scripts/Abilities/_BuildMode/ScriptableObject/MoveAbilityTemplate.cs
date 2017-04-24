using UnityEngine.Assertions;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMoveAbility", menuName = "Abilities/Build Mode/Move")]
public class MoveAbilityTemplate : HexTargetedBuildModeAbilityTemplate {

    public override Ability GenerateAbility()
    {
        Assert.IsTrue(unitPresenceRequirement == HexOccupationRequirement.NONE);

        return new MoveAbility(this);
    }

}
