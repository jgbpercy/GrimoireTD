using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitComponent : MonoBehaviour {

    private Unit unitModel;

    public Unit UnitModel
    {
        get
        {
            return unitModel;
        }
    }

    public void SetUp(Unit unitModel)
    {
        this.unitModel = unitModel;
    }
}
