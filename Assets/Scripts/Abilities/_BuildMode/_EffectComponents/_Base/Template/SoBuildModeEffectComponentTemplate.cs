using System;
using UnityEngine;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SoBuildModeEffectComponentTemplate : ScriptableObject, IBuildModeEffectComponentTemplate
    {
        public virtual IBuildModeEffectComponent GenerateEffectComponent()
        {
            //TODO: standardise these messages
            throw new Exception("Cannot generate component from base SoBuildModeEffectComponentTemplate; it is pseudo abstract. Use inherited classes.");
        }
    }
}
