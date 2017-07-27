using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "NewMoveEffectComponent", menuName = "Build Mode Abilities/Effect Components/Move")]
public class SoMoveEffectComponent : SoBuildModeEffectComponent, IMoveEffectComponent
{
    public override void ExecuteEffect(DefendingEntity executingEntity, IReadOnlyList<IBuildModeTargetable> targets)
    {
        CDebug.Log(CDebug.buildModeAbilities, "Call ExecuteEffect for a MoveEffectComponent");

        Unit executingUnit = executingEntity as Unit;
        Assert.IsTrue(executingUnit != null);

        //Assertions regarding what sort of targeting will have been done for a move ability
        Assert.IsTrue(targets.Count == 1);
        Coord targetCoord = targets[0] as Coord;
        Assert.IsTrue(targetCoord != null);

        executingUnit.Move(targetCoord);
    }
}
