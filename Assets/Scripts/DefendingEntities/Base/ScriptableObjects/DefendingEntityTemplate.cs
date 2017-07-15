using UnityEngine;

public class DefendingEntityTemplate : ScriptableObject
{

    [SerializeField]
    private SoDefendingEntityImprovement baseCharacteristics;

    [SerializeField]
    protected GameObject prefab;

    public IDefendingEntityImprovement BaseCharacteristics
    {
        get
        {
            return baseCharacteristics;
        }
    }

    public GameObject Prefab
    {
        get
        {
            return prefab;
        }
    }
}
