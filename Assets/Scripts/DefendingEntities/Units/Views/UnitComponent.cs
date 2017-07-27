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

        unitModel.RegisterForOnMovedCallback(OnMoved);
    }

    private void OnMoved(Coord coord)
    {
        transform.position = coord.ToPositionVector();
    }
}
