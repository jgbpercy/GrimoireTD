using System;
using System.Collections.Generic;
using GrimoireTD.Abilities;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Attributes;
using GrimoireTD.DefendingEntities.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Map;
using GrimoireTD.Abilities.DefendMode.Projectiles;

namespace GrimoireTD.DefendingEntities
{
    public interface IDefendingEntity : IBuildModeTargetable
    {
        string Id { get; }

        IDefendingEntityTemplate DefendingEntityTemplate { get; }

        IReadOnlyDictionary<int, IAbility> Abilities { get; }

        IEnumerable<IDefenderAura> AurasEmitted { get; }

        IEnumerable<IDefenderAura> AffectedByDefenderAuras { get; }

        IEnumerable<IHexOccupationBonus> FlatHexOccupationBonuses { get; }

        IReadOnlyAttributes<DefendingEntityAttributeName> Attributes { get; }

        Coord CoordPosition { get; }

        string CurrentName();

        IReadOnlyList<IDefendModeAbility> DefendModeAbilities();
        IReadOnlyList<IBuildModeAbility> BuildModeAbilities();

        IEconomyTransaction GetFlatHexOccupationBonus(IHexType hexType);

        string UIText();

        void CreatedProjectile(IProjectile projectile);

        void RegisterForOnAbilityAddedCallback(Action<IAbility> callback);
        void DeregisterForOnAbilityAddedCallback(Action<IAbility> callback);

        void RegisterForOnAbilityRemovedCallback(Action<IAbility> callback);
        void DeregisterForOnAbilityRemovedCallback(Action<IAbility> callback);

        void RegisterForOnAffectedByDefenderAuraAddedCallback(Action<IDefenderAura> callback);
        void DeregisterForOnAffectedByDefenderAuraAddedCallback(Action<IDefenderAura> callback);

        void RegisterForOnAffectedByDefenderAuraRemovedCallback(Action<IDefenderAura> callback);
        void DeregisterForOnAffectedByDefenderAuraRemovedCallback(Action<IDefenderAura> callback);

        void RegisterForOnAuraEmittedAddedCallback(Action<IDefenderAura> callback);
        void DeregisterForOnAuraEmittedAddedCallback(Action<IDefenderAura> callback);

        void RegisterForOnAuraEmittedRemovedCallback(Action<IDefenderAura> callback);
        void DeregisterForOnAuraEmittedRemovedCallback(Action<IDefenderAura> callback);

        void RegisterForOnFlatHexOccupationBonusAddedCallback(Action<IHexOccupationBonus> callback);
        void DeregisterForOnFlatHexOccupationBonusAddedCallback(Action<IHexOccupationBonus> callback);

        void RegisterForOnFlatHexOccupationBonusRemovedCallback(Action<IHexOccupationBonus> callback);
        void DeregisterForOnFlatHexOccupationBonusRemovedCallback(Action<IHexOccupationBonus> callback);

        void RegisterForOnProjectileCreatedCallback(Action<IProjectile> callback);
        void DeregisterForOnProjectileCreatedCallback(Action<IProjectile> callback);

        void RegisterForOnBuildModeAbilityExecutedCallback(Action<IBuildModeAbility> callback);
        void DeregisterForOnBuildModeAbilityExecutedCallback(Action<IBuildModeAbility> callback);

        void RegisterForOnTriggeredFlatHexOccupationBonusCallback(Action<IDefendingEntity, IEconomyTransaction> callback);
        void DregisterForOnTriggeredFlatHexOccupationBonusCallback(Action<IDefendingEntity, IEconomyTransaction> callback);
    }
}