using UnityEngine;
using System;

[Serializable]
public class StructureEnhancement {

    [SerializeField]
    private SoDefendingEntityImprovement enhancementBonus;

    [SerializeField]
    private string descriptionText;

    [SerializeField]
    private EconomyTransaction cost;

    public IDefendingEntityImprovement EnhancementBonus
    {
        get
        {
            return enhancementBonus;
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

