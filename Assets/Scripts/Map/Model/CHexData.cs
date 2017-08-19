using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.DefendingEntities.DefenderEffects;

namespace GrimoireTD.Map
{
    public class CHexData : IHexData
    {
        private IHexType hexType;

        private IStructure structureHere = null;

        private IUnit unitHere = null;

        private CallbackList<IDefenderAura> defenderAurasHere;

        public IHexType HexType
        {
            get
            {
                return hexType;
            }
        }

        public IStructure StructureHere
        {
            get
            {
                return structureHere;
            }
        }

        public IUnit UnitHere
        {
            get
            {
                return unitHere;
            }
        }

        public IReadOnlyCollection<IDefenderAura> DefenderAurasHere
        {
            get
            {
                return defenderAurasHere;
            }
        }

        //TODO: publics are hangover from early pathing prototype - fix (just move into pathing service itself?)
        public float pathingFScore { get; set; }
        public float pathingGScore { get; set; }
        public Coord pathingCameFrom { get; set; }

        public CHexData(IHexType createWithType)
        {
            hexType = createWithType;
            ResetPathingData();

            defenderAurasHere = new CallbackList<IDefenderAura>();
        }

        public void ResetPathingData()
        {
            pathingFScore = Mathf.Infinity;
            pathingGScore = Mathf.Infinity;
            pathingCameFrom = null;
        }

        //Public non-changing helpers
        public bool IsPathableByCreeps()
        {
            return hexType.TypeIsPathableByCreeps && IsEmpty();
        }

        public bool IsPathableByCreepsWithUnitRemoved()
        {
            return hexType.TypeIsPathableByCreeps && structureHere == null;
        }

        public bool IsPathableByCreepsWithTypePathable()
        {
            return IsEmpty();
        }

        public bool IsPathableByCreepsWithStructureRemoved()
        {
            return hexType.TypeIsPathableByCreeps && unitHere == null;
        }

        public bool CanPlaceStructureHere()
        {
            return hexType.IsBuildable && structureHere == null;
        }

        public bool CanPlaceUnitHere()
        {
            return hexType.UnitCanOccupy && unitHere == null;
        }

        public bool IsEmpty()
        {
            return structureHere == null && unitHere == null;
        }

        //Public change methods
        public void AddStructureHere(IStructure structureAdded)
        {
            structureHere = structureAdded;
        }

        public void PlaceUnitHere(IUnit unitPlaced)
        {
            unitHere = unitPlaced;
        }

        public void RemoveUnitHere()
        {
            unitHere = null;
        }

        public void AddDefenderAura(IDefenderAura aura)
        {
            defenderAurasHere.Add(aura);

            aura.RegisterForOnClearAuraCallback(OnClearDefenderAura);
        }

        private void OnClearDefenderAura(IDefenderAura aura)
        {
            defenderAurasHere.TryRemove(aura);

            aura.DeregisterForOnClearAuraCallback(OnClearDefenderAura);
        }

        //Callbacks
        public void RegisterForOnDefenderAuraAddedCallback(Action<IDefenderAura> callback)
        {
            defenderAurasHere.RegisterForAdd(callback);
        }

        public void DeregisterForOnDefenderAuraAddedCallback(Action<IDefenderAura> callback)
        {
            defenderAurasHere.DeregisterForAdd(callback);
        }

        public void RegisterForOnDefenderAuraRemovedCallback(Action<IDefenderAura> callback)
        {
            defenderAurasHere.RegisterForRemove(callback);
        }

        public void DeregisterForOnDefenderAuraRemovedCallback(Action<IDefenderAura> callback)
        {
            defenderAurasHere.DeregisterForRemove(callback);
        }
    }
}