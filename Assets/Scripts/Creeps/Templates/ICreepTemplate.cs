using UnityEngine;
using GrimoireTD.Economy;

namespace GrimoireTD.Creeps
{
    public interface ICreepTemplate
    {
        string NameInGame { get; }

        float BaseSpeed { get; }

        int MaxHitpoints { get; }

        GameObject CreepPrefab { get; }

        IEconomyTransaction Bounty { get; }

        IResistances Resistances { get; }

        Creep GenerateCreep(Vector3 spawnPosition);
    }
}