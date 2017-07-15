using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTargetEnemyClosestToFinishInRange", menuName = "Defend Mode Abilities/Targeting Components/Enemy Closest To Finish In Range")]
public class TargetEnemyClosestToFinishInRange : TargetingComponentFloatRange {

    public override List<IDefendModeTargetable> FindTargets(DefendingEntity attachedToDefendingEntity)
    {
        IDefendModeTargetable target = CreepManager.CreepInRangeNearestToEnd(
            attachedToDefendingEntity.CoordPosition.ToPositionVector(), 
            GetActualRange(attachedToDefendingEntity)
        );

        if ( target == null )
        {
            return null;
        }
        else
        {
            return new List<IDefendModeTargetable> { target };
        }
    }
}
