using UnityEngine;
using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities.DefendMode
{
    [CreateAssetMenu(fileName = "NewSoCreepClosestToFinishInRangeArgsTemplate", menuName = "Defend Mode Abilities/Targeting Components/Rule Args/Creep Closest To Finish In Range")]
    public class SoCreepClosestToFinishInRangeArgsTemplate : SoFloatRangeArgsTemplate
    {
        public override DefendModeTargetingArgs GenerateArgs(
            IDefender attachedToDefender
        )
        {
            var range = GetActualRange(attachedToDefender);

            return new CreepClosestToFinishInRangeArgs(attachedToDefender, range);
        }
    }
}