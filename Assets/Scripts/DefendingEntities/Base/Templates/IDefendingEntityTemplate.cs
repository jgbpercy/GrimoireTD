using UnityEngine;

public interface IDefendingEntityTemplate {

    IDefendingEntityImprovement BaseCharacteristics { get; }

    GameObject Prefab { get; }
}
