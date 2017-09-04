using System.Collections.Generic;
using GrimoireTD.DefendingEntities;
using GrimoireTD.ChannelDebug;
using GrimoireTD.DefendingEntities.Units;
using NUnit.Framework;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class CMoveEffectComponent : CBuildModeEffectComponent, IMoveEffectComponent
    {
        public CMoveEffectComponent() { }

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