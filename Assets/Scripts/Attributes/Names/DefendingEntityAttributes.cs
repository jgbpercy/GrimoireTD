using System.Collections.Generic;

namespace GrimoireTD.Attributes
{
    public static class DefendingEntityAttributes
    {
        public static readonly Dictionary<DEAttrName, string> DisplayNames = new Dictionary<DEAttrName, string>
        {
            { DEAttrName.rangeBonus, "Range Bonus" },
            { DEAttrName.damageBonus, "Damage Bonus" },
            { DEAttrName.cooldownReduction, "Cooldown Reduction" }
        };

        public static Dictionary<DEAttrName, IAttribute> NewAttributesDictionary()
        {
            return new Dictionary<DEAttrName, IAttribute>
            {
                { DEAttrName.rangeBonus, new CAdditiveAttribute(DisplayNames[DEAttrName.rangeBonus]) },
                { DEAttrName.damageBonus, new CAdditiveAttribute(DisplayNames[DEAttrName.damageBonus]) },
                { DEAttrName.cooldownReduction, new CDiminishingAttribute(DisplayNames[DEAttrName.cooldownReduction]) }
            };
        }
    }
}