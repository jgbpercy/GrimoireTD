﻿using UnityEngine;

public interface ICreepTemplate {

    string NameInGame { get; }

    float BaseSpeed { get; }

    int MaxHitpoints { get; }

    GameObject CreepPrefab { get; }

    EconomyTransaction Bounty { get; }

    Resistances Resistances { get; }

    Creep GenerateCreep(Vector3 spawnPosition);
}