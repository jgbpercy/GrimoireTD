using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendingEntityTemplate : ScriptableObject {

    [SerializeField]
    protected string nameInGame;

    [SerializeField]
    protected string description;

    [SerializeField]
    protected AbilityTemplate[] baseAbilities;

    [SerializeField]
    protected GameObject prefab;

    public string NameInGame
    {
        get
        {
            return nameInGame;
        }
    }

    public string Description
    {
        get
        {
            return description;
        }
    }

    public AbilityTemplate[] BaseAbilities
    {
        get
        {
            return baseAbilities;
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
