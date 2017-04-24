using System.Collections;
using System.Collections.Generic;

public abstract class DefendingEntity
{
    protected SortedList<int, Ability> abilities;

    private DefendingEntityTemplate defendingEntityTemplate;

    public SortedList<int, Ability> Abilities
    {
        get
        {
            return abilities;
        }
    }

    public DefendingEntityTemplate DefendingEntityTemplate
    {
        get
        {
            return defendingEntityTemplate;
        }
    }

    public DefendingEntity(DefendingEntityTemplate template)
    {
        abilities = new SortedList<int, Ability>();

        for (int i = 0; i < template.BaseAbilities.Length; i++)
        {
            abilities.Add(i, template.BaseAbilities[i].GenerateAbility());
        }

        defendingEntityTemplate = template;
    }

    public List<DefendModeAbility> DefendModeAbilities()
    {
        List<DefendModeAbility> defendModeAbilities = new List<DefendModeAbility>();

        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i] is DefendModeAbility)
            {
                defendModeAbilities.Add((DefendModeAbility)abilities[i]);
            }
        }

        return defendModeAbilities;
    }

    public List<BuildModeAbility> BuildModeAbilities()
    {
        List<BuildModeAbility> buildModeAbilities = new List<BuildModeAbility>();

        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i] is BuildModeAbility)
            {
                buildModeAbilities.Add((BuildModeAbility)abilities[i]);
            }
        }

        return buildModeAbilities;
    }

    public abstract string UIText();

}
