using UnityEngine;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.Technical;

namespace GrimoireTD.DefendingEntities
{
    //TODO: Change these dumb things to hook into some central management callback so the model isn't talking to them
    public class DefendingEntityView : SingletonMonobehaviour<DefendingEntityView>
    {
        [SerializeField]
        private Transform structureFolder;

        [SerializeField]
        private Transform unitFolder;

        public void CreateStructure(IStructure structureModel, Vector3 position)
        {
            StructureComponent structureComponent = Instantiate(structureModel.StructureTemplate.Prefab, position, Quaternion.identity, structureFolder).GetComponent<StructureComponent>();

            structureComponent.SetUp(structureModel);
        }

        public void CreateUnit(IUnit unitModel, Vector3 position)
        {
            UnitComponent unitComponent = Instantiate(unitModel.UnitTemplate.Prefab, position, Quaternion.identity, unitFolder).GetComponent<UnitComponent>();

            unitComponent.SetUp(unitModel);
        }
    }
}