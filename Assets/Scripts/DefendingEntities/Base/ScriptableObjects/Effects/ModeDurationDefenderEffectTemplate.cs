using UnityEngine;

public class ModeDurationDefenderEffectTemplate : DefenderEffectTemplate
{

    [SerializeField]
    private int baseDuration;

    public int BaseDuration
    {
        get
        {
            return baseDuration;
        }
    }
}
