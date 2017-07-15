using UnityEngine;

public class DefenderEffectTemplate : ScriptableObject
{

    public enum AffectsType
    {
        UNITS,
        STRUCTURES,
        BOTH
    }

    [SerializeField]
    private AffectsType affects;

    [SerializeField]
    private string nameInGame;

    [SerializeField]
    private SoDefendingEntityImprovement improvement;

    public AffectsType Affects
    {
        get
        {
            return affects;
        }
    }

    public string NameInGame
    {
        get
        {
            return nameInGame;
        }
    }

    public IDefendingEntityImprovement Improvement
    {
        get
        {
            return improvement;
        }
    }
    
}
