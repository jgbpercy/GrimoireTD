using UnityEngine;

public abstract class Ability {

    protected AbilityTemplate template;

    public Ability(AbilityTemplate template, DefendingEntity attachedToDefendingEntity)
    {
        this.template = template;
    }

    public abstract string UIText();
}
