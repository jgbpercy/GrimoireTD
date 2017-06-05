using UnityEngine;
using System;

[Serializable]
public class StructureEnhancement {

    [SerializeField]
    private NamedAttributeModifier[] enhancementBonuses;

    [SerializeField]
    private string descriptionText;

    [SerializeField]
    private EconomyTransaction cost;

    public NamedAttributeModifier[] EnhancementBonuses
    {
        get
        {
            return enhancementBonuses;
        }
    }

    public string DescriptionText
    {
        get
        {
            return descriptionText;
        }
    }

    public EconomyTransaction Cost
    {
        get
        {
            return cost;
        }
    }
}

