using System.Collections.Generic;

namespace GrimoireTD.Attributes
{
    public static class DefendingEntityAttributes
    {
        public static readonly Dictionary<DefendingEntityAttributeName, string> DisplayNames = new Dictionary<DefendingEntityAttributeName, string>
        {
            { DefendingEntityAttributeName.rangeBonus, "Range Bonus" },
            { DefendingEntityAttributeName.damageBonus, "Damage Bonus" },
            { DefendingEntityAttributeName.cooldownReduction, "Cooldown Reduction" }
        };

        public static Dictionary<DefendingEntityAttributeName, GameAttribute> NewAttributesDictionary()
        {
            return new Dictionary<DefendingEntityAttributeName, GameAttribute>
            {
                { DefendingEntityAttributeName.rangeBonus, new AdditiveAttribute(DisplayNames[DefendingEntityAttributeName.rangeBonus]) },
                { DefendingEntityAttributeName.damageBonus, new AdditiveAttribute(DisplayNames[DefendingEntityAttributeName.damageBonus]) },
                { DefendingEntityAttributeName.cooldownReduction, new DiminishingAttribute(DisplayNames[DefendingEntityAttributeName.cooldownReduction]) }
            };
        }
    }
}