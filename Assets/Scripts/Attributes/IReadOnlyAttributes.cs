using System;

namespace GrimoireTD.Attributes
{
    public interface IReadOnlyAttributes<T>
    {
        float GetAttribute(T attributeName);

        string TempDebugGetAttributeDisplayName(T attrbuteName);

        void RegisterForOnAttributeChangedCallback(Action<float> callback, T attribute);

        void DeregisterForOnAttributeChangedCallback(Action<float> callback, T attribute);

        void RegisterForOnAttributeChangedCallback(Action<T, float> callback);

        void DeregisterForOnAttributeChangedCallback(Action<T, float> callback);
    }
}