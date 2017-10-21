using System;
using System.Collections.Generic;
using GrimoireTD.DefendingEntities;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class CMoveEffectComponent : IMoveEffectComponent
    {
        public void ExecuteEffect(IDefendingEntity executingEntity, IReadOnlyList<IBuildModeTargetable> targets)
        {
            IUnit executingUnit = executingEntity as IUnit;

            //#optimisation: disable in release build
            if (executingUnit == null) throw new ArgumentException("Non-unit passed to move ability ExecuteEffect");

            //#optimisation: disable in release build
            if (targets.Count != 1) throw new ArgumentException("Wrong number of targets passed to move ability ExecuteEffect");

            Coord targetCoord = targets[0] as Coord;

            //#optimisation: disable in release build
            if (targetCoord == null) throw new ArgumentException("Non-coord target passed to move ability ExecuteEffect");

            executingUnit.Move(targetCoord);
        }
    }
}