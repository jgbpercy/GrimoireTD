using UnityEngine;

public class AttackEffectType : ScriptableObject {

    [SerializeField]
    private string effectName;

    public virtual string EffectName()
    {
        return effectName;
    }
}