using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Creeps;
using GrimoireTD.DefendingEntities;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.Map;
using GrimoireTD.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GrimoireTD.Economy
{
    public class CEconomyManager : IEconomyManager
    {
        private List<IResource> resources;

        private Dictionary<IResourceTemplate, IResource> resourceTemplateToResourceDict;

        public IReadOnlyCollection<IReadOnlyResource> Resources
        {
            get
            {
                return resources;
            }
        }

        //TODO: this is bit hacky/slow? remove?
        public IEconomyTransaction ResourcesAsTransaction
        {
            get
            {
                var resourceTransactions = resources.Select(x => new CResourceTransaction(x, x.AmountOwned));

                return new CEconomyTransaction(resourceTransactions);
            }
        }

        private Action<IResource> OnResourceCreatedCallback;

        private Action<IResource, int, int> OnAnyResourceChangedCallback;

        public CEconomyManager(IReadOnlyMapData mapData, IReadOnlyCreepManager creepManager)
        {
            resources = new List<IResource>();
            resourceTemplateToResourceDict = new Dictionary<IResourceTemplate, IResource>();

            //Callbacks
            InterfaceController.Instance.RegisterForOnBuildStructureUserAction(OnStructureBuilt);

            creepManager.RegisterForCreepSpawnedCallback(creep =>
            {
                creep.RegisterForOnDiedCallback(() =>
                {
                    OnCreepDied(creep);
                });
            });

            mapData.RegisterForOnStructureCreatedCallback((structure, coord) =>
            {
                structure.RegisterForOnBuildModeAbilityExecutedCallback(OnBuildModeAbilityExecuted);
                structure.RegisterForOnTriggeredFlatHexOccupationBonusCallback(OnFlatHexOccupationBonusTriggered);
                structure.RegisterForOnUpgradedCallback(OnStructureUpgraded);
            });

            mapData.RegisterForOnUnitCreatedCallback((unit, coord) =>
            {
                unit.RegisterForOnBuildModeAbilityExecutedCallback(OnBuildModeAbilityExecuted);
                unit.RegisterForOnTriggeredFlatHexOccupationBonusCallback(OnFlatHexOccupationBonusTriggered);
                unit.RegisterForOnTriggeredConditionalOccupationBonusesCallback(OnConditionalOccupationBonusesTriggered);
            });
        }

        public void SetUp(IEnumerable<IResourceTemplate> resourceTemplates, IEconomyTransaction startingResources)
        {
            foreach (IResourceTemplate resourceTemplate in resourceTemplates)
            {
                IResource newResource = new CResource(resourceTemplate);

                resources.Add(newResource);
                resourceTemplateToResourceDict.Add(resourceTemplate, newResource);

                newResource.RegisterForOnResourceChangedCallback((int byAmount, int newAmount) => OnResourceChanged(newResource, byAmount, newAmount));
            }

            DoTransaction(startingResources);
        }

        public IResource GetResourceFromTemplate(IResourceTemplate resourceTemplate)
        {
            return resourceTemplateToResourceDict[resourceTemplate];
        }

        private void DoTransaction(IEconomyTransaction transaction)
        {
            if (!transaction.CanDoTransaction())
            {
                throw new Exception("EconomyManager triggered a transaction that can't be done. Some code didn't check the transaction could be done.");
            }

            foreach (IResource resource in resources)
            {
                resource.DoTransaction(transaction.GetTransactionAmount(resource));
            }
        }

        private void OnStructureBuilt(Coord coord, IStructureTemplate structureTemplate)
        {
            DoTransaction(structureTemplate.Cost);
        }

        private void OnCreepDied(ICreep creep)
        {
            DoTransaction(creep.CreepTemplate.Bounty);
        }

        private void OnBuildModeAbilityExecuted(IBuildModeAbility buildModeAbility)
        {
            DoTransaction(buildModeAbility.BuildModeAbilityTemplate.Cost);
        }

        private void OnFlatHexOccupationBonusTriggered(IDefendingEntity defendingEntity, IEconomyTransaction bonus)
        {
            DoTransaction(bonus);
        }

        private void OnConditionalOccupationBonusesTriggered(IUnit unit, IEconomyTransaction hexBonus, IEconomyTransaction structureBonus)
        {
            DoTransaction(hexBonus);
            DoTransaction(structureBonus);
        }

        private void OnStructureUpgraded(IStructureUpgrade upgrade, IStructureEnhancement enhancement)
        {
            DoTransaction(enhancement.Cost);
        }

        private void OnResourceChanged(IResource resource, int byAmount, int newAmount)
        {
            OnAnyResourceChangedCallback?.Invoke(resource, byAmount, newAmount);
        }

        public void RegisterForOnResourceCreatedCallback(Action<IResource> callback)
        {
            OnResourceCreatedCallback += callback;
        }

        public void DeregisterForOnResourceCreatedCallback(Action<IResource> callback)
        {
            OnResourceCreatedCallback -= callback;
        }

        public void RegisterForOnAnyResourceChangedCallback(Action<IResource, int, int> callback)
        {
            OnAnyResourceChangedCallback += callback;
        }

        public void DeregisterForOnAnyResourceChangedCallback(Action<IResource, int, int> callback)
        {
            OnAnyResourceChangedCallback -= callback;
        }
    }
}