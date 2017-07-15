using System;
using System.Collections.Generic;

public static class BuildModeAbilityAutoTargetedRuleService {

    //private static MapData map = MapGenerator.Instance.Map;

    //Rule names
    public enum RuleName
    {
        SingleHex
    }

    //Wrapper function
    public static List<IBuildModeTargetable> RunRule(RuleName ruleName, Coord targetCoord)
    {
        switch (ruleName)
        {
            case RuleName.SingleHex:
                return SingleHex(targetCoord);
            default:
                throw new Exception("BuildModeAbilityAutoTargetedRuleService was passed an AOE rule name to run that does not have a rule configured.");
        }
    }

    //Rules
    private static List<IBuildModeTargetable> SingleHex(Coord targetCoord)
    {
        return new List<IBuildModeTargetable> { targetCoord };
    }
}
