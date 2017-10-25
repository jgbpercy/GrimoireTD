using UnityEngine;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.Technical;
using GrimoireTD.Creeps;
using GrimoireTD.Abilities.DefendMode.Projectiles;
using GrimoireTD.DefendingEntities;
using System.Collections.Generic;
using GrimoireTD.Dependencies;

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
        private SProjectileGraphicsMapping[] projectileGraphicsMappings;

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
            foreach (SProjectileGraphicsMapping mapping in projectileGraphicsMappings)
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
            mapData = DepsProv.TheMapData;
            creepManager = DepsProv.TheCreepManager;

            mapData.OnStructureCreated += OnStructureCreated;
            mapData.OnUnitCreated += OnUnitCreated;

            creepManager.OnCreepSpawned += OnCreepSpawned;
        }

        private void OnStructureCreated(object sender, EAOnStructureCreated args)
        {
            StructureComponent structureComponent = Instantiate
            (
                defendingEntityPrefabs[args.StructureCreated.DefendingEntityTemplate], 
                args.Position.ToPositionVector(), 
                Quaternion.identity, 
                structureFolder
            )   
                .GetComponent<StructureComponent>();

            structureComponent.SetUp(args.StructureCreated);

            args.StructureCreated.OnProjectileCreated += OnProjectileCreated;
        }

        private void OnUnitCreated(object sender, EAOnUnitCreated args)
        {
            UnitComponent unitComponent = Instantiate
            (
                defendingEntityPrefabs[args.UnitCreated.DefendingEntityTemplate], 
                args.Position.ToPositionVector(), 
                Quaternion.identity, unitFolder
            )
                .GetComponent<UnitComponent>();

            unitComponent.SetUp(args.UnitCreated);

            args.UnitCreated.OnProjectileCreated += OnProjectileCreated;
        }

        private void OnCreepSpawned(object sender, EAOnCreepSpawned args)
        {
            CreepComponent creepComponent = Instantiate
            (
                creepPrefabs[args.NewCreep.CreepTemplate], 
                args.NewCreep.Position, 
                Quaternion.identity, 
                creepFolder
            )
                .GetComponent<CreepComponent>();

            creepComponent.SetUp(args.NewCreep);
        }

        public void OnProjectileCreated(object sender, EAOnProjectileCreated args)
        {
            ProjectileComponent projectileController = Instantiate
            (
                projectilePrefabs[args.Projectile.ProjectileTemplate],
                args.Projectile.Position, 
                Quaternion.identity, 
                projectileFolder
            )
                .GetComponent<ProjectileComponent>();

            projectileController.SetUp(args.Projectile);
        }
    }
}