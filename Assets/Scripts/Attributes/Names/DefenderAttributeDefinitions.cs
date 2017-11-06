using System.Collections.Generic;

namespace GrimoireTD.Attributes
{
    public static class DefenderAttributeDefinitions
    {
        public static readonly Dictionary<DeAttrName, string> DisplayNames = new Dictionary<DeAttrName, string>
        {
            { DeAttrName.rangeBonus, "Range Bonus" },
            { DeAttrName.damageBonus, "Damage Bonus" },
            { DeAttrName.cooldownReduction, "Cooldown Reduction" }
        };

        public static Dictionary<DeAttrName, IAttribute> NewAttributesDictionary()
        {
            return new Dictionary<DeAttrName, IAttribute>
            {
                { DeAttrName.rangeBonus, new CAdditiveAttribute(DisplayNames[DeAttrName.rangeBonus]) },
                { DeAttrName.damageBonus, new CAdditiveAttribute(DisplayNames[DeAttrName.damageBonus]) },
                { DeAttrName.cooldownReduction, new CDiminishingAttribute(DisplayNames[DeAttrName.cooldownReduction]) }
            };
        }
    }
}