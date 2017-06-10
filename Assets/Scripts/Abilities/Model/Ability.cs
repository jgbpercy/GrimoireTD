using UnityEngine;

public abstract class Ability {

    protected AbilityTemplate template;

    public Ability(AbilityTemplate template)
    {
        this.template = template;
    }

    public abstract string UIText();
}
