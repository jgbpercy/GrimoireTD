using System;
using GrimoireTD.Economy;

namespace GrimoireTD.Defenders.Units
{
    public class EAOnTriggeredConditionalOccupationBonus : EventArgs
    {
        public readonly IDefender TriggeringDefender;

        public readonly IEconomyTransaction HexTransaction;

        public readonly IEconomyTransaction StructureTransaction;

        public EAOnTriggeredConditionalOccupationBonus(IDefender triggeringDefender, IEconomyTransaction hexTransaction, IEconomyTransaction structureTransaction)
        {
            TriggeringDefender = triggeringDefender;
            HexTransaction = hexTransaction;
            StructureTransaction = structureTransaction;
        }
    }
}