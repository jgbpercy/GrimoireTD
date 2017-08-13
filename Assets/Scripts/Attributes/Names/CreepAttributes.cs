using System.Collections.Generic;

namespace GrimoireTD.Attributes
{
    public static class CreepAttributes
    {
        public static readonly Dictionary<CreepAttributeName, string> DisplayNames = new Dictionary<CreepAttributeName, string>
        {
            { CreepAttributeName.armor, "Armor" }
        };

        public static Dictionary<CreepAttributeName, GameAttribute> NewAttributesDictionary()
        {
            return new Dictionary<CreepAttributeName, GameAttribute>
            {
                { CreepAttributeName.armor, new AdditiveAttribute(DisplayNames[CreepAttributeName.armor]) }
            };
        }
    }
}