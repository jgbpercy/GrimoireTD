using UnityEngine;

public class DefendingEntityView : SingletonMonobehaviour<DefendingEntityView> {

    [SerializeField]
    private Transform structureFolder;

    [SerializeField]
    private Transform unitFolder;

    public void CreateStructure(Structure structureModel, Vector3 position)
    {
        StructureComponent structureComponent = Instantiate(structureModel.StructureClassTemplate.Prefab, position, Quaternion.identity, structureFolder).GetComponent<StructureComponent>();

        structureComponent.SetUp(structureModel);
    }
    
    public void CreateUnit(Unit unitModel, Vector3 position)
    {
        UnitComponent unitComponent = Instantiate(unitModel.UnitClassTemplate.Prefab, position, Quaternion.identity, unitFolder).GetComponent<UnitComponent>();

        unitComponent.SetUp(unitModel);
    }
}
