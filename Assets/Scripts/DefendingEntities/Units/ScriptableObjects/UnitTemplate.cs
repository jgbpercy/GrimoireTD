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

    [SerializeField]
    private HexOccupationBonus[] baseConditionalHexOccupationBonuses;

    [SerializeField]
    private StructureOccupationBonus[] baseConditionalStructureOccupationBonuses;

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

    public HexOccupationBonus[] BaseConditionalHexOccupationBonuses
    {
        get
        {
            return baseConditionalHexOccupationBonuses;
        }
    }

    public StructureOccupationBonus[] BaseConditionalStructureOccupationBonuses
    {
        get
        {
            return baseConditionalStructureOccupationBonuses;
        }
    }

    public virtual Unit GenerateUnit(Vector3 position)
    {
        return new Unit(this, position);
    }
}
