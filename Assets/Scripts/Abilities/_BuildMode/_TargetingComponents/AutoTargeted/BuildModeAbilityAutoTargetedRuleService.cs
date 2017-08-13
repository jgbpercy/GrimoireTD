﻿using System;
using System.Collections.Generic;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public static class BuildModeAbilityAutoTargetedRuleService
    {

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
}