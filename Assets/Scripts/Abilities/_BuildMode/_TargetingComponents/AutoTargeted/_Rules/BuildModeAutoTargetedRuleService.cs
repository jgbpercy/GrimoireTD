using System;
using System.Collections.Generic;

namespace GrimoireTD.Abilities.BuildMode
{
    public static class BuildModeAutoTargetedRuleService
    {
        //Wrapper Function
        public static Func<BuildModeAutoTargetedArgs, List<IBuildModeTargetable>> RunRule = (args) =>
        {
            var singleHexArgs = args as SingleHexArgs;
            if (singleHexArgs != null)
            {
                return SingleHex(singleHexArgs);
            }

            throw new ArgumentException("BuildModeAbilityAutoTargetedRuleService was passed a rule args for which there was no rule.");
        };

        //Rules
        private static List<IBuildModeTargetable> SingleHex(SingleHexArgs args)
        {
            if (args.TargetCoord == null)
            {
                return null;
            }

            return new List<IBuildModeTargetable> { args.TargetCoord };
        }
    }
}
