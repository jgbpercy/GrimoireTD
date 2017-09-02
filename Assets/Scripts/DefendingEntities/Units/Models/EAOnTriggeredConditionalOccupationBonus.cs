using System;
using GrimoireTD.Economy;

namespace GrimoireTD.DefendingEntities.Units
{
    public class EAOnTriggeredConditionalOccupationBonus : EventArgs
    {
        public readonly IDefendingEntity TriggeringEntity;

        public readonly IEconomyTransaction HexTransaction;

        public readonly IEconomyTransaction StructureTransaction;

        public EAOnTriggeredConditionalOccupationBonus(IDefendingEntity triggeringEntity, IEconomyTransaction hexTransaction, IEconomyTransaction structureTransaction)
        {
            TriggeringEntity = triggeringEntity;
            HexTransaction = hexTransaction;
            StructureTransaction = structureTransaction;
        }
    }
}