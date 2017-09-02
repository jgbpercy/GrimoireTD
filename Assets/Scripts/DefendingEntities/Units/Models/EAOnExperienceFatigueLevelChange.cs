using System;

namespace GrimoireTD.DefendingEntities.Units
{
    public class EAOnExperienceFatigueLevelChange : EventArgs
    {
        public readonly int NewExperienceValue;

        public readonly int NewFatigueValue;

        public readonly int NewLevel;

        public readonly int NewLevelUpsPending;

        public EAOnExperienceFatigueLevelChange(int newExperienceValue, int newFatigueValue, int newLevel, int newLevelUpsPending)
        {
            NewExperienceValue = newExperienceValue;
            NewFatigueValue = newFatigueValue;
            NewLevel = newLevel;
            NewLevelUpsPending = newLevelUpsPending;
        }
    }
}