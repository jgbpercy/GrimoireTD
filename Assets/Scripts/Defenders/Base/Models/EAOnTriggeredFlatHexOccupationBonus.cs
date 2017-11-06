using System;
using GrimoireTD.Economy;

namespace GrimoireTD.Defenders
{
    public class EAOnTriggeredFlatHexOccupationBonus : EventArgs
    {
        public readonly IDefender TriggeringDefender;

        public readonly IEconomyTransaction Transaction;

        public EAOnTriggeredFlatHexOccupationBonus(IDefender triggeringDefender, IEconomyTransaction transaction)
        {
            TriggeringDefender = triggeringDefender;
            Transaction = transaction;
        }
    }
}