using UnityEngine;

[CreateAssetMenu(fileName = "NewStructure", menuName = "Structures and Units/Structure")]
public class StructureTemplate : DefendingEntityTemplate {

    [SerializeField]
    private string startingNameInGame;

    [SerializeField]
    private string startingDescription;

    [SerializeField]
    private EconomyTransaction cost;

    [SerializeField]
    private StructureUpgrade[] structureUpgrades;

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

    public StructureUpgrade[] StructureUpgrades
    {
        get
        {
            return structureUpgrades;
        }
    }

    public string UIText()
    {
        string uiText = "F:" + Mathf.Abs(cost.FoodChange) + "  W:" + Mathf.Abs(cost.WoodChange) + "  S:" + Mathf.Abs(cost.StoneChange) + "  G:" + Mathf.Abs(cost.GoldChange) + "  M:" + Mathf.Abs(cost.ManaChange) + "\n";

    

        return uiText;
    }

    public virtual Structure GenerateStructure(Coord position)
    {
        return new Structure(this, position);
    }
}
