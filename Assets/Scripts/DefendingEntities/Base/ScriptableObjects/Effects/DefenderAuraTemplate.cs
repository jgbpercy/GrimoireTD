using UnityEngine;

[CreateAssetMenu(fileName = "NewDefenderAura", menuName = "Structures and Units/Defender Aura")]
public class DefenderAuraTemplate : DefenderEffectTemplate
{

    [SerializeField]
    private int baseRange;

    [SerializeField]
    private bool affectsSelf;

    public int BaseRange
    {
        get
        {
            return baseRange;
        }
    }

    public bool AffectsSelf
    {
        get
        {
            return affectsSelf;
        }
    }

    public DefenderAura GenerateDefenderAura(DefendingEntity sourceDefendingEntity)
    {
        return new DefenderAura(this, sourceDefendingEntity);
    }
}
