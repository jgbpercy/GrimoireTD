using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewDefendModeAbilityTemplate", menuName = "Abilities/Defend Mode Ability")]
public class DefendModeAbilityTemplate : AbilityTemplate {

    [SerializeField]
    protected float cooldown;

    [SerializeField]
    protected TargetingComponent targetingComponent;

    [SerializeField]
    protected EffectComponent effectComponent;

    public float Cooldown
    {
        get
        {
            return cooldown;
        }
    }

    public TargetingComponent TargetingComponent
    {
        get
        {
            return targetingComponent;
        }
    }

    public EffectComponent EffectComponent
    {
        get
        {
            return effectComponent;
        }
    }

    public override Ability GenerateAbility()
    {
        return new DefendModeAbility(this);
    }

}
