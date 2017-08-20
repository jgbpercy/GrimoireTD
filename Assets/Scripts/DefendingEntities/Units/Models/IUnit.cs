using System;
using System.Collections.Generic;
using GrimoireTD.Economy;
using GrimoireTD.Map;

namespace GrimoireTD.DefendingEntities.Units
{
    public interface IUnit : IDefendingEntity
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

        IEnumerable<IHexOccupationBonus> ConditionalHexOccupationBonuses { get; }
        IEnumerable<IStructureOccupationBonus> ConditionalStructureOccupationBonuses { get; }

        void Move(Coord targetCoord);

        void RegenerateCachedDisallowedMovementDestinations();

        void TrackTime(bool wasIdle, float time);

        bool TryLevelUp(IUnitTalent talentChosen);

        void TempDebugAddExperience();

        IEconomyTransaction GetConditionalHexOccupationBonus(IHexType hexType);

        void RegisterForExperienceFatigueChangedCallback(Action callback);
        void DeregisterForExperienceFatigueChangedCallback(Action callback);

        void RegisterForOnConditionalHexOccupationBonusAddedCallback(Action<IHexOccupationBonus> callback);
        void DeregisterForOnConditionalHexOccupationBonusAddedCallback(Action<IHexOccupationBonus> callback);

        void RegisterForOnConditionalHexOccupationBonusRemovedCallback(Action<IHexOccupationBonus> callback);
        void DeregisterForOnConditionalHexOccupationBonusRemovedCallback(Action<IHexOccupationBonus> callback);

        void RegisterForOnConditionalStructureOccupationBonusAddedCallback(Action<IStructureOccupationBonus> callback);
        void DeregisterForOnConditionalStructureOccupationBonusAddedCallback(Action<IStructureOccupationBonus> callback);

        void RegisterForOnConditionalStructureOccupationBonusRemovedCallback(Action<IStructureOccupationBonus> callback);
        void DeregisterForOnConditionalStructureOccupationBonusRemovedCallback(Action<IStructureOccupationBonus> callback);

        void RegisterForOnMovedCallback(Action<Coord> callback);
        void DeregisterForOnMovedCallback(Action<Coord> callback);

        void RegisterForOnTriggeredConditionalOccupationBonusesCallback(Action<IUnit, IEconomyTransaction, IEconomyTransaction> callback);
        void DeregisterForOnTriggeredConditionalOccupationBonusesCallback(Action<IUnit, IEconomyTransaction, IEconomyTransaction> callback);
    }
}