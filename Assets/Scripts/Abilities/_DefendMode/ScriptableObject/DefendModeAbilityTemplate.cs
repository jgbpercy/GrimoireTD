using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewDefendModeAbilityTemplate", menuName = "Defend Mode Abilities/Defend Mode Ability")]
public class DefendModeAbilityTemplate : AbilityTemplate {

    [SerializeField]
    protected float baseCooldown;

    [SerializeField]
    protected TargetingComponent targetingComponent;

    [SerializeField]
    protected EffectComponent effectComponent;

    public float BaseCooldown
    {
        get
        {
            return baseCooldown;
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

    public override Ability GenerateAbility(DefendingEntity attachedToDefendingEntity)
    {
        return new DefendModeAbility(this, attachedToDefendingEntity);
    }

}
