using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitTalent", menuName = "Structures and Units/Unit Talent")]
public class UnitTalent : ScriptableObject {

    [SerializeField]
    private SoUnitImprovement[] unitImprovements;

    [SerializeField]
    private string descriptionText;

    public IUnitImprovement[] UnitImprovements
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
