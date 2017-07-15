using UnityEngine;

[CreateAssetMenu(fileName = "NewDefendModeAbilityTemplate", menuName = "Defend Mode Abilities/Defend Mode Ability")]
public class DefendModeAbilityTemplate : AbilityTemplate {

    [SerializeField]
    protected float baseCooldown;

    [SerializeField]
    protected DMTargetingComponent targetingComponent;

    [SerializeField]
    protected DMEffectComponent effectComponent;

    public float BaseCooldown
    {
        get
        {
            return baseCooldown;
        }
    }

    public DMTargetingComponent TargetingComponent
    {
        get
        {
            return targetingComponent;
        }
    }

    public DMEffectComponent EffectComponent
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
