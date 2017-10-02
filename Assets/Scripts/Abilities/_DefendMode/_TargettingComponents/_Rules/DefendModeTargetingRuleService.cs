using System;
using System.Collections.Generic;
using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode
{
    public static class DefendModeTargetingRuleService
    {
        //Wrapper Function
        public static Func<DefendModeTargetingArgs, List<IDefendModeTargetable>> RunRule = (args) =>
        {
            var closestEnemyToEndInRange = args as CreepClosestToFinishInRangeArgs;
            if (closestEnemyToEndInRange != null)
            {
                return CreepClosestToFinishInRange(closestEnemyToEndInRange);
            }

            throw new ArgumentException("DefendModeTargetingRuleService was passed a rule args for which there was no rule.");
        };

        //Rules
        private static List<IDefendModeTargetable> CreepClosestToFinishInRange(CreepClosestToFinishInRangeArgs args)
        {
            float creepDistanceFromPosition;
            IDefendModeTargetable target = null;

            for (int i = 0; i < args.CreepList.Count; i++)
            {
                creepDistanceFromPosition = Vector3.Magnitude(
                    args.CreepList[i].Position - args.AttachedToDefendingEntity.CoordPosition.ToPositionVector()
                );

                if (creepDistanceFromPosition < args.Range)
                {
                    target = args.CreepList[i];
                    break;
                }
            }

            if (target != null)
            {
                return new List<IDefendModeTargetable> { target };
            }

            return null;
        }
    }
}
