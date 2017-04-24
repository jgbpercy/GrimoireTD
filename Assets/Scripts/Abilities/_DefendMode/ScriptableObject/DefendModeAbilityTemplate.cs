using UnityEngine;
using System;

public class DefendModeAbilityTemplate : AbilityTemplate {

    [SerializeField]
    protected float cooldown;

    public float Cooldown
    {
        get
        {
            return cooldown;
        }
    }

    public override Ability GenerateAbility()
    {
        throw new NotImplementedException("Cannot Generate from DefendModeAbilityTemplate - it is pseudo-abstract");
    }

}
