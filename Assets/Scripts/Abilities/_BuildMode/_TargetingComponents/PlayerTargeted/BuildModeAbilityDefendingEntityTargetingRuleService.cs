using System;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.BuildMode
{
    //TODO: If sticking with Unity SOs, does Unity have a better way of allowing me to select a function than this?
    public static class BuildModeAbilityDefendingEntityTargetingRuleService
    {
        public enum RuleName
        {

        }

        public static bool RunRule(RuleName rule, DefendingEntity sourceDefendingEntity, DefendingEntity targetDefendingEntity)
        {
            switch (rule)
            {
                default:
                    throw new Exception("DefendingEntityTargetingRules was passed a rule name to run that does not have a rule configured.");
            }
        }
    }
}
