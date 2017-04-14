using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreepManager : MonoBehaviour {

    [SerializeField]
    private GameObject[] creepPrefabs;

    private List<Wave> WaveList;

    private int currentWave = 0;

    private List<Coord> path;
    private MapLoader mapLoader;
    private bool pathGenerated;

    private List<Creep> creepList;

    private bool waveIsActive = false;
    private bool waveIsSpawning = false;

    private EconomyManager economyManager;

    //debug crap
    private bool infDebugSpawn = false;
    private bool infDebugSpawnWave = false;

    [SerializeField]
    private bool debugOn = false;

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

    public List<Coord> Path
    {
        get
        {
            return path;
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
        //own components
        mapLoader = gameObject.GetComponent<MapLoader>();
        economyManager = gameObject.GetComponent<EconomyManager>();

        slowSources = slowSourcesInEditor;
    }

    void Start () {


        creepList = new List<Creep>();

        path = null;
        pathGenerated = false;

        //test waves
        WaveList = new List<Wave>();
        float[] timings = new float[10] { 0.25f, 0.5f, 0.75f, 1f, 1.25f, 3.25f, 3.5f, 3.75f, 4, 4.25f };
        GameObject[] creeps = new GameObject[10] { creepPrefabs[1], creepPrefabs[1], creepPrefabs[1], creepPrefabs[1], creepPrefabs[1], creepPrefabs[0], creepPrefabs[0], creepPrefabs[0], creepPrefabs[0], creepPrefabs[0] };

        WaveList.Add(new Wave(timings, creeps));

        float[] timingsTwo = new float[10] { 0.5f, 0.7f, 0.9f, 1.4f, 1.6f, 1.8f, 2.0f, 2.5f, 2.7f, 2.9f };
        GameObject[] creepsTwo = new GameObject[10] { creepPrefabs[0], creepPrefabs[0], creepPrefabs[0], creepPrefabs[0], creepPrefabs[0], creepPrefabs[0], creepPrefabs[0], creepPrefabs[0], creepPrefabs[0], creepPrefabs[0] };

        WaveList.Add(new Wave(timingsTwo, creepsTwo));

    }

    void Update () {

		if (mapLoader.Map != null && !pathGenerated)
        {
            path = mapLoader.Map.GetPath(new Coord(0, 0), new Coord(mapLoader.Map.Width - 1, mapLoader.Map.Height - 1));

            pathGenerated = true;

            if (debugOn) { 
                Debug.Log("Path generated:");
                foreach (Coord currentCoord in path)
                {
                    Debug.Log("(" + currentCoord.X + ", " + currentCoord.Y + ")");
                }
            }
        }

        if (pathGenerated && debugOn)
        {
            for (int i = 1; i < path.Count; i++)
            {
                Debug.DrawLine(path[i].ToPositionVector(), path[i - 1].ToPositionVector());

            } 

        }

        if ( infDebugSpawnWave )
        {
            infDebugSpawnWave = false;

            StartCoroutine(SpawnWave());
        }

        if ( infDebugSpawn )
        {
            infDebugSpawn = false;
            SpawnCreep(creepPrefabs[0]);
        }

        if ( creepList.Count == 0 && !waveIsSpawning )
        {
            waveIsActive = false;
        }

        creepList.Sort((x, y) => x.DistanceFromEnd.CompareTo(y.DistanceFromEnd));
    }

    public Transform CreepInRangeNearestToEnd(Vector3 position, float range)
    {
        float creepDistanceFromPosition;

        for (int i = 0; i < creepList.Count; i++)
        {
            creepDistanceFromPosition = Vector3.Magnitude(creepList[i].transform.position - position);
            if ( creepDistanceFromPosition < range )
            {
                return creepList[i].transform;
            }
        }

        return null;
    }

    public void CreepDied(Creep deadCreep)
    {
        creepList.Remove(deadCreep);

        economyManager.GainCreepBounty(deadCreep.gameObject.name);
    }

    private IEnumerator SpawnWave()
    {
        Wave waveToSpawn = WaveList[currentWave];
        float prevSpawnTime = 0f;
        GameObject creepToSpawn;

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

        yield return null;

    }

    private void SpawnCreep(GameObject creepToSpawn)
    {
        Creep newCreep = Instantiate(creepToSpawn, Vector3.zero, Quaternion.identity).GetComponent<Creep>();

        creepList.Add(newCreep);
    }

    public bool StartNextWave()
    {
        if ( WaveList.Count <= currentWave )
        {
            return false;
        }

        waveIsActive = true;

        StartCoroutine(SpawnWave());

        return true;
    }
}
    