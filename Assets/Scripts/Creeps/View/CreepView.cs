using UnityEngine;

public class CreepView : SingletonMonobehaviour<CreepView> {

    [SerializeField]
    private Transform creepFolder;

    public void CreateCreep(Creep creepModel)
    {
        CreepComponent creepComponent = Instantiate(creepModel.CreepTemplate.CreepPrefab, creepModel.Position, Quaternion.identity, creepFolder).GetComponent<CreepComponent>();

        creepComponent.SetUp(creepModel);
    }

}
