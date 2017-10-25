using System;
using UnityEngine;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SoBuildModeAutoTargetedArgsTemplate : ScriptableObject, IBuildModeAutoTargetedArgsTemplate
    {
        public virtual BuildModeAutoTargetedArgs GenerateArgs(Coord targetCoord)
        {
            throw new Exception("SoBuildModeAbilityHexTargetedArgsTemplate is pseudo abstract. Use inheriting classes to generate args.");
        }
    }
}