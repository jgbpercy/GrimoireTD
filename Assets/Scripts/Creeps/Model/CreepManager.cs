 using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreepManager : SingletonMonobehaviour<CreepManager> {

    private List<Wave> WaveList;

    private int currentWave = 0;

    private List<Creep> creepList;

    private bool waveIsActive = false;
    private bool waveIsSpawning = false;
    private bool trackIdleTime = false;

    [SerializeField]
    private float trackIdleTimeAfterSpawns = 10f;

    public bool WaveIsActive
    {
        get
        {
            return waveIsActive;
        }
    }

    public bool WaveIsSpawning
    {
        get
        {
            return waveIsSpawning;
        }
    }

    public bool TrackIdleTime
    {
        get
        {
            return trackIdleTime;
        }
    }

    void Start ()
    {
        CDebug.Log(CDebug.applicationLoading, "Creep Manager Start");

        creepList = new List<Creep>();

        //test waves hacky hacky make scriptable objects
        WaveList = new List<Wave>();

        foreach (WaveTemplate waveTemplate in MapGenerator.Instance.Level.Waves)
        {
            WaveList.Add(waveTemplate.GenerateWave());
        }

    }

    void Update () {

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

        yield return new WaitForSeconds(trackIdleTimeAfterSpawns);

        trackIdleTime = false;

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
        trackIdleTime = true;

        StartCoroutine(SpawnWave());

        return true;
    }
}
    