using System;

namespace GrimoireTD.Attributes
{
    public interface IReadOnlyAttributes<T>
    {
        float GetAttribute(T attributeName);

        string TempDebugGetAttributeDisplayName(T attrbuteName);

        void RegisterForOnAttributeChangedCallback(Action<float> callback, T attribute);

        void DeregisterForOnAttributeChangedCallback(Action<float> callback, T attribute);

        void RegisterForOnAnyAttributeChangedCallback(Action<T, float> callback);

        void DeregisterForOnAnyAttributeChangedCallback(Action<T, float> callback);
    }
}