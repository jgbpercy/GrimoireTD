 using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreepManager : SingletonMonobehaviour<CreepManager> {

    [SerializeField]
    private CreepTemplate[] creepTemplates;

    private List<Wave> WaveList;

    private int currentWave = 0;

    private List<Creep> creepList;

    private bool waveIsActive = false;
    private bool waveIsSpawning = false;

    //debug crap
    private bool infDebugSpawn = false;
    private bool infDebugSpawnWave = false;

    [SerializeField]
    private string[] slowSourcesInEditor;

    private static string[] slowSources;

    public static string[] SlowSources
    {
        get
        {
            return slowSources;
        }
    }

    public bool InfDebugSpawn
    {
        set
        {
            infDebugSpawn = value;
        }
    }

    public bool InfDebugSpawnWave
    {
        set
        {
            infDebugSpawnWave = value;
        }
    }

    public bool WaveIsActive
    {
        get
        {
            return waveIsActive;
        }
    }

    private void Awake()
    {
        slowSources = slowSourcesInEditor;
    }

    void Start ()
    {
        CDebug.Log(CDebug.applicationLoading, "Creep Manager Start");

        creepList = new List<Creep>();

        //test waves hacky hacky make scriptable objects
        WaveList = new List<Wave>();
        float[] timings = new float[10] { 0.25f, 0.5f, 0.75f, 1f, 1.25f, 3.25f, 3.5f, 3.75f, 4, 4.25f };
        CreepTemplate[] creeps = new CreepTemplate[10] { creepTemplates[1], creepTemplates[1], creepTemplates[1], creepTemplates[1], creepTemplates[1], creepTemplates[0], creepTemplates[0], creepTemplates[0], creepTemplates[0], creepTemplates[0] };

        WaveList.Add(new Wave(timings, creeps));

        float[] timingsTwo = new float[10] { 0.5f, 0.7f, 0.9f, 1.4f, 1.6f, 1.8f, 2.0f, 2.5f, 2.7f, 2.9f };
        CreepTemplate[] creepsTwo = new CreepTemplate[10] { creepTemplates[0], creepTemplates[0], creepTemplates[0], creepTemplates[0], creepTemplates[0], creepTemplates[0], creepTemplates[0], creepTemplates[0], creepTemplates[0], creepTemplates[0] };

        WaveList.Add(new Wave(timingsTwo, creepsTwo));

    }

    void Update () {

        if ( infDebugSpawnWave )
        {
            infDebugSpawnWave = false;

            StartCoroutine(SpawnWave());
        }

        if ( infDebugSpawn )
        {
            infDebugSpawn = false;
            SpawnCreep(creepTemplates[0]);
        }

        if ( creepList.Count == 0 && !waveIsSpawning )
        {
            waveIsActive = false;
        }

        creepList.Sort((x, y) => x.DistanceFromEnd.CompareTo(y.DistanceFromEnd));
    }

    public static ITargetable CreepInRangeNearestToEnd(Vector3 fromPosition, float range)
    {
        float creepDistanceFromPosition;

        List<Creep> creepList = Instance.creepList;

        for (int i = 0; i < creepList.Count; i++)
        {
            creepDistanceFromPosition = Vector3.Magnitude(creepList[i].Position - fromPosition);
            if ( creepDistanceFromPosition < range )
            {
                return creepList[i];
            }
        }

        return null;
    }

    private IEnumerator SpawnWave()
    {
        CDebug.Log(CDebug.creepSpawning, "SpawnWave coroutine started");

        Wave waveToSpawn = WaveList[currentWave];
        float prevSpawnTime = 0f;
        CreepTemplate creepToSpawn;

        waveIsSpawning = true;

        while ( waveToSpawn.NextSpawnTime() != 0f )
        {
            yield return new WaitForSeconds(waveToSpawn.NextSpawnTime() - prevSpawnTime);
            prevSpawnTime = waveToSpawn.NextSpawnTime();

            creepToSpawn = waveToSpawn.SpawnNextCreep();
            SpawnCreep(creepToSpawn);
        }

        waveIsSpawning = false;
        currentWave++;

        CDebug.Log(CDebug.creepSpawning, "SpawnWave coroutine finished");

        yield return null;

    }

    private void SpawnCreep(CreepTemplate creepToSpawn)
    {
        CDebug.Log(CDebug.creepSpawning, "SpawnCreep called for " + creepToSpawn.NameInGame);

        //TODO unhardcode spawnposition at zero
        Creep newCreep = creepToSpawn.GenerateCreep(Vector3.zero);

        creepList.Add(newCreep);

        newCreep.RegisterForOnDiedCallback(() => creepList.Remove(newCreep));
        newCreep.RegisterForOnDiedCallback(() => EconomyManager.Instance.OnCreepDied(creepToSpawn.Bounty));
    }

    public bool StartNextWave()
    {
        CDebug.Log(CDebug.creepSpawning, "Start Next Wave called, currentWave = " + currentWave);

        if ( WaveList.Count <= currentWave )
        {
            return false;
        }

        waveIsActive = true;

        StartCoroutine(SpawnWave());

        return true;
    }
}
    