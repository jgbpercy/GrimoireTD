using System;

namespace GrimoireTD.Technical
{
    public interface IModelObjectFrameUpdater
    {
        void Register(Action<float> updateAction);
        void Deregister(Action<float> updateAction);
    }
}