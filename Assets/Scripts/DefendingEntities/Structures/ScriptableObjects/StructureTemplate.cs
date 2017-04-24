using UnityEngine;

[CreateAssetMenu(fileName = "NewStructure", menuName = "Structures and Units/Structure")]
public class StructureTemplate : DefendingEntityTemplate {

    [SerializeField]
    private EconomyTransaction cost;

    public EconomyTransaction Cost
    {
        get
        {
            return cost;
        }
    }

    public string UIText()
    {
        string uiText = "F:" + Mathf.Abs(cost.FoodChange) + "  W:" + Mathf.Abs(cost.WoodChange) + "  S:" + Mathf.Abs(cost.StoneChange) + "  G:" + Mathf.Abs(cost.GoldChange) + "  M:" + Mathf.Abs(cost.ManaChange) + "\n";

        uiText += "Abilities: " + baseAbilities.Length;

        return uiText;
    }

    public virtual Structure GenerateStructure(Vector3 position)
    {
        return new Structure(this, position);
    }
}
