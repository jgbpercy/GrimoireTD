using System;
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

            creepModel.OnDied += (object sender, EventArgs args) => Destroy(gameObject);

            creepModel.OnHealthChanged += UpdateHealthBar;
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

        private void UpdateHealthBar(object sender, EAOnHealthChanged args)
        {
            healthBar.value = args.NewValue;
        }
    }
}