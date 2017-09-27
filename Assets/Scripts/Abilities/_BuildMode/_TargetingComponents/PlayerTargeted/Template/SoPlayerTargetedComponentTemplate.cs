using System;
using UnityEngine;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SoPlayerTargetedComponentTemplate : SoBuildModeTargetingComponentTemplate, IPlayerTargetedComponentTemplate
    {
        [SerializeField]
        private SoBuildModeAutoTargetedArgsTemplate aoeRule;

        public IBuildModeAutoTargetedArgsTemplate AoeRule
        {
            get
            {
                return aoeRule;
            }
        }
    }
}