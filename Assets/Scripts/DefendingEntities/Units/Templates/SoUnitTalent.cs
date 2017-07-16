using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitTalent", menuName = "Structures and Units/Unit Talent")]
public class SoUnitTalent : ScriptableObject, IUnitTalent {

    [SerializeField]
    private SoUnitImprovement[] unitImprovements;

    [SerializeField]
    private string descriptionText;

    //Eugh https://stackoverflow.com/questions/5968708/why-array-implements-ilist
    //IList needed as these need to be accessed by index
    //TODO: .NET 4.5 fixes this with IReadOnlyList? Try once available
    public IList<IUnitImprovement> UnitImprovements
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
