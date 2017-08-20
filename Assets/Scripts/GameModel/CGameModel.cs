using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.Creeps;
using GrimoireTD.Economy;
using GrimoireTD.Levels;
using GrimoireTD.Map;

namespace GrimoireTD
{
    public class CGameModel : IGameModel
    {
        private readonly IGameStateManager gameStateManager;

        private readonly IMapData mapData;

        private readonly ICreepManager creepManager;

        private readonly IEconomyManager economyManager;

        private readonly IAttackEffectTypeManager attackEffectTypeManager;

        public float UnitFatigueFactorInfelctionPoint { get; private set; }
        public float UnitFatigueFactorShallownessMultiplier { get; private set; }

        public IReadOnlyGameStateManager GameStateManager
        {
            get
            {
                return gameStateManager;
            }
        }

        public IReadOnlyMapData MapData
        {
            get
            {
                return mapData;
            }
        }

        public IReadOnlyCreepManager CreepManager
        {
            get
            {
                return creepManager;
            }
        }

        public IReadOnlyEconomyManager EconomyManager
        {
            get
            {
                return economyManager;
            }
        }

        public IReadOnlyAttackEffectTypeManager AttackEffectTypeManager
        {
            get
            {
                return attackEffectTypeManager;
            }
        }

        public CGameModel()
        {
            gameStateManager = new CGameStateManager();
            mapData = new CMapData();
            creepManager = new CCreepManager();
            economyManager = new CEconomyManager();
            attackEffectTypeManager = new CAttackEffectTypeManager();

            GameModels.Models.Add(this);
        }

        public void SetUp(
            ILevel level,
            IEnumerable<IResourceTemplate> resourceTemplates,
            IEnumerable<IHexType> hexTypes,
            IEnumerable<IAttackEffectType> attackEffectTypes,
            IDictionary<Color32, IHexType> colorToHexTypeDictionary,
            float trackIdleTimeAfterSpawns,
            float unitFatigueFactorInfelctionPoint,
            float unitFatigueFactorShallownessMultiplier
        )
        {
            gameStateManager.SetUp();
            mapData.SetUp(level.LevelImage, colorToHexTypeDictionary, level.StartingStructures, level.StartingUnits);
            creepManager.SetUp(level.Waves, trackIdleTimeAfterSpawns);
            economyManager.SetUp(resourceTemplates, level.StartingResources);
            attackEffectTypeManager.SetUp(attackEffectTypes);

            UnitFatigueFactorInfelctionPoint = unitFatigueFactorInfelctionPoint;
            UnitFatigueFactorShallownessMultiplier = UnitFatigueFactorShallownessMultiplier;
        }
    }
}