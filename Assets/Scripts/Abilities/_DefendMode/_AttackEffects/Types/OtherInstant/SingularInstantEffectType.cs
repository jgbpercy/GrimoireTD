using UnityEngine;

[CreateAssetMenu(
    fileName = "NewSingularInstantEffectType",
    menuName = "Attack Effects/Singular Instant"
)]
public class SingularInstantEffectType : AttackEffectType {

    [SerializeField]
    private SingularInstantType singularInstantType;

    public SingularInstantType SingularInstantType
    {
        get
        {
            return singularInstantType;
        }
    }
}
