using UnityEngine;

public class StructureView : SingletonMonobehaviour<StructureView> {

    [SerializeField]
    private Transform structureFolder;

    public void CreateStructure(Structure structureModel, Vector3 position)
    {
        StructureComponent structureComponent = Instantiate(structureModel.StructureClassTemplate.StructurePrefab, position, Quaternion.identity, structureFolder).GetComponent<StructureComponent>();

        structureComponent.SetUp(structureModel);
    }
}
