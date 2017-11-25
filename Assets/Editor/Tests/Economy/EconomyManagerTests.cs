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

        //Model and Frame Updater


        //Instance Dependency Provider Deps
        IReadOnlyMapData mapData = Substitute.For<IReadOnlyMapData>();

        IReadOnlyCreepManager creepManager = Substitute.For<IReadOnlyCreepManager>();

        IModelInterfaceController interfaceController = Substitute.For<IModelInterfaceController>();

        //Template Deps


        //Other Deps Passed To Ctor or SetUp
        IResourceTemplate resource1Template = Substitute.For<IResourceTemplate>();
        IResourceTemplate resource2Template = Substitute.For<IResourceTemplate>();

        IResource resource1;
        IResource resource2;

        IEconomyTransaction startingResources = Substitute.For<IEconomyTransaction>();

        //Other Objects Passed To Methods


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //Model and Frame Updater
            DepsProv.TheInterfaceController = () =>
            {
                return interfaceController;
            };

            //Instance Dependency Provider Deps


            //Template Deps


            //Other Deps Passed To Ctor or SetUp
            resource1Template.MaxAmount.Returns(100000);
            resource2Template.MaxAmount.Returns(100000);

            startingResources.CanDoTransaction().Returns(true);

            //Other Objects Passed To Methods


        }

        [SetUp]
        public void EachTestSetUp()
        {
            resource1 = Substitute.For<IResource>();
            resource2 = Substitute.For<IResource>();

            resource1Template.GenerateResource().Returns(resource1);
            resource2Template.GenerateResource().Returns(resource2);

            startingResources.GetTransactionAmount(resource1).Returns(resource1StartingAmount);
            var startingResource1Transaction = new CResourceTransaction(resource1, resource1StartingAmount);
            startingResources.GetResourceTransaction(resource1).Returns(startingResource1Transaction);

            startingResources.GetTransactionAmount(resource2).Returns(resource2StartingAmount);
            var startingResource2Transaction = new CResourceTransaction(resource2, resource2StartingAmount);
            startingResources.GetResourceTransaction(resource2).Returns(startingResource2Transaction);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(DepsProv).TypeInitializer.Invoke(null, null);
        }

        private CEconomyManager ConstructAndSetUpSubject()
        {
            var subject = ConstructSubject();

            SetUpSubject(subject);

            return subject; 
        }

        private CEconomyManager ConstructSubject()
        {
            return new CEconomyManager(
                mapData,
                creepManager
            );
        }

        private void SetUpSubject(CEconomyManager subject)
        {
            subject.SetUp(
                new List<IResourceTemplate> { resource1Template, resource2Template },
                startingResources
            );
        }

        [Test]
        public void SetUp_Always_AddsResourcesToPublicList()
        {
            var subject = ConstructAndSetUpSubject();

            Assert.True(subject.Resources.Contains(resource1));
            Assert.True(subject.Resources.Contains(resource2));
        }

        [Test]
        public void SetUp_Always_AddsStartingResource()
        {
            ConstructAndSetUpSubject();

            resource1.Received(1).DoTransaction(resource1StartingAmount);
            resource2.Received(1).DoTransaction(resource2StartingAmount);
        }

        [Test]
        public void GetResourceFromTemplate_Always_ReturnsTheResourceFromTheTemplatePassed()
        {
            var subject = ConstructAndSetUpSubject();

            var result = subject.GetResourceFromTemplate(resource1Template);

            Assert.AreEqual(resource1, result);
        }

        [Test]
        public void OnStructureBuilt_Always_SubtractsTheStructuresCostAndFiresEvent()
        {
            ConstructAndSetUpSubject();

            var buildStructureResource1Cost = -20;
            var buildStructureResource2Cost = -15;

            var structureCost = Substitute.For<IEconomyTransaction>();

            structureCost.GetTransactionAmount(resource1).Returns(buildStructureResource1Cost);
            structureCost.GetTransactionAmount(resource2).Returns(buildStructureResource2Cost);
            structureCost.CanDoTransaction().Returns(true);

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
            ConstructAndSetUpSubject();

            var creepDiedResource1Bounty = 5;
            var creepDiedResource2Bounty = 7;

            var creepBounty = Substitute.For<IEconomyTransaction>();

            creepBounty.GetTransactionAmount(resource1).Returns(creepDiedResource1Bounty);
            creepBounty.GetTransactionAmount(resource2).Returns(creepDiedResource2Bounty);
            creepBounty.CanDoTransaction().Returns(true);
            
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
            ConstructAndSetUpSubject();
            
            var buildModeAbilityExecutedResource1Cost = -3;
            var buildModeAbilityExecutedResource2Cost = -8;

            var unit = Substitute.For<IUnit>();

            mapData.OnUnitCreated += Raise.EventWith(new EAOnUnitCreated(
                new Coord(2, 2),
                unit
            ));

            var abilityCost = Substitute.For<IEconomyTransaction>();

            abilityCost.GetTransactionAmount(resource1).Returns(buildModeAbilityExecutedResource1Cost);
            abilityCost.GetTransactionAmount(resource2).Returns(buildModeAbilityExecutedResource2Cost);
            abilityCost.CanDoTransaction().Returns(true);

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
            ConstructAndSetUpSubject();

            var flatHexOccupationBonusResource1Amount = 4;
            var flatHexOccupationBonusResource2Amount = 12;

            var unit = Substitute.For<IUnit>();

            mapData.OnUnitCreated += Raise.EventWith(new EAOnUnitCreated(
                new Coord(2, 2),
                unit
            ));

            var bonus = Substitute.For<IEconomyTransaction>();

            bonus.GetTransactionAmount(resource1).Returns(flatHexOccupationBonusResource1Amount);
            bonus.GetTransactionAmount(resource2).Returns(flatHexOccupationBonusResource2Amount);
            bonus.CanDoTransaction().Returns(true);

            unit.OnTriggeredFlatHexOccupationBonus += Raise.EventWith(
                new EAOnTriggeredFlatHexOccupationBonus(unit, bonus));

            resource1.Received(1).DoTransaction(flatHexOccupationBonusResource1Amount);
            resource2.Received(1).DoTransaction(flatHexOccupationBonusResource2Amount);
        }

        [Test]
        public void OnConditionalOccupationBonusTriggered_Always_AddsTheTransactions()
        {
            ConstructAndSetUpSubject();

            var conditionalHexOccupationBonusResource1Amount = 1;
            var conditionalHexOccupationBonusResource2Amount = 11;

            var conditionalStructureOccupationBonusResource1Amount = 2;
            var conditionalStructureOccupationBonusResource2Amount = 13;

            var unit = Substitute.For<IUnit>();

            mapData.OnUnitCreated += Raise.EventWith(new EAOnUnitCreated(
                new Coord(2, 3),
                unit
            ));

            var hexTransaction = Substitute.For<IEconomyTransaction>();

            hexTransaction.GetTransactionAmount(resource1).Returns(conditionalHexOccupationBonusResource1Amount);
            hexTransaction.GetTransactionAmount(resource2).Returns(conditionalHexOccupationBonusResource2Amount);
            hexTransaction.CanDoTransaction().Returns(true);

            var structureTransaction = Substitute.For<IEconomyTransaction>();

            structureTransaction.GetTransactionAmount(resource1).Returns(conditionalStructureOccupationBonusResource1Amount);
            structureTransaction.GetTransactionAmount(resource2).Returns(conditionalStructureOccupationBonusResource2Amount);
            structureTransaction.CanDoTransaction().Returns(true);

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
            ConstructAndSetUpSubject();
            
            var structureUpgradedResource1Cost = -18;
            var structureUpgradedResource2Cost = -22;

            var structure = Substitute.For<IStructure>();

            mapData.OnStructureCreated += Raise.EventWith(new EAOnStructureCreated(
                new Coord(3, 4),
                structure
            ));

            var enhancementCost = Substitute.For<IEconomyTransaction>();

            enhancementCost.GetTransactionAmount(resource1).Returns(structureUpgradedResource1Cost);
            enhancementCost.GetTransactionAmount(resource2).Returns(structureUpgradedResource2Cost);
            enhancementCost.CanDoTransaction().Returns(true);
            
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
            var subject = ConstructAndSetUpSubject();

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