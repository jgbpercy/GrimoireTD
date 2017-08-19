using UnityEngine;
using UnityEngine.UI;

namespace GrimoireTD.Creeps
{
    public class CreepComponent : MonoBehaviour
    {
        private Slider healthBar;

        private ICreep creepModel;

        public ICreep CreepModel
        {
            get
            {
                return creepModel;
            }
        }

        public void SetUp(ICreep creepModel)
        {
            this.creepModel = creepModel;

            creepModel.RegisterForOnDiedCallback(() => { Destroy(gameObject); });

            creepModel.RegisterForOnHealthChangedCallback(UpdateHealthBar);
        }

        private void Start()
        {

            healthBar = GetComponentInChildren<Slider>();
            healthBar.maxValue = creepModel.CreepTemplate.MaxHitpoints;
            healthBar.value = healthBar.maxValue;
        }

        private void Update()
        {

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
}