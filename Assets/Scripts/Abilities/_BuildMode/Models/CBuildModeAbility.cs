using System;
using System.Collections.Generic;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class CBuildModeAbility : CAbility, IBuildModeAbility
    {
        public IBuildModeAbilityTemplate BuildModeAbilityTemplate { get; }

        public event EventHandler<EAOnExecutedBuildModeAbility> OnExecuted;

        public CBuildModeAbility(IBuildModeAbilityTemplate template) : base(template)
        {
            BuildModeAbilityTemplate = template;
        }

        public void ExecuteAbility(IDefendingEntity executingEntity, Coord executionPosition)
        {
            IReadOnlyList<IBuildModeTargetable> targetList = BuildModeAbilityTemplate.TargetingComponent.FindTargets(executionPosition);

            BuildModeAbilityTemplate.EffectComponent.ExecuteEffect(executingEntity, targetList);

            OnExecuted?.Invoke(this, new EAOnExecutedBuildModeAbility(this));
        }

        public override string UIText()
        {
            return BuildModeAbilityTemplate.NameInGame;
        }
    }
}