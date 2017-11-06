using System;
using UnityEngine;
using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SoPlayerTargetsDefenderArgsTemplate : ScriptableObject, IPlayerTargetsDefenderArgsTemplate
    {
        public virtual PlayerTargetsDefenderArgs GenerateArgs(
            IDefender sourceDefender,
            IDefender targetDefender
        )
        {
            throw new Exception("SoBuildModeAbilityDefenderTargetedArgsTemplate is pseudo abstract. Use inheriting classes to generate args.");
        }
    }
}