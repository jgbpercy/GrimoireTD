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

        public static Dictionary<CreepAttributeName, GameAttribute> NewAttributesDictionary()
        {
            return new Dictionary<CreepAttributeName, GameAttribute>
            {
                { CreepAttributeName.rawArmor, new AdditiveAttribute(DisplayNames[CreepAttributeName.rawArmor]) },
                { CreepAttributeName.armorMultiplier, new AdditiveAttribute(DisplayNames[CreepAttributeName.armorMultiplier]) },
                { CreepAttributeName.rawSpeed, new AdditiveAttribute(DisplayNames[CreepAttributeName.rawSpeed]) },
                { CreepAttributeName.speedMultiplier, new AdditiveAttribute(DisplayNames[CreepAttributeName.speedMultiplier]) }
            };
        }
    }
}