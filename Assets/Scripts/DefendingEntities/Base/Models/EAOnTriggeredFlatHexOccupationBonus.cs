using System;
using GrimoireTD.Economy;

namespace GrimoireTD.DefendingEntities
{
    public class EAOnTriggeredFlatHexOccupationBonus : EventArgs
    {
        public readonly IDefendingEntity TriggeringEntity;

        public readonly IEconomyTransaction Transaction;

        public EAOnTriggeredFlatHexOccupationBonus(IDefendingEntity triggeringEntity, IEconomyTransaction transaction)
        {
            TriggeringEntity = triggeringEntity;
            Transaction = transaction;
        }
    }
}