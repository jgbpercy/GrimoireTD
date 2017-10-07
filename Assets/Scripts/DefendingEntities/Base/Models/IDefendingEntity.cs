using System;
using System.Collections.Generic;
using GrimoireTD.Abilities;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Abilities.DefendMode.Projectiles;
using GrimoireTD.Attributes;
using GrimoireTD.DefendingEntities.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Map;

namespace GrimoireTD.DefendingEntities
{
    public interface IDefendingEntity : IBuildModeTargetable
    {
        string Id { get; }

        IDefendingEntityTemplate DefendingEntityTemplate { get; }

        IReadOnlyAbilities Abilities { get; }

        IReadOnlyCallbackList<IDefenderAura> AurasEmitted { get; }

        IReadOnlyCallbackList<IDefenderAura> AffectedByDefenderAuras { get; }

        IReadOnlyCallbackList<IHexOccupationBonus> FlatHexOccupationBonuses { get; }

        IReadOnlyAttributes<DEAttrName> Attributes { get; }

        string CurrentName { get; }

        string UIText { get; }

        event EventHandler<EAOnProjectileCreated> OnProjectileCreated;

        event EventHandler<EAOnTriggeredFlatHexOccupationBonus> OnTriggeredFlatHexOccupationBonus;

        IEconomyTransaction GetFlatHexOccupationBonus(IHexType hexType);

        void CreatedProjectile(IProjectile projectile);
    }
}