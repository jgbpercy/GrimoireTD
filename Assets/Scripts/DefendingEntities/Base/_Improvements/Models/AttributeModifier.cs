using System;
using UnityEngine;

[Serializable]
public class AttributeModifier
{
    [SerializeField]
    private float magnitude;

    public float Magnitude
    {
        get
        {
            return magnitude;
        }
    }

    public AttributeModifier(float magnitude)
    {
        this.magnitude = magnitude;
    }

    //?
    /*
    public void ChangeMagnitude(float newMagnitude)
    {
        magnitude = newMagnitude;
    }
    */

    public string AsPercentString
    {
        get
        {
            return magnitude * 100 + "%";
        }
    }
}

[Serializable]
public class NamedAttributeModifier
{
    [SerializeField]
    private AttributeName attributeName;
    [SerializeField]
    private AttributeModifier attributeModifier;

    public AttributeName AttributeName
    {
        get
        {
            return attributeName;
        }
    }

    public AttributeModifier AttributeModifier
    {
        get
        {
            return attributeModifier;
        }
    }

}