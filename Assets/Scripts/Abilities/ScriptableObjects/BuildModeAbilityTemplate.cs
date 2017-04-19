using UnityEngine;
using System;

public class BuildModeAbilityTemplate : AbilityTemplate {

    public override Ability GenerateAbility()
    {
        throw new NotImplementedException("Cannot Generate from BuildModeAbilityTemplate - it is pseudo-abstract");
    }

}
