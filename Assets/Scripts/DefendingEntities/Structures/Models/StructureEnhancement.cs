using UnityEngine;
using System;

//TODO: Make SO/I?
[Serializable]
public class StructureEnhancement {

    [SerializeField]
    private SoDefendingEntityImprovement enhancementBonus;

    [SerializeField]
    private string descriptionText;

    [SerializeField]
    private SEconomyTransaction cost;

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

    public IEconomyTransaction Cost
    {
        get
        {
            return cost;
        }
    }
}

