using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Economy;
using GrimoireTD.Dependencies;
using GrimoireTD.Creeps;
using GrimoireTD.Map;
using GrimoireTD.UI;
using GrimoireTD.Defenders.Structures;
using GrimoireTD.Defenders.Units;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Defenders;

namespace GrimoireTD.Tests.EconomyManagerTests
{
    public class EconomyManagerTests
    {
        //Primitives and Basic Objects
        private int resource1StartingAmount = 100;
        private int resource2StartingAmount = 50;

        int creepDiedResource1Bounty = 1;
        int creepDiedResource2Bounty = 2;

        int buildStructureResource1Cost = -3;
        int buildStructureResource2Cost = -4;

        int buildModeAbilityExecutedResource1Cost = -5;
        int buildModeAbilityExecutedResource2Cost = -6;

        int flatHexOccupationBonusResource1Amount = 7;
        int flatHexOccupationBonusResource2Amount = 8;

        int conditionalHexOccupationBonusResource1Amount = 9;
        int conditionalHexOccupationBonusResource2Amount = 10;

        int conditionalStructureOccupationBonusResource1Amount = 11;
        int conditionalStructureOccupationBonusResource2Amount = 12;

        int structureUpgradedResource1Cost = -13;
        int structureUpgradedResource2Cost = -14;

        //Instance Dependency Provider Deps
        IReadOnlyMapData mapData = Substitute.For<IReadOnlyMapData>();

        IReadOnlyCreepManager creepManager = Substitute.For<IReadOnlyCreepManager>();

        IModelInterfaceController interfaceController = Substitute.For<IModelInterfaceController>();
        
        //Other Deps Passed To Ctor or SetUp
        IResourceTemplate resource1Template = Substitute.For<IResourceTemplate>();
        IResourceTemplate resource2Template = Substitute.For<IResourceTemplate>();

        IResource resource1;
        IResource resource2;

        IEconomyTransaction startingResources = Substitute.For<IEconomyTransaction>();

        //subject
        IEconomyManager subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //Model and Frame Updater
            DepsProv.TheInterfaceController = () =>
            {
                return interfaceController;
            };
            
            //Other Deps Passed To Ctor or SetUp
            resource1Template.MaxAmount.Returns(100000);
            resource2Template.MaxAmount.Returns(100000);

            resource1 = Substitute.For<IResource>();
            resource2 = Substitute.For<IResource>();

            resource1.CanDoTransaction(Arg.Any<int>()).Returns(true);
            resource2.CanDoTransaction(Arg.Any<int>()).Returns(true);

            resource1Template.GenerateResource().Returns(resource1);
            resource2Template.GenerateResource().Returns(resource2);

            var startingResource1Transaction = new CResourceTransaction(resource1, resource1StartingAmount);
            var startingResource2Transaction = new CResourceTransaction(resource2, resource2StartingAmount);

            startingResources.TransactionDictionary.Returns(new Dictionary<IReadOnlyResource, IResourceTransaction>
            {
                { resource1, startingResource1Transaction },
                { resource2, startingResource2Transaction }
            });

            subject = ConstructAndSetUpSubject();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(DepsProv).TypeInitializer.Invoke(null, null);
        }

        private CEconomyManager ConstructAndSetUpSubject()
        {
            var subject = new CEconomyManager(
                mapData,
                creepManager
            );

            subject.SetUp(
                new List<IResourceTemplate> { resource1Template, resource2Template },
                startingResources
            );

            return subject; 
        }

        [Test]
        public void SetUp_Always_AddsResourcesToPublicList()
        {
            Assert.True(subject.Resources.Contains(resource1));
            Assert.True(subject.Resources.Contains(resource2));
        }

        [Test]
        public void SetUp_Always_AddsStartingResource()
        {
            resource1.Received(1).DoTransaction(resource1StartingAmount);
            resource2.Received(1).DoTransaction(resource2StartingAmount);
        }

        [Test]
        public void GetResourceFromTemplate_Always_ReturnsTheResourceFromTheTemplatePassed()
        {
            var result = subject.GetResourceFromTemplate(resource1Template);

            Assert.AreEqual(resource1, result);
        }

        [Test]
        public void OnStructureBuilt_Always_SubtractsTheStructuresCostAndFiresEvent()
        {
            var structureCost = Substitute.For<IEconomyTransaction>();

            structureCost.TransactionDictionary.Returns(new Dictionary<IReadOnlyResource, IResourceTransaction>
            {
                { resource1, new CResourceTransaction(resource1, buildStructureResource1Cost) },
                { resource2, new CResourceTransaction(resource2, buildStructureResource2Cost) }
            });

            var structureTemplate = Substitute.For<IStructureTemplate>();
            
            structureTemplate.Cost.Returns(structureCost);

            interfaceController.OnBuildStructurePlayerAction += Raise.EventWith(new EAOnBuildStructurePlayerAction(
                new Coord(3, 3),
                structureTemplate
            ));

            resource1.Received(1).DoTransaction(buildStructureResource1Cost);
            resource2.Received(1).DoTransaction(buildStructureResource2Cost);
        }

        [Test]
        public void OnCreepDied_Always_AddsTheCreepsBounty()
        {
            var creepBounty = Substitute.For<IEconomyTransaction>();

            creepBounty.TransactionDictionary.Returns(new Dictionary<IReadOnlyResource, IResourceTransaction>
            {
                { resource1, new CResourceTransaction(resource1, creepDiedResource1Bounty) },
                { resource2, new CResourceTransaction(resource2, creepDiedResource2Bounty) },
            });
            
            var creep = Substitute.For<ICreep>();

            creep.CreepTemplate.Bounty.Returns(creepBounty);

            creepManager.OnCreepSpawned += Raise.EventWith(new EAOnCreepSpawned(creep));

            creep.OnDied += Raise.Event();

            resource1.Received(1).DoTransaction(creepDiedResource1Bounty);
            resource2.Received(1).DoTransaction(creepDiedResource2Bounty);
        }

        [Test]
        public void OnBuildModeAbilityExecuted_Always_SubtractsTheAbilitysCost()
        {
            var unit = Substitute.For<IUnit>();

            mapData.OnUnitCreated += Raise.EventWith(new EAOnUnitCreated(
                new Coord(2, 2),
                unit
            ));

            var abilityCost = Substitute.For<IEconomyTransaction>();

            abilityCost.TransactionDictionary.Returns(new Dictionary<IReadOnlyResource, IResourceTransaction>
            {
                { resource1, new CResourceTransaction(resource1, buildModeAbilityExecutedResource1Cost) },
                { resource2, new CResourceTransaction(resource2, buildModeAbilityExecutedResource2Cost) },
            });

            var ability = Substitute.For<IBuildModeAbility>();
            
            ability.BuildModeAbilityTemplate.Cost.Returns(abilityCost);

            unit.Abilities.OnBuildModeAbilityAdded += Raise.EventWith(new EAOnBuildModeAbilityAdded(ability));

            ability.OnExecuted += Raise.EventWith(new EAOnExecutedBuildModeAbility(ability));

            resource1.Received(1).DoTransaction(buildModeAbilityExecutedResource1Cost);
            resource2.Received(1).DoTransaction(buildModeAbilityExecutedResource2Cost);
        }

        [Test]
        public void OnFlatHexOccupatioBonusTriggered_Always_AddsTheTransaction()
        {
            var unit = Substitute.For<IUnit>();

            mapData.OnUnitCreated += Raise.EventWith(new EAOnUnitCreated(
                new Coord(2, 2),
                unit
            ));

            var bonus = Substitute.For<IEconomyTransaction>();

            bonus.TransactionDictionary.Returns(new Dictionary<IReadOnlyResource, IResourceTransaction>
            {
                { resource1, new CResourceTransaction(resource1, flatHexOccupationBonusResource1Amount) },
                { resource2, new CResourceTransaction(resource2, flatHexOccupationBonusResource2Amount) },
            });

            unit.OnTriggeredFlatHexOccupationBonus += Raise.EventWith(
                new EAOnTriggeredFlatHexOccupationBonus(unit, bonus));

            resource1.Received(1).DoTransaction(flatHexOccupationBonusResource1Amount);
            resource2.Received(1).DoTransaction(flatHexOccupationBonusResource2Amount);
        }

        [Test]
        public void OnConditionalOccupationBonusTriggered_Always_AddsTheTransactions()
        {
            var unit = Substitute.For<IUnit>();

            mapData.OnUnitCreated += Raise.EventWith(new EAOnUnitCreated(
                new Coord(2, 3),
                unit
            ));

            var hexTransaction = Substitute.For<IEconomyTransaction>();

            hexTransaction.TransactionDictionary.Returns(new Dictionary<IReadOnlyResource, IResourceTransaction>
            {
                { resource1, new CResourceTransaction(resource1, conditionalHexOccupationBonusResource1Amount) },
                { resource2, new CResourceTransaction(resource2, conditionalHexOccupationBonusResource2Amount) },
            });

            var structureTransaction = Substitute.For<IEconomyTransaction>();

            structureTransaction.TransactionDictionary.Returns(new Dictionary<IReadOnlyResource, IResourceTransaction>
            {
                { resource1, new CResourceTransaction(resource1, conditionalStructureOccupationBonusResource1Amount) },
                { resource2, new CResourceTransaction(resource2, conditionalStructureOccupationBonusResource2Amount) },
            });

            unit.OnTriggeredConditionalOccupationBonuses += Raise.EventWith(new EAOnTriggeredConditionalOccupationBonus(
                unit,
                hexTransaction,
                structureTransaction
            ));

            resource1.Received(1).DoTransaction(conditionalHexOccupationBonusResource1Amount);
            resource1.Received(1).DoTransaction(conditionalStructureOccupationBonusResource1Amount);

            resource2.Received(1).DoTransaction(conditionalHexOccupationBonusResource2Amount);
            resource2.Received(1).DoTransaction(conditionalStructureOccupationBonusResource2Amount);
        }

        [Test]
        public void OnStructureUpgraded_Always_SubtractsTheUpgradeCost()
        {
            var structure = Substitute.For<IStructure>();

            mapData.OnStructureCreated += Raise.EventWith(new EAOnStructureCreated(
                new Coord(3, 4),
                structure
            ));

            var enhancementCost = Substitute.For<IEconomyTransaction>();

            enhancementCost.TransactionDictionary.Returns(new Dictionary<IReadOnlyResource, IResourceTransaction>
            {
                { resource1, new CResourceTransaction(resource1, structureUpgradedResource1Cost) },
                { resource2, new CResourceTransaction(resource2, structureUpgradedResource2Cost) },
            });

            var enhancement = Substitute.For<IStructureEnhancement>();

            enhancement.Cost.Returns(enhancementCost);

            structure.OnUpgraded += Raise.EventWith(new EAOnUpgraded(
                Substitute.For<IStructureUpgrade>(),
                enhancement
            ));

            resource1.Received(1).DoTransaction(structureUpgradedResource1Cost);
            resource2.Received(1).DoTransaction(structureUpgradedResource2Cost);
        }

        [Test]
        public void OnResourceChangedEvent_Always_FiresOnAnyResourceChangedEvent()
        {
            var eventTester = new EventTester<EAOnAnyResourceChanged>();
            subject.OnAnyResourceChanged += eventTester.Handler;

            var byAmount = 5;
            var toAmount = 10;

            resource2.OnResourceChanged += Raise.EventWith(new EAOnResourceChanged(byAmount, toAmount));

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => {
                return args.Resource == resource2 &&
                    args.ByAmount == byAmount &&
                    args.ToAmount == toAmount;
            });
        }
    }
}