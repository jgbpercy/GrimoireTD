using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave {

    private SortedList<float, CreepTemplate> spawns; 

    private bool debugOn;

    public SortedList<float, CreepTemplate> Spawns
    {
        get
        {
            return spawns;
        }
    }

    public Wave()
    {
        spawns = new SortedList<float, CreepTemplate>();

    }

    public Wave(SortedList<float, CreepTemplate> spawns)
    {
        this.spawns = spawns;
    }

    public Wave(float[] timings, CreepTemplate[] creeps)
    {

        spawns = new SortedList<float, CreepTemplate>();

        for (int i = 0; i < timings.Length; i++)
        {
            if ( creeps.Length > i )
            {
                spawns.Add(timings[i], creeps[i]);
            }
        }
    }

    public bool AddCreepToSpawns(float timeOffset, CreepTemplate creep)
    {
        if ( spawns.ContainsKey(timeOffset) )
        {
            return false;
        }
        else
        {
            spawns.Add(timeOffset, creep);
            return true;
        }
    }

    public float NextSpawnTime()
    {
        if ( spawns.Count == 0 )
        {
            return 0f;
        }
        return spawns.Keys[0];
    }

    public CreepTemplate SpawnNextCreep()
    {
        CreepTemplate returnCreep = spawns.Values[0];

        spawns.RemoveAt(0);

        return returnCreep;
    }
}
