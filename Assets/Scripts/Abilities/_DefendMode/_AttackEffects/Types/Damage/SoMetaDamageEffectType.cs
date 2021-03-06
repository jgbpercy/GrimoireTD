﻿using GrimoireTD.Dependencies;
using System.Collections.Generic;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public class SoMetaDamageEffectType : SoDamageEffectType, IMetaDamageEffectType
    {
        public IReadOnlyList<ISpecificDamageEffectType> SpecificDamageTypes
        {
            get
            {
                return DepsProv.TheAttackEffectTypeManager.GetSpecificDamageTypes(this);
            }
        }
    }
}