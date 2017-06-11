using UnityEngine;

public enum HexOccupationRequirement
{
    PRESENT,
    NONE,
    EITHER
}

public class HexTargetedBuildModeAbilityTemplate : BuildModeAbilityTemplate {

    [SerializeField]
    private int range;

    [SerializeField]
    protected HexOccupationRequirement unitPresenceRequirement;
    [SerializeField]
    protected HexOccupationRequirement strucutrePresenceRequirement;

    public int Range
    {
        get
        {
            return range;
        }
    }

    public HexOccupationRequirement UnitPresenceRequirement
    {
        get
        {
            return unitPresenceRequirement;
        }
    }

    public HexOccupationRequirement StructurePresenceRequirement
    {
        get
        {
            return strucutrePresenceRequirement;
        }
    }

    public override Ability GenerateAbility(DefendingEntity attachedToDefendingEntity)
    {
        throw new System.NotImplementedException("Cannot Generate from HexTargetedBuildModeAbilityTemplate - it is pseudo-abstract");
    }

}
