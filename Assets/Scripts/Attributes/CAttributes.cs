using System;
using System.Collections.Generic;

namespace GrimoireTD.Attributes
{
    public class CAttributes<T> : IAttributes<T> where T : struct, IConvertible
    {
        private Dictionary<T, IAttribute> attributesDict;

        public event EventHandler<EAOnAnyAttributeChanged<T>> OnAnyAttributeChanged;

        public CAttributes(IDictionary<T, IAttribute> attributesDict)
        {
            this.attributesDict = new Dictionary<T, IAttribute>(attributesDict);
        }

        public void AddModifier(INamedAttributeModifier<T> attributeModifier)
        {
            IAttribute attributeToModify;

            bool hasAttribute = attributesDict.TryGetValue(attributeModifier.AttributeName, out attributeToModify);

            if (!hasAttribute)
            {
                throw new Exception("Attempted to add a modifier to attribute " + attributeToModify + ", which entity does not have.");
            }

            attributeToModify.AddModifier(attributeModifier);

            float newAttributeValue = GetAttribute(attributeModifier.AttributeName).Value();

            OnAnyAttributeChanged?.Invoke(this, new EAOnAnyAttributeChanged<T>(attributeModifier.AttributeName, newAttributeValue));
        }

        public bool TryRemoveModifier(INamedAttributeModifier<T> attributeModifier)
        {
            if (attributesDict[attributeModifier.AttributeName].TryRemoveModifier(attributeModifier))
            {
                float newAttributeValue = GetAttribute(attributeModifier.AttributeName).Value();

                OnAnyAttributeChanged?.Invoke(this, new EAOnAnyAttributeChanged<T>(attributeModifier.AttributeName, newAttributeValue));

                return true;
            }

            return false;
        }

        public IReadOnlyAttribute GetAttribute(T attributeName)
        {
            return attributesDict[attributeName];
        }
    }
}