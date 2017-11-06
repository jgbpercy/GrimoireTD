using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Dependencies;

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

            var creepManager = DepsProv.TheCreepManager;

            var creepList = creepManager.CreepList;

            for (int i = 0; i < creepList.Count; i++)
            {
                creepDistanceFromPosition = Vector3.Magnitude(
                    creepList[i].Position - args.AttachedToDefender.CoordPosition.ToPositionVector()
                );

                if (creepDistanceFromPosition < args.Range)
                {
                    target = creepList[i];
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
