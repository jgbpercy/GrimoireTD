using System.Collections.Generic;

namespace GrimoireTD.Attributes
{
    public static class CreepAttributes
    {
        public static readonly Dictionary<CreepAttributeName, string> DisplayNames = new Dictionary<CreepAttributeName, string>
        {
            { CreepAttributeName.rawArmor, "Raw Armor" },
            { CreepAttributeName.armorMultiplier, "Armor Multiplier" },
            { CreepAttributeName.rawSpeed, "Raw Speed" },
            { CreepAttributeName.speedMultiplier, "Speed Multiplier" }
        };

        public static Dictionary<CreepAttributeName, IAttribute> NewAttributesDictionary()
        {
            return new Dictionary<CreepAttributeName, IAttribute>
            {
                { CreepAttributeName.rawArmor, new CAdditiveAttribute(DisplayNames[CreepAttributeName.rawArmor]) },
                { CreepAttributeName.armorMultiplier, new CAdditiveAttribute(DisplayNames[CreepAttributeName.armorMultiplier]) },
                { CreepAttributeName.rawSpeed, new CAdditiveAttribute(DisplayNames[CreepAttributeName.rawSpeed]) },
                { CreepAttributeName.speedMultiplier, new CAdditiveAttribute(DisplayNames[CreepAttributeName.speedMultiplier]) }
            };
        }
    }
}