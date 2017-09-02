using UnityEngine;
using GrimoireTD.Map;

namespace GrimoireTD.DefendingEntities.Units
{
    public class UnitComponent : MonoBehaviour
    {
        private IUnit unitModel;

        public IUnit UnitModel
        {
            get
            {
                return unitModel;
            }
        }

        public void SetUp(IUnit unitModel)
        {
            this.unitModel = unitModel;

            unitModel.OnMoved += OnMoved;
        }

        private void OnMoved(object sender, EAOnMoved args)
        {
            transform.position = args.ToPosition.ToPositionVector();
        }
    }
}