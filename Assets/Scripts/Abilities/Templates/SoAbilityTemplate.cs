using System;
using UnityEngine;
using GrimoireTD.DefendingEntities;

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

        public virtual Ability GenerateAbility(DefendingEntity attachedToDefendingEntity)
        {
            throw new NotImplementedException("Cannot Generate from AbilityTemplate - it is pseudo-abstract");
        }
    }
}