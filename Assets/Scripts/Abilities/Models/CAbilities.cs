using System;
using System.Collections.Generic;
using System.Linq;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Dependencies;

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

            defendModeAbilityManager = DepsProv.DefendModeAbilityManager(this, attachedToDefendingEntity);
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

        //TODO: audit use of this to see if we ever legitamately call it without knowing if the ability will be present to remove
        //if not, then handle that in a more error like way
        public bool TryRemoveAbility(IAbility ability)
        {
            int indexOfAbility = abilityList.IndexOfValue(ability);

            if (indexOfAbility == -1)
            {
                return false;
            }

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

            return TryRemoveAbility(abilityToRemove);
        }

        public IReadOnlyList<IDefendModeAbility> DefendModeAbilities()
        {
            return abilityList
                .Where(kvp => kvp.Value is IDefendModeAbility)
                .Select(kvp => kvp.Value as IDefendModeAbility)
                .ToList();
        }

        public IReadOnlyList<IBuildModeAbility> BuildModeAbilities()
        {
            return abilityList
                .Where(kvp => kvp.Value is IBuildModeAbility)
                .Select(kvp => kvp.Value as IBuildModeAbility)
                .ToList();
        }
    }
}