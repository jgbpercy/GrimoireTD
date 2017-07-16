﻿using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStructure", menuName = "Structures and Units/Structure")]
public class SoStructureTemplate : SoDefendingEntityTemplate, IStructureTemplate {

    [SerializeField]
    private string startingNameInGame;

    [SerializeField]
    private string startingDescription;

    [SerializeField]
    private EconomyTransaction cost;

    [SerializeField]
    private SoStructureUpgrade[] structureUpgrades;

    public string StartingNameInGame
    {
        get
        {
            return startingNameInGame;
        }
    }

    public string StartingDescription
    {
        get
        {
            return startingDescription;
        }
    }

    public EconomyTransaction Cost
    {
        get
        {
            return cost;
        }
    }

    public IEnumerable<IStructureUpgrade> StructureUpgrades
    {
        get
        {
            return structureUpgrades;
        }
    }

    public string UIText()
    {
        string uiText = Cost.ToString(EconomyTransaction.StringFormats.ShortNameSingleLine, true) + "\n";

        return uiText;
    }

    public virtual Structure GenerateStructure(Coord position)
    {
        return new Structure(this, position);
    }
}