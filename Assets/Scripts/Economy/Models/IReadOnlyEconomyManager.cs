using System;
using System.Collections.Generic;

namespace GrimoireTD.Economy
{
    public interface IReadOnlyEconomyManager
    {
        IReadOnlyCollection<IReadOnlyResource> Resources { get; }

        IEconomyTransaction ResourcesAsTransaction { get; }

        IResource GetResourceFromTemplate(IResourceTemplate resourceTemplate);

        void RegisterForOnResourceCreatedCallback(Action<IResource> callback);
        void DeregisterForOnResourceCreatedCallback(Action<IResource> callback);

        void RegisterForOnAnyResourceChangedCallback(Action<IResource, int, int> callback);
        void DeregisterForOnAnyResourceChangedCallback(Action<IResource, int, int> callback);
    }
}