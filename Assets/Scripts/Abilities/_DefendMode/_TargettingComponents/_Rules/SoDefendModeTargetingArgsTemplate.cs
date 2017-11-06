using System;
using UnityEngine;
using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities.DefendMode
{
    public class SoDefendModeTargetingArgsTemplate : ScriptableObject, IDefendModeTargetingArgsTemplate
    {
        public virtual DefendModeTargetingArgs GenerateArgs(
            IDefender attachedToDefender
        )
        {
            throw new Exception("SoDefendModeTargetingArgsTemplate is pseudo abstract. Use inheriting classes to generate args.");
        }
    }
}