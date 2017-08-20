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

        IEconomyTransaction Bounty { get; }

        IBaseResistances BaseResistances { get; }

        ICreep GenerateCreep(Vector3 spawnPosition);
    }
}