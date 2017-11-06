using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.Attributes;
using GrimoireTD.Defenders;
using GrimoireTD.TemporaryEffects;
using GrimoireTD.Abilities.DefendMode;

namespace GrimoireTD.Creeps
{
    public interface ICreep : IDefendModeTargetable
    {
        ICreepTemplate CreepTemplate { get; }

        IReadOnlyTemporaryEffectsManager TemporaryEffects { get; }

        Vector3 Position { get; }

        float DistanceFromEnd { get; }

        IReadOnlyAttributes<CreepAttrName> Attributes { get; }

        IReadOnlyResistances Resistances { get; }

        int CurrentHitpoints { get; }

        float CurrentArmor { get; }
        float CurrentSpeed { get; }
        
        event EventHandler<EAOnHealthChanged> OnHealthChanged;

        event EventHandler<EAOnAttributeChanged> OnArmorChanged;

        event EventHandler<EAOnAttributeChanged> OnSpeedChanged;

        void ApplyAttackEffects(IEnumerable<IAttackEffect> attackEffects, IDefender sourceDefender);
    }
}