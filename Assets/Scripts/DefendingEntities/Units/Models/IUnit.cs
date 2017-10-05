﻿using System;
using System.Collections.Generic;
using GrimoireTD.Economy;
using GrimoireTD.Map;
using GrimoireTD.Technical;

namespace GrimoireTD.DefendingEntities.Units
{
    public interface IUnit : IDefendingEntity, IFrameUpdatee
    {
        IUnitTemplate UnitTemplate { get; }

        IReadOnlyList<Coord> CachedDisallowedMovementDestinations { get; }

        IReadOnlyDictionary<IUnitTalent, int> LevelledTalents { get; }

        float TimeIdle { get; }
        float TimeActive { get; }

        int Experience { get; }
        int Fatigue { get; }
        int LevelUpsPending { get; }
        int Level { get; }

        IReadOnlyCallbackList<IHexOccupationBonus> ConditionalHexOccupationBonuses { get; }
        IReadOnlyCallbackList<IStructureOccupationBonus> ConditionalStructureOccupationBonuses { get; }

        event EventHandler<EAOnMoved> OnMoved;

        event EventHandler<EAOnExperienceFatigueLevelChange> OnExperienceFatigueLevelChanged;

        event EventHandler<EAOnTriggeredConditionalOccupationBonus> OnTriggeredConditionalOccupationBonuses;

        void Move(Coord targetCoord);

        void RegenerateCachedDisallowedMovementDestinations();

        bool TryLevelUp(IUnitTalent talentChosen);

        void TempDebugAddExperience();

        IEconomyTransaction GetConditionalHexOccupationBonus(IHexType hexType);
    }
}