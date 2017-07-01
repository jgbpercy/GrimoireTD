using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitTalent", menuName = "Structures and Units/Unit Talent")]
public class UnitTalent : ScriptableObject {

    //TODO make array of arrays so that you can have multiple attribute gains in a talent level
    [SerializeField]
    private UnitImprovement[] unitImprovements;

    [SerializeField]
    private string descriptionText;

    public UnitImprovement[] UnitImprovements
    {
        get
        {
            return unitImprovements;
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
