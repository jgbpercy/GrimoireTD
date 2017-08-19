﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GrimoireTD.DefendingEntities;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.ChannelDebug;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    [CreateAssetMenu(fileName = "NewMoveEffectComponent", menuName = "Build Mode Abilities/Effect Components/Move")]
    public class SoMoveEffectComponent : SoBuildModeEffectComponent, IMoveEffectComponent
    {
        public override void ExecuteEffect(IDefendingEntity executingEntity, IReadOnlyList<IBuildModeTargetable> targets)
        {
            CDebug.Log(CDebug.buildModeAbilities, "Call ExecuteEffect for a MoveEffectComponent");

            IUnit executingUnit = executingEntity as IUnit;
            Assert.IsTrue(executingUnit != null);

            //Assertions regarding what sort of targeting will have been done for a move ability
            Assert.IsTrue(targets.Count == 1);
            Coord targetCoord = targets[0] as Coord;
            Assert.IsTrue(targetCoord != null);

            executingUnit.Move(targetCoord);
        }
    }
}