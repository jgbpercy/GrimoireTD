using System;
using System.Collections.Generic;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Abilities.DefendMode;

namespace GrimoireTD.Abilities
{
    public interface IReadOnlyAbilities
    { 
        IReadOnlyDictionary<int, IAbility> AbilityList { get; }

        IReadOnlyDefendModeAbilityManager DefendModeAbilityManager { get; }

        event EventHandler<EAOnAbilityAdded> OnAbilityAdded;
        event EventHandler<EAOnAbilityRemoved> OnAbilityRemoved;

        event EventHandler<EAOnBuildModeAbilityAdded> OnBuildModeAbilityAdded;
        event EventHandler<EAOnBuildModeAbilityRemoved> OnBuildModeAbilityRemoved;

        event EventHandler<EAOnDefendModeAbilityAdded> OnDefendModeAbilityAdded;
        event EventHandler<EAOnDefendModeAbilityRemoved> OnDefendModeAbilityRemoved;

        IReadOnlyList<IDefendModeAbility> DefendModeAbilities();
        IReadOnlyList<IBuildModeAbility> BuildModeAbilities();
    }
}