using System;
using UnityEngine;
using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities
{
    public class SoAbilityTemplate : ScriptableObject, IAbilityTemplate
    {
        [SerializeField]
        protected string nameInGame;

        public string NameInGame
        {
            get
            {
                return nameInGame;
            }
        }

        public virtual IAbility GenerateAbility(IDefender attachedToDefender)
        {
            throw new NotImplementedException("Cannot Generate from AbilityTemplate - it is pseudo-abstract");
        }
    }
}