using System;
using UnityEngine;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SoBuildModeTargetingComponentTemplate : ScriptableObject, IBuildModeTargetingComponentTemplate
    {
        public virtual IBuildModeTargetingComponent GenerateTargetingComponent()
        {
            throw new NotImplementedException("SoBuildModeTargetingComponentTemplate is a pseudo abstract base and should not be used");
        }
    }
}