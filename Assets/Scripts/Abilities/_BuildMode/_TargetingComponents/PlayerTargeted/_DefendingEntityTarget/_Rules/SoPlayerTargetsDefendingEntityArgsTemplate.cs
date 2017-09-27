using System;
using UnityEngine;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SoPlayerTargetsDefendingEntityArgsTemplate : ScriptableObject, IPlayerTargetsDefendingEntityArgsTemplate
    {
        public virtual PlayerTargetsDefendingEntityArgs GenerateArgs(
            IDefendingEntity sourceEntity,
            IDefendingEntity targetEntity,
            IReadOnlyMapData mapData
        )
        {
            throw new Exception("SoBuildModeAbilityDefendingEntityTargetedArgsTemplate is pseudo abstract. Use inheriting classes to generate args.");
        }
    }
}