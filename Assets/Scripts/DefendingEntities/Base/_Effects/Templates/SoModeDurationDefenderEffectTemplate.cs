using UnityEngine;

public class ModeDurationDefenderEffectTemplate : SoDefenderEffectTemplate, IDefenderEffectTemplate
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
