using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Defenders.Structures;
using GrimoireTD.Defenders.Units;
using GrimoireTD.Defenders.DefenderEffects;

namespace GrimoireTD.Map
{
    public class CHexData : IHexData
    {
        private CallbackList<IDefenderAura> defenderAurasHere;

        public IHexType HexType { get; }

        public IStructure StructureHere { get; private set; }

        public IUnit UnitHere { get; private set; }

        public IReadOnlyCallbackList<IDefenderAura> DefenderAurasHere
        {
            get
            {
                return defenderAurasHere;
            }
        }

        public CHexData(IHexType createWithType)
        {
            HexType = createWithType;

            defenderAurasHere = new CallbackList<IDefenderAura>();
        }

        //Public non-changing helpers
        public bool IsPathableByCreeps()
        {
            return HexType.IsPathableByCreeps && IsEmpty();
        }

        public bool IsPathableByCreepsWithUnitRemoved()
        {
            return HexType.IsPathableByCreeps && StructureHere == null;
        }

        public bool IsPathableByCreepsWithTypePathable()
        {
            return IsEmpty();
        }

        public bool IsPathableByCreepsWithStructureRemoved()
        {
            return HexType.IsPathableByCreeps && UnitHere == null;
        }

        public bool CanBuildStructureHere()
        {
            return HexType.IsBuildable && StructureHere == null;
        }

        public bool CanPlaceUnitHere()
        {
            return HexType.UnitCanOccupy && UnitHere == null;
        }

        public bool IsEmpty()
        {
            return StructureHere == null && UnitHere == null;
        }

        //Public change methods
        public void BuildStructureHere(IStructure structureAdded)
        {
            StructureHere = structureAdded;
        }

        public void PlaceUnitHere(IUnit unitPlaced)
        {
            UnitHere = unitPlaced;
        }

        public void RemoveUnitHere()
        {
            UnitHere = null;
        }

        public void AddDefenderAura(IDefenderAura aura)
        {
            defenderAurasHere.Add(aura);

            aura.OnClearDefenderAura += OnClearDefenderAura;
        }

        private void OnClearDefenderAura(object sender, EAOnClearDefenderAura args)
        {
            defenderAurasHere.TryRemove(args.ClearedAura);
            
            args.ClearedAura.OnClearDefenderAura -= OnClearDefenderAura;
        }
    }
}