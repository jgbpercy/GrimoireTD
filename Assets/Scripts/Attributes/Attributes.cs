using System;
using System.Collections.Generic;

namespace GrimoireTD.Attributes
{
    public class Attributes<T> : IReadOnlyAttributes<T> where T : struct, IConvertible
    {
        private Dictionary<T, GameAttribute> attributesDict;

        private Dictionary<T, Action<float>> OnAttributeChangedCallbackDictionary;

        private Action<T, float> OnAnyAttributeChangedCallback;

        public Attributes(IDictionary<T, GameAttribute> attributesDict)
        {
            this.attributesDict = new Dictionary<T, GameAttribute>(attributesDict);

            OnAttributeChangedCallbackDictionary = new Dictionary<T, Action<float>>();

            foreach (T attributeName in this.attributesDict.Keys)
            {
                OnAttributeChangedCallbackDictionary.Add(attributeName, null);
            }
        }

        public void AddModifier(INamedAttributeModifier<T> attributeModifier)
        {
            GameAttribute attributeToModify;

            bool hasAttribute = attributesDict.TryGetValue(attributeModifier.AttributeName, out attributeToModify);

            if (!hasAttribute)
            {
                throw new Exception("Attempted to add a modifier to attribute " + attributeToModify + ", which entity does not have.");
            }

            attributeToModify.AddModifier(attributeModifier);

            float newAttributeValue = GetAttribute(attributeModifier.AttributeName);

            OnAttributeChangedCallbackDictionary[attributeModifier.AttributeName]?.Invoke(newAttributeValue);

            OnAnyAttributeChangedCallback?.Invoke(attributeModifier.AttributeName, newAttributeValue);
        }

        public bool TryRemoveModifier(INamedAttributeModifier<T> attributeModifier)
        {
            if (attributesDict[attributeModifier.AttributeName].TryRemoveModifier(attributeModifier))
            {
                float newAttributeValue = GetAttribute(attributeModifier.AttributeName);

                OnAttributeChangedCallbackDictionary[attributeModifier.AttributeName]?.Invoke(GetAttribute(attributeModifier.AttributeName));

                OnAnyAttributeChangedCallback?.Invoke(attributeModifier.AttributeName, newAttributeValue);

                return true;
            }

            return false;
        }

        public float GetAttribute(T attributeName)
        {
            return attributesDict[attributeName].Value();
        }

        public string TempDebugGetAttributeDisplayName(T attributeName)
        {
            return attributesDict[attributeName].DisplayName;
        }

        public void RegisterForOnAttributeChangedCallback(Action<float> callback, T attribute)
        {
            OnAttributeChangedCallbackDictionary[attribute] += callback;
        }

        public void DeregisterForOnAttributeChangedCallback(Action<float> callback, T attribute)
        {
            OnAttributeChangedCallbackDictionary[attribute] -= callback;
        }

        public void RegisterForOnAnyAttributeChangedCallback(Action<T, float> callback)
        {
            OnAnyAttributeChangedCallback += callback;
        }

        public void DeregisterForOnAnyAttributeChangedCallback(Action<T, float> callback)
        {
            OnAnyAttributeChangedCallback -= callback;
        }
    }
}