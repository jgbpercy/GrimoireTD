using UnityEngine;

public class SoCreepAuraTemplate : SoDefenderEffectTemplate, ICreepAuraTemplate
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
