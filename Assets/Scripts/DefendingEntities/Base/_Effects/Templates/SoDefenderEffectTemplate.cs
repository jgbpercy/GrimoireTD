using UnityEngine;

public class SoDefenderEffectTemplate : ScriptableObject, IDefenderEffectTemplate
{
    [SerializeField]
    private DefenderEffectAffectsType affects;

    [SerializeField]
    private string nameInGame;

    [SerializeField]
    private SoDefendingEntityImprovement improvement;

    public DefenderEffectAffectsType Affects
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
