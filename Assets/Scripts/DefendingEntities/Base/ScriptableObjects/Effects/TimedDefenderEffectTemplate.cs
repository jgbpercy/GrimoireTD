using UnityEngine;

public class TimedDefenderEffectTemplate : DefenderEffectTemplate
{

    [SerializeField]
    private float baseDuration;

    public float BaseDuration
    {
        get
        {
            return baseDuration;
        }
    }
}
