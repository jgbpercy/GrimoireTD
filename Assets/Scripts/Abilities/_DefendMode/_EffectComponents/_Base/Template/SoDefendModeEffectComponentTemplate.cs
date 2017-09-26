using System;
using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode
{
    public class SoDefendModeEffectComponentTemplate : ScriptableObject, IDefendModeEffectComponentTemplate
    {
        public virtual IDefendModeEffectComponent GenerateEffectComponent()
        {
            throw new Exception("Cannot generate component from base SoDefendModeEffectComponentTemplate; it is pseudo abstract. Use inherited classes.");
        }
    }
}