using System;
using System.Collections.Generic;

namespace GrimoireTD.Economy
{
    public interface IReadOnlyEconomyManager
    {
        IReadOnlyCollection<IReadOnlyResource> Resources { get; }

        IEconomyTransaction ResourcesAsTransaction { get; }

        event EventHandler<EAOnAnyResourceChanged> OnAnyResourceChanged;

        IResource GetResourceFromTemplate(IResourceTemplate resourceTemplate);
    }
}