using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTargetEnemyClosestToFinishInRange", menuName = "Defend Mode Abilities/Targeting Components/Enemy Closest To Finish In Range")]
public class TargetEnemyClosestToFinishInRange : TargetingComponentFloatRange {

    public override List<ITargetable> FindTargets(DefendingEntity attachedToDefendingEntity)
    {
        ITargetable target = CreepManager.CreepInRangeNearestToEnd(
            attachedToDefendingEntity.CoordPosition.ToPositionVector(), 
            GetActualRange(attachedToDefendingEntity)
        );

        if ( target == null )
        {
            return null;
        }
        else
        {
            return new List<ITargetable> { target };
        }
    }
}
