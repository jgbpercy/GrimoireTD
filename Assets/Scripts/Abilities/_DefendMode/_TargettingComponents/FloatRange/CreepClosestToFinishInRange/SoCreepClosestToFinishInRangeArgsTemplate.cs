using GrimoireTD.Creeps;
using GrimoireTD.DefendingEntities;
using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode
{
    [CreateAssetMenu(fileName = "NewSoCreepClosestToFinishInRangeArgsTemplate", menuName = "Defend Mode Abilities/Targeting Components/Rule Args/Creep Closest To Finish In Range")]
    public class SoCreepClosestToFinishInRangeArgsTemplate : SoFloatRangeArgsTemplate
    {
        public override DefendModeTargetingArgs GenerateArgs(
            IDefendingEntity attachedToDefendingEntity, 
            IReadOnlyCreepManager creepManager
        )
        {
            var range = GetActualRange(attachedToDefendingEntity);

            return new CreepClosestToFinishInRangeArgs(attachedToDefendingEntity, range, creepManager);
        }
    }
}