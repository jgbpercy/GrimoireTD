using System;

namespace GrimoireTD.Abilities.BuildMode
{
    public static class PlayerTargetsDefenderRuleService
    {
        public static Func<PlayerTargetsDefenderArgs, bool> RunRule = (args) =>
        {
            throw new Exception("DefenderTargetingRules was passed a rule args for which there was no rule.");
        };
    }
}
