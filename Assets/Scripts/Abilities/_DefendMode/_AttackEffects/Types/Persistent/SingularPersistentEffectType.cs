using UnityEngine;

[CreateAssetMenu(
    fileName = "NewSingularInstantEffectType",
    menuName = "Attack Effects/Singular Persistent"
)]
public class SingularPersistentEffectType : PersistentEffectType {

    [SerializeField]
    private SingularPersistentType singularPersistentType;

    public SingularPersistentType SingularPersistentType
    {
        get
        {
            return singularPersistentType;
        }
    }
}
