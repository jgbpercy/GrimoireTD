using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitTalent", menuName = "Structures and Units/Unit Talent")]
public class UnitTalent : ScriptableObject {

    //TODO make array of arrays so that you can have multiple attribute gains in a talent level
    [SerializeField]
    private NamedAttributeModifier[] attributeBonuses;

    [SerializeField]
    private string descriptionText;

    public NamedAttributeModifier[] AttributeBonuses
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
