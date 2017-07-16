using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildModeAbilityTemplate", menuName = "Build Mode Abilities/Build Mode Ability")]
public class SoBuildModeAbilityTemplate : SoAbilityTemplate, IBuildModeAbilityTemplate {

    [SerializeField]
    private EconomyTransaction cost;

    [SerializeField]
    private SoBuildModeTargetingComponent targetingComponent;

    [SerializeField]
    private SoBuildModeEffectComponent effectComponent;

    public EconomyTransaction Cost
    {
        get
        {
            return cost;
        }
    }

    public IBuildModeTargetingComponent TargetingComponent
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
