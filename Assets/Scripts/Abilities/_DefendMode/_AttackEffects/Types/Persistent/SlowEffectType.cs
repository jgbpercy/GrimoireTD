using UnityEngine;

[CreateAssetMenu(
    fileName = "NewSlowEffectType",
    menuName = "Attack Effects/Slow"
)]
public class SlowEffectType : PersistentEffectType {

    [SerializeField]
    private SlowType slowType;

    public SlowType SlowType
    {
        get
        {
            return slowType;
        }
    }

    public override string EffectName()
    {
        return "Slow (" + base.EffectName() + ")";
    }
}
