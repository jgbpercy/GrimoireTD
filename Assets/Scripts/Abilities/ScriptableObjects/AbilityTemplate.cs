﻿using UnityEngine;
using System;

//[CreateAssetMenu(fileName = "NewAbilityTemplate", menuName = "Abilities")]
public class AbilityTemplate : ScriptableObject {

    [SerializeField]
    protected string nameInGame;

    public string NameInGame
    {
        get
        {
            return nameInGame;
        }
    }

    public virtual Ability GenerateAbility(DefendingEntity attachedToDefendingEntity)
    {
        throw new NotImplementedException("Cannot Generate from AbilityTemplate - it is pseudo-abstract");
    }

}
