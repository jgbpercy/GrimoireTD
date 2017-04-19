using UnityEngine;

public class CreepView : SingletonMonobehaviour<CreepView> {

    [SerializeField]
    private Transform creepFolder;

    public void CreateCreep(Creep creepModel)
    {
        CreepComponent creepComponent = Instantiate(creepModel.CreepClassTemplate.CreepPrefab, creepModel.Position, Quaternion.identity, creepFolder).GetComponent<CreepComponent>();

        creepComponent.SetUp(creepModel);
    }

}
