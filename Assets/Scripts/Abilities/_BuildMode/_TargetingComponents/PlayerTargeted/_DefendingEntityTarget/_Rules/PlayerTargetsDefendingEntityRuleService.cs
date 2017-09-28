using System;

namespace GrimoireTD.Abilities.BuildMode
{
    public static class PlayerTargetsDefendingEntityRuleService
    {
        public static Func<PlayerTargetsDefendingEntityArgs, bool> RunRule = (args) =>
        {
            throw new Exception("DefendingEntityTargetingRules was passed a rule args for which there was no rule.");
        };
    }
}
