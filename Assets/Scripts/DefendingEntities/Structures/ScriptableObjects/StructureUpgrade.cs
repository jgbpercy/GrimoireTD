using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStuctureUpgrade", menuName = "Structures and Units/Structure Upgrade")]
public class StructureUpgrade : ScriptableObject {

    [SerializeField]
    private StructureEnhancement[] optionalEnhancements;

    [SerializeField]
    private NamedAttributeModifier[] mainUpgradeBonus;

    [SerializeField]
    private string newStructureName;

    [SerializeField]
    private string newStructureDescription;

    [SerializeField]
    private string bonusDescription;

    public StructureEnhancement[] OptionalEnhancements
    {
        get
        {
            return optionalEnhancements;
        }
    }

    public NamedAttributeModifier[] MainUpgradeBonus
    {
        get
        {
            return mainUpgradeBonus;
        }
    }

    public string NewStructureName
    {
        get
        {
            return newStructureName;
        }
    }

    public string NewStructureDescription
    {
        get
        {
            return newStructureDescription;
        }
    }

    public string BonusDescription
    {
        get
        {
            return bonusDescription;
        }
    }
}
