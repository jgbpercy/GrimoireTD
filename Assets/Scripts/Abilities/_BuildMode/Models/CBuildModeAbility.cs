using System;
using System.Collections.Generic;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class CBuildModeAbility : CAbility, IBuildModeAbility
    {
        private IBuildModeAbilityTemplate buildModeAbilityTemplate;

        private Action<IBuildModeAbility> OnExecutedCallback;

        public IBuildModeAbilityTemplate BuildModeAbilityTemplate
        {
            get
            {
                return buildModeAbilityTemplate;
            }
        }

        public CBuildModeAbility(IBuildModeAbilityTemplate template) : base(template)
        {
            buildModeAbilityTemplate = template;
        }

        public void ExecuteAbility(IDefendingEntity executingEntity, Coord executionPosition)
        {
            IReadOnlyList<IBuildModeTargetable> targetList = buildModeAbilityTemplate.TargetingComponent.FindTargets(executionPosition);

            buildModeAbilityTemplate.EffectComponent.ExecuteEffect(executingEntity, targetList);

            OnExecutedCallback?.Invoke(this);
        }

        public override string UIText()
        {
            return buildModeAbilityTemplate.NameInGame;
        }

        public void RegisterForOnExecutedCallback(Action<IBuildModeAbility> callback)
        {
            OnExecutedCallback += callback;
        }

        public void DeregisterForOnExecutedCallback(Action<IBuildModeAbility> callback)
        {
            OnExecutedCallback -= callback;
        }
    }
}