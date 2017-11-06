using System;
using System.Collections.Generic;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.Creeps;
using GrimoireTD.Economy;
using GrimoireTD.Map;
using GrimoireTD.Defenders.Structures;

namespace GrimoireTD
{
    public interface IReadOnlyGameModel
    {
        IReadOnlyGameStateManager GameStateManager { get; }
        IReadOnlyMapData MapData { get; }
        IReadOnlyCreepManager CreepManager { get; }
        IReadOnlyEconomyManager EconomyManager { get; }
        IReadOnlyAttackEffectTypeManager AttackEffectTypeManager { get; }

        bool IsSetUp { get; }

        IEnumerable<IStructureTemplate> BuildableStructureTemplates { get; }

        float UnitFatigueFactorInfelctionPoint { get; }
        float UnitFatigueFactorShallownessMultiplier { get; }

        event EventHandler<EAOnGameModelSetUp> OnGameModelSetUp;
    }
}