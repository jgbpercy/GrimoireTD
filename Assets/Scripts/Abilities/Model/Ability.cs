using UnityEngine;

public abstract class Ability {

    protected AbilityTemplate template;

    protected delegate ITargetable FindTarget(Vector3 startPosition, float range);
    protected FindTarget findTarget;

    public Ability(AbilityTemplate template)
    {
        this.template = template;
    }

    public abstract bool ExecuteAbility(Vector3 executionPosition);

    public abstract string UIText();

}
