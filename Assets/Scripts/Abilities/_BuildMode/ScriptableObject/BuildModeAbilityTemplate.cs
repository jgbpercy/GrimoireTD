using UnityEngine;
using System;

public class BuildModeAbilityTemplate : AbilityTemplate {

    [SerializeField]
    private EconomyTransaction cost;

    public EconomyTransaction Cost
    {
        get
        {
            return cost;
        }
    }

    public override Ability GenerateAbility(DefendingEntity attachedToDefendingEntity)
    {
        throw new NotImplementedException("Cannot Generate from BuildModeAbilityTemplate - it is pseudo-abstract");
    }

}
