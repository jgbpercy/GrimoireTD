using System;
using System.Collections.Generic;

public class BuildModeAbility : Ability {

    private BuildModeAbilityTemplate buildModeAbilityTemplate;

    public BuildModeAbilityTemplate BuildModeAbilityTemplate
    {
        get
        {
            return buildModeAbilityTemplate;
        }
    }

    public BuildModeAbility(BuildModeAbilityTemplate template, DefendingEntity attachedToDefendingEntity) : base(template, attachedToDefendingEntity)
    {
        buildModeAbilityTemplate = template;
    }

    public void ExecuteAbility(DefendingEntity executingEntity, Coord executionPosition)
    {
        List<IBuildModeTargetable> targetList = buildModeAbilityTemplate.TargetingComponent.FindTargets(executionPosition);

        buildModeAbilityTemplate.EffectComponent.ExecuteEffect(executingEntity, targetList);

        EconomyManager.Instance.DoTransaction(buildModeAbilityTemplate.Cost);
    }

    public override string UIText()
    {
        throw new NotImplementedException();
    }
}
