using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTargetEnemyClosestToFinishInRange", menuName = "Abilities/Targeting Components/Enemy Closest To Finish In Range")]
public class TargetEnemyClosestToFinishInRange : TargetingComponentFloatRange {

    public override List<ITargetable> FindTargets(Vector3 position)
    {
        ITargetable target = CreepManager.CreepInRangeNearestToEnd(position, range);

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
