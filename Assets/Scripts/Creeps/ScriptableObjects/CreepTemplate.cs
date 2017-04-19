using UnityEngine;

[CreateAssetMenu(fileName = "NewCreep", menuName = "Creeps/Creep")]
public class CreepTemplate : ScriptableObject {

    [SerializeField]
    private string nameInGame;

    [SerializeField]
    private float baseSpeed;

    [SerializeField]
    private int maxHitpoints;

    [SerializeField]
    private GameObject creepPrefab;

    [SerializeField]
    private EconomyTransaction bounty;

    public string NameInGame
    {
        get
        {
            return nameInGame;
        }
    }

    public float BaseSpeed
    {
        get
        {
            return baseSpeed;
        }
    }

    public int MaxHitpoints
    {
        get
        {
            return maxHitpoints;
        }
    }

    public GameObject CreepPrefab
    {
        get
        {
            return creepPrefab;
        }
    }

    public EconomyTransaction Bounty
    {
        get
        {
            return bounty;
        }
    }

    public virtual Creep GenerateCreep(Vector3 spawnPosition)
    {
        return new Creep(this, spawnPosition);
    }

}
