using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Economy;
using GrimoireTD.Attributes;

namespace GrimoireTD.Creeps
{
    public interface ICreepTemplate
    {
        string NameInGame { get; }

        IEnumerable<INamedAttributeModifier<CreepAttributeName>> BaseAttributes { get; }

        int MaxHitpoints { get; }

        GameObject CreepPrefab { get; }

        IEconomyTransaction Bounty { get; }

        BaseResistances BaseResistances { get; }

        Creep GenerateCreep(Vector3 spawnPosition);
    }
}