using System;
using System.Collections.Generic;

namespace GrimoireTD.Abilities.DefendMode
{
    public static class DefendModeTargetingRuleService
    {
        //Wrapper Function
        public static List<IDefendModeTargetable> RunRule<T>(T args) where T : DefendModeTargetingArgs
        {
            var closestEnemyToEndInRange = args as CreepClosestToFinishInRangeArgs;
            if (closestEnemyToEndInRange != null)
            {
                return CreepClosestToFinishInRange(closestEnemyToEndInRange);
            }

            throw new ArgumentException("DefendModeTargetingRuleService was passed a rule args for which there was no rule.");
        }

        //Rules
        private static List<IDefendModeTargetable> CreepClosestToFinishInRange(CreepClosestToFinishInRangeArgs args)
        {
            var target = args.CreepManager.CreepInRangeNearestToEnd(
                args.AttachedToDefendingEntity.CoordPosition.ToPositionVector(),
                args.Range
            );

            if (target != null)
            {
                return new List<IDefendModeTargetable> { target };
            }

            return null;
        }
    }
}
