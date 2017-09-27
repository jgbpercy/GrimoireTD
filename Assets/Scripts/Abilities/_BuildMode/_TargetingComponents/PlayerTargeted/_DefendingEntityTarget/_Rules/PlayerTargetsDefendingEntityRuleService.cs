using System;

namespace GrimoireTD.Abilities.BuildMode
{
    public static class PlayerTargetsDefendingEntityRuleService
    {
        public static bool RunRule<T>(T args) where T : PlayerTargetsDefendingEntityArgs
        {
            throw new Exception("DefendingEntityTargetingRules was passed a rule args for which there was no rule.");
        }
    }
}
