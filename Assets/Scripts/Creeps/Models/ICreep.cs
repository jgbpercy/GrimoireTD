using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.Attributes;
using GrimoireTD.DefendingEntities;
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

        IReadOnlyAttributes<CreepAttributeName> Attributes { get; }

        IReadOnlyResistances Resistances { get; }

        int CurrentHitpoints { get; }

        float CurrentArmor { get; }
        float CurrentSpeed { get; }

        void ApplyAttackEffects(IEnumerable<IAttackEffect> attackEffects, IDefendingEntity sourceDefendingEntity);

        void ModelObjectFrameUpdate();
        void GameObjectDestroyed();

        void RegisterForOnHealthChangedCallback(Action callback);
        void DeregisterForOnHealthChangedCallback(Action callback);
    }
}