using System;
using System.Collections.Generic;
using System.Linq;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Creeps;
using GrimoireTD.Defenders;
using GrimoireTD.Defenders.Structures;
using GrimoireTD.Defenders.Units;
using GrimoireTD.Map;
using GrimoireTD.UI;
using GrimoireTD.Dependencies;

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

        //TODO: this is hacky/slow? remove?
        public IEconomyTransaction ResourcesAsTransaction
        {
            get
            {
                var resourceTransactions = resources.Select(x => new CResourceTransaction(x, x.AmountOwned));

                return new CEconomyTransaction(resourceTransactions);
            }
        }

        public event EventHandler<EAOnAnyResourceChanged> OnAnyResourceChanged;

        //NOTE: Could move these ctor args to come from DepsProv, but for now prefer them to be explicit for the GameModelLoader ordering stuff
        public CEconomyManager(IReadOnlyMapData mapData, IReadOnlyCreepManager creepManager)
        {
            resources = new List<IResource>();
            resourceTemplateToResourceDict = new Dictionary<IResourceTemplate, IResource>();

            //Callbacks
            DepsProv.TheInterfaceController().OnBuildStructurePlayerAction += OnStructureBuilt;

            creepManager.OnCreepSpawned += (object spawnSender, EAOnCreepSpawned spawnArgs) =>
            {
                spawnArgs.NewCreep.OnDied += (object diedSender, EventArgs diedArgs) => OnCreepDied(spawnArgs.NewCreep);
            };

            mapData.OnStructureCreated += (object createdSender, EAOnStructureCreated createdArgs) =>
            {
                createdArgs.StructureCreated.Abilities.OnBuildModeAbilityAdded += (object abilitySender, EAOnBuildModeAbilityAdded abilityArgs) =>
                {
                    abilityArgs.BuildModeAbility.OnExecuted += OnBuildModeAbilityExecuted;
                };
                //TODO: needed? This should be pre set up?
                foreach (var buildModeAbility in createdArgs.StructureCreated.Abilities.BuildModeAbilities())
                {
                    buildModeAbility.OnExecuted += OnBuildModeAbilityExecuted;
                }

                createdArgs.StructureCreated.OnTriggeredFlatHexOccupationBonus += OnFlatHexOccupationBonusTriggered;

                createdArgs.StructureCreated.OnUpgraded += OnStructureUpgraded;
            };

            mapData.OnUnitCreated += (object createdSender, EAOnUnitCreated createdArgs) =>
            {
                createdArgs.UnitCreated.Abilities.OnBuildModeAbilityAdded += (object abilitySender, EAOnBuildModeAbilityAdded abiliyArgs) =>
                {
                    abiliyArgs.BuildModeAbility.OnExecuted += OnBuildModeAbilityExecuted;
                };
                //TODO: needed? This should be pre set up?
                foreach (var buildModeAbility in createdArgs.UnitCreated.Abilities.BuildModeAbilities())
                {
                    buildModeAbility.OnExecuted += OnBuildModeAbilityExecuted;
                }

                createdArgs.UnitCreated.OnTriggeredFlatHexOccupationBonus += OnFlatHexOccupationBonusTriggered;

                createdArgs.UnitCreated.OnTriggeredConditionalOccupationBonuses += OnConditionalOccupationBonusesTriggered;
            };
        }

        public void SetUp(IEnumerable<IResourceTemplate> resourceTemplates, IEconomyTransaction startingResources)
        {
            foreach (var resourceTemplate in resourceTemplates)
            {
                var newResource = resourceTemplate.GenerateResource();

                resources.Add(newResource);
                resourceTemplateToResourceDict.Add(resourceTemplate, newResource);

                newResource.OnResourceChanged += (object sender, EAOnResourceChanged args) => OnResourceChanged(newResource, args.ByAmount, args.ToAmount);
            }

            DoTransaction(startingResources);
        }

        public IResource GetResourceFromTemplate(IResourceTemplate resourceTemplate)
        {
            return resourceTemplateToResourceDict[resourceTemplate];
        }

        private void DoTransaction(IEconomyTransaction transaction)
        {
            //TODO: remove in release
            if (!transaction.CanDoTransaction())
            {
                throw new Exception("EconomyManager triggered a transaction that can't be done. Some code didn't check the transaction could be done.");
            }

            foreach (var resource in resources)
            {
                resource.DoTransaction(transaction.GetTransactionAmount(resource));
            }
        }

        private void OnStructureBuilt(object sender, EAOnBuildStructurePlayerAction args)
        {
            DoTransaction(args.StructureTemplate.Cost);
        }

        private void OnCreepDied(ICreep creep)
        {
            DoTransaction(creep.CreepTemplate.Bounty);
        }

        private void OnBuildModeAbilityExecuted(object sender, EAOnExecutedBuildModeAbility args)
        {
            DoTransaction(args.ExecutedAbility.BuildModeAbilityTemplate.Cost);
        }

        private void OnFlatHexOccupationBonusTriggered(object sender, EAOnTriggeredFlatHexOccupationBonus args)
        {
            DoTransaction(args.Transaction);
        }

        private void OnConditionalOccupationBonusesTriggered(object sender, EAOnTriggeredConditionalOccupationBonus args)
        {
            DoTransaction(args.HexTransaction);
            DoTransaction(args.StructureTransaction);
        }

        private void OnStructureUpgraded(object sender, EAOnUpgraded args)
        {
            DoTransaction(args.Enhancement.Cost);
        }

        private void OnResourceChanged(IResource resource, int byAmount, int newAmount)
        {
            OnAnyResourceChanged?.Invoke(this, new EAOnAnyResourceChanged(resource, byAmount, newAmount));
        }
    }
}