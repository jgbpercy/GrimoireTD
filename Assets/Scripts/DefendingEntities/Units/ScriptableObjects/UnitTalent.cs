using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitTalent", menuName = "Structures and Units/Unit Talent")]
public class UnitTalent : ScriptableObject {

    [SerializeField]
    private UnitAttributeNamedModifier[] attributeBonuses;

    [SerializeField]
    private string descriptionText;

    public UnitAttributeNamedModifier[] AttributeBonuses
    {
        get
        {
            return attributeBonuses;
        }
    }

    public string DescriptionText
    {
        get
        {
            return descriptionText;
        }
    }

}
