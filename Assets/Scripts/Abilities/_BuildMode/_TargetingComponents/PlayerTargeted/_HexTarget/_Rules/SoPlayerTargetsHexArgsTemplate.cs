﻿using System;
using UnityEngine;
using GrimoireTD.Defenders;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SoPlayerTargetsHexArgsTemplate : ScriptableObject, IPlayerTargetsHexArgsTemplate
    {
        public virtual PlayerTargetsHexArgs GenerateArgs(
            IDefender sourceDefender, 
            Coord targetCoord
        )
        {
            throw new Exception("SoBuildModeAbilityHexTargetedArgsTemplate is pseudo abstract. Use inheriting classes to generate args.");
        }
    }
}