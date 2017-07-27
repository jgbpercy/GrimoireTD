using System;
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
        List<IBuildModeTargetable> targetList = buildModeAbilityTemplate.TargetingComponent.FindTargets(executionPosition);

        buildModeAbilityTemplate.EffectComponent.ExecuteEffect(executingEntity, targetList);

        EconomyManager.Instance.DoTransaction(buildModeAbilityTemplate.Cost);
    }

    public override string UIText()
    {
        return buildModeAbilityTemplate.NameInGame;
    }
}
