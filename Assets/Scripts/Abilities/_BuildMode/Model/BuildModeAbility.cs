using UnityEngine;

public abstract class BuildModeAbility : Ability {

    private BuildModeAbilityTemplate buildModeAbilityTemplate;

    public BuildModeAbilityTemplate BuildModeAbilityTemplate
    {
        get
        {
            return buildModeAbilityTemplate;
        }
    }

    public BuildModeAbility(BuildModeAbilityTemplate template) : base(template)
    {
        buildModeAbilityTemplate = template;
    }

    public abstract bool ExecuteAbility(Coord fromCoord, Coord targetCoord, DefendingEntity executingEntity);
}
