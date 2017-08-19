using UnityEngine;
using GrimoireTD.DefendingEntities;

public interface IDefendingEntityTemplate
{
    IDefendingEntityImprovement BaseCharacteristics { get; }

    GameObject Prefab { get; }
}
