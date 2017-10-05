using System;
using System.Collections.Generic;
using System.Linq;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Abilities.BuildMode;

namespace GrimoireTD.Abilities
{
    public class CAbilities : IAbilities
    {
        private SortedList<int, IAbility> abilityList;

        public event EventHandler<EAOnAbilityAdded> OnAbilityAdded;
        public event EventHandler<EAOnAbilityRemoved> OnAbilityRemoved;

        public event EventHandler<EAOnBuildModeAbilityAdded> OnBuildModeAbilityAdded;
        public event EventHandler<EAOnBuildModeAbilityRemoved> OnBuildModeAbilityRemoved;

        public event EventHandler<EAOnDefendModeAbilityAdded> OnDefendModeAbilityAdded;
        public event EventHandler<EAOnDefendModeAbilityRemoved> OnDefendModeAbilityRemoved;

        private IDefendModeAbilityManager defendModeAbilityManager;

        public IReadOnlyDictionary<int, IAbility> AbilityList
        {
            get
            {
                return abilityList;
            }
        }

        public IReadOnlyDefendModeAbilityManager DefendModeAbilityManager
        {
            get
            {
                return defendModeAbilityManager;
            }
        }

        public CAbilities(IDefendingEntity attachedToDefendingEntity)
        {
            abilityList = new SortedList<int, IAbility>();

            defendModeAbilityManager = new CDefendModeAbilityManager(this, attachedToDefendingEntity);
        }

        public void AddAbility(IAbility ability)
        {
            abilityList.Add(abilityList.Count, ability);

            OnAbilityAdded?.Invoke(this, new EAOnAbilityAdded(ability));

            var buildModeAbility = ability as IBuildModeAbility;
            if (buildModeAbility != null)
            {
                OnBuildModeAbilityAdded?.Invoke(this, new EAOnBuildModeAbilityAdded(buildModeAbility));
                return;
            }

            var defendModeAbility = ability as IDefendModeAbility;
            if (defendModeAbility != null)
            {
                OnDefendModeAbilityAdded?.Invoke(this, new EAOnDefendModeAbilityAdded(defendModeAbility));
                return;
            }
        }

        public bool TryRemoveAbility(IAbility ability)
        {
            if (!abilityList.ContainsValue(ability))
            {
                return false;
            }

            int indexOfAbility = abilityList.IndexOfValue(ability);
            abilityList.RemoveAt(indexOfAbility);

            OnAbilityRemoved?.Invoke(this, new EAOnAbilityRemoved(ability));

            var buildModeAbility = ability as IBuildModeAbility;
            if (buildModeAbility != null)
            {
                OnBuildModeAbilityRemoved?.Invoke(this, new EAOnBuildModeAbilityRemoved(buildModeAbility));
                return true;
            }

            var defendModeAbility = ability as IDefendModeAbility;
            if (defendModeAbility != null)
            {
                OnDefendModeAbilityRemoved?.Invoke(this, new EAOnDefendModeAbilityRemoved(defendModeAbility));
                return true;
            }

            return true;
        }

        public bool TryRemoveAbility(IAbilityTemplate template)
        {
            var abilityToRemove = abilityList
                .Select(kvp => kvp.Value)
                .Where(x => x.AbilityTemplate == template)
                .FirstOrDefault();

            if (abilityToRemove == null)
            {
                return false;
            }

            int indexOfAbility = abilityList.IndexOfValue(abilityToRemove);
            abilityList.RemoveAt(indexOfAbility);

            OnAbilityRemoved?.Invoke(this, new EAOnAbilityRemoved(abilityToRemove));

            var buildModeAbility = abilityToRemove as IBuildModeAbility;
            if (buildModeAbility != null)
            {
                OnBuildModeAbilityRemoved?.Invoke(this, new EAOnBuildModeAbilityRemoved(buildModeAbility));
                return true;
            }

            var defendModeAbility = abilityToRemove as IDefendModeAbility;
            if (defendModeAbility != null)
            {
                OnDefendModeAbilityRemoved?.Invoke(this, new EAOnDefendModeAbilityRemoved(defendModeAbility));
                return true;
            }

            return true;
        }

        //TODO: wont this have a problem if ability removed? Change to foreach? Or just change list implementation overall?
        public IReadOnlyList<IDefendModeAbility> DefendModeAbilities()
        {
            List<IDefendModeAbility> defendModeAbilities = new List<IDefendModeAbility>();

            for (int i = 0; i < abilityList.Count; i++)
            {
                var defendModeAbility = abilityList[i] as IDefendModeAbility;
                if (defendModeAbility != null)
                {
                    defendModeAbilities.Add(defendModeAbility);
                }
            }

            return defendModeAbilities;
        }

        public IReadOnlyList<IBuildModeAbility> BuildModeAbilities()
        {
            List<IBuildModeAbility> buildModeAbilities = new List<IBuildModeAbility>();

            for (int i = 0; i < abilityList.Count; i++)
            {
                var buildModeAbility = abilityList[i] as IBuildModeAbility;
                if (buildModeAbility != null)
                {
                    buildModeAbilities.Add(buildModeAbility);
                }
            }

            return buildModeAbilities;
        }
    }
}