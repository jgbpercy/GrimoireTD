using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildModeAbilityTemplate", menuName = "Build Mode Abilities/Build Mode Ability")]
public class BuildModeAbilityTemplate : AbilityTemplate {

    [SerializeField]
    private EconomyTransaction cost;

    [SerializeField]
    private BMTargetingComponent targetingComponent;

    [SerializeField]
    private SoBuildModeEffectComponent effectComponent;

    public EconomyTransaction Cost
    {
        get
        {
            return cost;
        }
    }

    public BMTargetingComponent TargetingComponent
    {
        get
        {
            return targetingComponent;
        }
    }

    public IBuildModeEffectComponent EffectComponent
    {
        get
        {
            return effectComponent;
        }
    }

    public override Ability GenerateAbility(DefendingEntity attachedToDefendingEntity)
    {
        return new BuildModeAbility(this, attachedToDefendingEntity);
    }

}
