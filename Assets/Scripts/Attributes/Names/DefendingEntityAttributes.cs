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

        public static Dictionary<DefendingEntityAttributeName, IAttribute> NewAttributesDictionary()
        {
            return new Dictionary<DefendingEntityAttributeName, IAttribute>
            {
                { DefendingEntityAttributeName.rangeBonus, new CAdditiveAttribute(DisplayNames[DefendingEntityAttributeName.rangeBonus]) },
                { DefendingEntityAttributeName.damageBonus, new CAdditiveAttribute(DisplayNames[DefendingEntityAttributeName.damageBonus]) },
                { DefendingEntityAttributeName.cooldownReduction, new CDiminishingAttribute(DisplayNames[DefendingEntityAttributeName.cooldownReduction]) }
            };
        }
    }
}