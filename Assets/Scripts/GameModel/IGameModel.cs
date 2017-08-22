using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.Creeps;
using GrimoireTD.Economy;
using GrimoireTD.Levels;
using GrimoireTD.Map;
using GrimoireTD.DefendingEntities.Structures;

namespace GrimoireTD
{
    public interface IGameModel
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

        void SetUp(
            ILevel level, 
            IEnumerable<IResourceTemplate> resourceTemplates, 
            IEnumerable<IHexType> hexTypes, 
            IEnumerable<IAttackEffectType> attackEffectTypes,
            IDictionary<Color32, IHexType> colorToHexTypeDictionary,
            IEnumerable<IStructureTemplate> buildableStructureTemplates,
            float trackIdleTimeAfterSpawns,
            float unitFatigueFactorInfelctionPoint,
            float unitFatigueFactorShallownessMultiplier
        );

        void RegisterForOnSetUpCallback(Action callback);
    }
}