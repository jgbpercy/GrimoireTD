using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Creeps;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.DefendMode
{
    [CreateAssetMenu(fileName = "NewSoCreepClosestToFinishInRangeArgsTemplate", menuName = "Defend Mode Abilities/Targeting Components/Rule Args/Creep Closest To Finish In Range")]
    public class SoCreepClosestToFinishInRangeArgsTemplate : SoFloatRangeArgsTemplate
    {
        public override DefendModeTargetingArgs GenerateArgs(
            IDefendingEntity attachedToDefendingEntity
        )
        {
            var range = GetActualRange(attachedToDefendingEntity);

            return new CreepClosestToFinishInRangeArgs(attachedToDefendingEntity, range);
        }
    }
}