using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStructure", menuName = "Structures and Units/Unit")]
public class UnitTemplate : DefendingEntityTemplate {

    public virtual Unit GenerateUnit(Vector3 position)
    {
        return new Unit(this, position);
    }
}
