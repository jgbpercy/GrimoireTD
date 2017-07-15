using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "Structures and Units/Unit")]
public class UnitTemplate : DefendingEntityTemplate {

    [SerializeField]
    private string nameInGame;

    [SerializeField]
    private string description;

    [SerializeField]
    private int experienceToLevelUp = 100;

    [SerializeField]
    private UnitTalent[] unitTalents;
    
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

    public int ExperienceToLevelUp
    {
        get
        {
            return experienceToLevelUp;
        }
    }

    public UnitTalent[] UnitTalents
    {
        get
        {
            return unitTalents;
        }
    }

    public IUnitImprovement BaseUnitCharacteristics
    {
        get
        {
            SoUnitImprovement baseUnitCharacteristics = BaseCharacteristics as SoUnitImprovement;
            if ( baseUnitCharacteristics != null )
            {
                return baseUnitCharacteristics;
            }
            throw new System.Exception("Unit BaseCharacteristics is not a SoUnitImprovement");
        }
    }

    public virtual Unit GenerateUnit(Coord position)
    {
        return new Unit(this, position);
    }
}
