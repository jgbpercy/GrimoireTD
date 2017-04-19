using UnityEngine;
using UnityEngine.UI;

public class CreepComponent : MonoBehaviour {

    private Slider healthBar;

    private Creep creepModel;

    public Creep CreepModel
    {
        get
        {
            return creepModel;
        }
    }

    public void SetUp(Creep creepModel)
    {
        this.creepModel = creepModel;

        creepModel.RegisterForOnDiedCallback(() => { Destroy(gameObject); });

        creepModel.RegisterForOnHealthChangedCallback(UpdateHealthBar);
    }

	private void Start () {

        healthBar = GetComponentInChildren<Slider>();
        healthBar.maxValue = creepModel.CreepClassTemplate.MaxHitpoints;
        healthBar.value = healthBar.maxValue;
    }

    private void Update () {

        transform.position = creepModel.Position;
    }

    private void UpdateHealthBar()
    {
        healthBar.value = creepModel.CurrentHitpoints;
    }

    private void OnDestroy()
    {
        creepModel.GameObjectDestroyed();
    }

}
