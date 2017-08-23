using UnityEngine;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.Technical;
using GrimoireTD.ChannelDebug;
using GrimoireTD.Creeps;
using GrimoireTD.Abilities.DefendMode.Projectiles;
using GrimoireTD.DefendingEntities;
using System.Collections.Generic;

namespace GrimoireTD.Map
{
    public class MapEntitiesView : SingletonMonobehaviour<MapEntitiesView>
    {
        private IReadOnlyMapData mapData;

        private IReadOnlyCreepManager creepManager;

        [SerializeField]
        private Transform structureFolder;

        [SerializeField]
        private Transform unitFolder;

        [SerializeField]
        private Transform projectileFolder;

        [SerializeField]
        private Transform creepFolder;

        [SerializeField]
        private DefendingEntityGraphicsMapping[] defendingEntityGraphicsMappings;

        [SerializeField]
        private ProjectileGraphicsMapping[] projectileGraphicsMappings;

        [SerializeField]
        private CreepGraphicsMapping[] creepGraphicsMappings;

        private Dictionary<IDefendingEntityTemplate, GameObject> defendingEntityPrefabs;

        private Dictionary<IProjectileTemplate, GameObject> projectilePrefabs;

        private Dictionary<ICreepTemplate, GameObject> creepPrefabs;

        public IReadOnlyDictionary<IDefendingEntityTemplate, GameObject> DefendingEntityPrefabs
        {
            get
            {
                return defendingEntityPrefabs;
            }
        }

        public IReadOnlyDictionary<IProjectileTemplate, GameObject> ProjectilePrefabs
        {
            get
            {
                return projectilePrefabs;
            }
        }

        public IReadOnlyDictionary<ICreepTemplate, GameObject> CreepPrefabs
        {
            get
            {
                return creepPrefabs;
            }
        }

        private void Awake()
        {
            defendingEntityPrefabs = new Dictionary<IDefendingEntityTemplate, GameObject>();
            foreach (DefendingEntityGraphicsMapping mapping in defendingEntityGraphicsMappings)
            {
                defendingEntityPrefabs.Add(mapping.DefendingEntityTemplate, mapping.Prefab);
            }

            projectilePrefabs = new Dictionary<IProjectileTemplate, GameObject>();
            foreach (ProjectileGraphicsMapping mapping in projectileGraphicsMappings)
            {
                projectilePrefabs.Add(mapping.ProjectileTemplate, mapping.Prefab);
            }

            creepPrefabs = new Dictionary<ICreepTemplate, GameObject>();
            foreach (CreepGraphicsMapping mapping in creepGraphicsMappings)
            {
                creepPrefabs.Add(mapping.CreepTemplate, mapping.Prefab);
            }
        }

        private void Start()
        {
            CDebug.Log(CDebug.applicationLoading, "Map Entities View Start");

            mapData = GameModels.Models[0].MapData;
            creepManager = GameModels.Models[0].CreepManager;

            mapData.RegisterForOnStructureCreatedCallback(CreateStructure);
            mapData.RegisterForOnUnitCreatedCallback(CreateUnit);

            creepManager.RegisterForCreepSpawnedCallback(CreateCreep);
        }

        private void CreateStructure(IStructure structureModel, Coord coord)
        {
            StructureComponent structureComponent = Instantiate
            (
                defendingEntityPrefabs[structureModel.DefendingEntityTemplate], 
                coord.ToPositionVector(), 
                Quaternion.identity, 
                structureFolder
            )   
                .GetComponent<StructureComponent>();

            structureComponent.SetUp(structureModel);

            structureModel.RegisterForOnProjectileCreatedCallback(CreateProjectile);
        }

        private void CreateUnit(IUnit unitModel, Coord coord)
        {
            UnitComponent unitComponent = Instantiate
            (
                defendingEntityPrefabs[unitModel.DefendingEntityTemplate], 
                coord.ToPositionVector(), 
                Quaternion.identity, unitFolder
            )
                .GetComponent<UnitComponent>();

            unitComponent.SetUp(unitModel);

            unitModel.RegisterForOnProjectileCreatedCallback(CreateProjectile);
        }

        private void CreateCreep(ICreep creepModel)
        {
            CreepComponent creepComponent = Instantiate
            (
                creepPrefabs[creepModel.CreepTemplate], 
                creepModel.Position, 
                Quaternion.identity, 
                creepFolder
            )
                .GetComponent<CreepComponent>();

            creepComponent.SetUp(creepModel);
        }

        public void CreateProjectile(IProjectile projectileModel)
        {
            ProjectileComponent projectileController = Instantiate
            (
                projectilePrefabs[projectileModel.ProjectileTemplate], 
                projectileModel.Position, 
                Quaternion.identity, 
                projectileFolder
            )
                .GetComponent<ProjectileComponent>();

            projectileController.SetUp(projectileModel);
        }
    }
}