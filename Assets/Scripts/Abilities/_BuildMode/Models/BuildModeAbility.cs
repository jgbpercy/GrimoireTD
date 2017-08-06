using System.Collections.Generic;

public class BuildModeAbility : Ability {

    private IBuildModeAbilityTemplate buildModeAbilityTemplate;

    public IBuildModeAbilityTemplate BuildModeAbilityTemplate
    {
        get
        {
            return buildModeAbilityTemplate;
        }
    }

    public BuildModeAbility(IBuildModeAbilityTemplate template, DefendingEntity attachedToDefendingEntity) : base(template, attachedToDefendingEntity)
    {
        buildModeAbilityTemplate = template;
    }

    public void ExecuteAbility(DefendingEntity executingEntity, Coord executionPosition)
    {
        IReadOnlyList<IBuildModeTargetable> targetList = buildModeAbilityTemplate.TargetingComponent.FindTargets(executionPosition);

        buildModeAbilityTemplate.EffectComponent.ExecuteEffect(executingEntity, targetList);

        buildModeAbilityTemplate.Cost.DoTransaction();
    }

    public override string UIText()
    {
        return buildModeAbilityTemplate.NameInGame;
    }
}
