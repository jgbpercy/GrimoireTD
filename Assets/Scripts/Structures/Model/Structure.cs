using System.Collections.Generic;
using UnityEngine;

public class Structure : DefendingEntity {
    
    private SortedList<int, Ability> abilities;

    private StructureTemplate structureTemplate;

    public SortedList<int, Ability> Abilities
    {
        get
        {
            return abilities;
        }
    }

    public StructureTemplate StructureClassTemplate
    {
        get
        {
            return structureTemplate;
        }
    }

    public Structure(StructureTemplate structureTemplate, Vector3 position)
    {
        this.structureTemplate = structureTemplate;

        abilities = new SortedList<int, Ability>();

        for (int i = 0; i < structureTemplate.BaseAbilities.Length; i++)
        {
            abilities.Add(i, structureTemplate.BaseAbilities[i].GenerateAbility());
        }

        StructureView.Instance.CreateStructure(this, position);
    }

    public string UIText()
    {
        return structureTemplate.Description;
    }

    public override List<DefendModeAbility> DefendModeAbilities()
    {
        List<DefendModeAbility> defendModeAbilities = new List<DefendModeAbility>();

        for (int i = 0; i < abilities.Count; i++)
        {
            if ( abilities[i] is DefendModeAbility )
            {
                defendModeAbilities.Add((DefendModeAbility)abilities[i]);
            }
        }

        return defendModeAbilities;
    }
}
