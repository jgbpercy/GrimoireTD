using System.Collections.Generic;

namespace GrimoireTD.Attributes
{
    public static class CreepAttributes
    {
        public static readonly Dictionary<CreepAttrName, string> DisplayNames = new Dictionary<CreepAttrName, string>
        {
            { CreepAttrName.rawArmor, "Raw Armor" },
            { CreepAttrName.armorMultiplier, "Armor Multiplier" },
            { CreepAttrName.rawSpeed, "Raw Speed" },
            { CreepAttrName.speedMultiplier, "Speed Multiplier" }
        };

        public static Dictionary<CreepAttrName, IAttribute> NewAttributesDictionary()
        {
            return new Dictionary<CreepAttrName, IAttribute>
            {
                { CreepAttrName.rawArmor, new CAdditiveAttribute(DisplayNames[CreepAttrName.rawArmor]) },
                { CreepAttrName.armorMultiplier, new CAdditiveAttribute(DisplayNames[CreepAttrName.armorMultiplier]) },
                { CreepAttrName.rawSpeed, new CAdditiveAttribute(DisplayNames[CreepAttrName.rawSpeed]) },
                { CreepAttrName.speedMultiplier, new CAdditiveAttribute(DisplayNames[CreepAttrName.speedMultiplier]) }
            };
        }
    }
}