using System;
using UnityEngine;

/*TODO? Make something like an ITileHasThisCheckable interface by which you can generically 
 * query a tile for some criteria. So a single OccupationBonus class would just have a private
 * ITileHasThisCheckable, and DefendingEntity.GetHexOccupationBonus would just take an 
 * ITileHasThisCheckable. More bother than it is worth for now; current way is clearer and
 * almost same amount of code
*/
[Serializable]
public class StructureOccupationBonus {

    [SerializeField]
    private StructureTemplate structureTemplate;

    [SerializeField]
    private StructureUpgrade structureUpgradeLevel;

    [SerializeField]
    private EconomyTransaction resourceGain;

    public StructureTemplate StructureTemplate
    {
        get
        {
            return structureTemplate;
        }
    }

    public StructureUpgrade StructureUpgradeLevel
    {
        get
        {
            return structureUpgradeLevel;
        }
    }

    public EconomyTransaction ResourceGain
    {
        get
        {
            return resourceGain;
        }
    }
}
