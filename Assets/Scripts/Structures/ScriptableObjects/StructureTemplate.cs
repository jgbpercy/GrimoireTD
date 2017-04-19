using UnityEngine;

[CreateAssetMenu(fileName = "NewStructure", menuName = "Structures and Units/Structure")]
public class StructureTemplate : ScriptableObject {

    [SerializeField]
    private string nameInGame;

    [SerializeField]
    private string description;

    [SerializeField]
    private EconomyTransaction cost;

    [SerializeField]
    private AbilityTemplate[] baseAbilities;

    [SerializeField]
    private GameObject structurePrefab;

    public string NameInGame
    {
        get
        {
            return nameInGame;
        }
    }

    public string Description
    {
        get
        {
            return description;
        }
    }

    public EconomyTransaction Cost
    {
        get
        {
            return cost;
        }
    }

    public AbilityTemplate[] BaseAbilities
    {
        get
        {
            return baseAbilities;
        }
    }

    public GameObject StructurePrefab
    {
        get
        {
            return structurePrefab;
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
