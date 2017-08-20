using System.Collections.Generic;

namespace GrimoireTD.Economy
{
    public interface IEconomyManager : IReadOnlyEconomyManager
    {
        void SetUp(IEnumerable<IResourceTemplate> resourceTemplates, IEconomyTransaction startingResources);
    }
}