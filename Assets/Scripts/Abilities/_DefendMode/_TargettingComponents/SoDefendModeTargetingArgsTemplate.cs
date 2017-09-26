using System;
using UnityEngine;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Creeps;

namespace GrimoireTD.Abilities.DefendMode
{
    public class SoDefendModeTargetingArgsTemplate : ScriptableObject, IDefendModeTargetingArgsTemplate
    {
        public virtual DefendModeTargetingArgs GenerateArgs(
            IDefendingEntity attachedToDefendingEntity,
            IReadOnlyCreepManager creepManager
        )
        {
            throw new Exception("SoDefendModeTargetingArgsTemplate is pseudo abstract. Use inheriting classes to generate args.");
        }
    }
}