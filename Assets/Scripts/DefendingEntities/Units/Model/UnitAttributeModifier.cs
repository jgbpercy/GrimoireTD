using System;
using UnityEngine;

[Serializable]
public class UnitAttributeModifier
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

    public UnitAttributeModifier(float magnitude)
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
public class UnitAttributeNamedModifier
{
    [SerializeField]
    private UnitAttributeName unitAttributeName;
    [SerializeField]
    private UnitAttributeModifier unitAttributeModifier;

    public UnitAttributeName UnitAttributeName
    {
        get
        {
            return unitAttributeName;
        }
    }

    public UnitAttributeModifier UnitAttributeModifier
    {
        get
        {
            return unitAttributeModifier;
        }
    }

}