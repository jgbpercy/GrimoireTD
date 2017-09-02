using System;

namespace GrimoireTD.Economy
{
    public class EAOnResourceCreated : EventArgs
    {
        public readonly IReadOnlyResource Resource;

        public EAOnResourceCreated(IReadOnlyResource resource)
        {
            Resource = resource;
        }
    }
}