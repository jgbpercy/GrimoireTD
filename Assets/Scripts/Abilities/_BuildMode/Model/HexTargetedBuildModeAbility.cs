public abstract class HexTargetedBuildModeAbility : BuildModeAbility {

    private HexTargetedBuildModeAbilityTemplate hexTargetedBuildModeAbilityTemplate;

    private MapData map;

    public HexTargetedBuildModeAbilityTemplate HexTargettedBuildModeAbilityTemplate
    {
        get
        {
            return hexTargetedBuildModeAbilityTemplate;
        }
    }

    public HexTargetedBuildModeAbility(HexTargetedBuildModeAbilityTemplate template) : base(template)
    {
        hexTargetedBuildModeAbilityTemplate = template;

        map = MapGenerator.Instance.Map;
    }

    public virtual bool IsTargetSuitable(Coord fromCoord, Coord targetCoord)
    {
        if ( !MapData.HexIsInRange(hexTargetedBuildModeAbilityTemplate.Range, fromCoord, targetCoord) )
        {
            return false;
        }

        if (hexTargetedBuildModeAbilityTemplate.UnitPresenceRequirement == HexOccupationRequirement.NONE && map.GetHexAt(targetCoord).UnitHere != null)
        {
            return false;
        }
        
        if (hexTargetedBuildModeAbilityTemplate.UnitPresenceRequirement == HexOccupationRequirement.PRESENT && map.GetHexAt(targetCoord).UnitHere == null)
        {
            return false;
        }

        if (hexTargetedBuildModeAbilityTemplate.StructurePresenceRequirement == HexOccupationRequirement.NONE && map.GetHexAt(targetCoord).StructureHere != null)
        {
            return false;
        }

        if (hexTargetedBuildModeAbilityTemplate.StructurePresenceRequirement == HexOccupationRequirement.PRESENT && map.GetHexAt(targetCoord).StructureHere == null)
        {
            return false;
        }

        return true;
    }
}
