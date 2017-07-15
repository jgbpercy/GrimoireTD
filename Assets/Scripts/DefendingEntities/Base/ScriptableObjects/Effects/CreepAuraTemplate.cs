using UnityEngine;

public class CreepAuraTemplate : DefenderEffectTemplate
{

    [SerializeField]
    private float baseRange;

    public float BaseRange
    {
        get
        {
            return baseRange;
        }
    }
}
