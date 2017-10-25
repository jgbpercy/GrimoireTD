using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Creeps;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Tests.DefendModeTargetingRuleServiceTests
{
    public class DefendModeTargetingRuleServiceTests
    {
        //Model and Frame Updater
        private IReadOnlyGameModel gameModel = Substitute.For<IReadOnlyGameModel>();

        private IReadOnlyCreepManager creepManager = Substitute.For<ICreepManager>();

        //Other Objects Passed To Methods
        private IDefendingEntity attachedToDefendingEntity = Substitute.For<IDefendingEntity>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            gameModel.CreepManager.Returns(creepManager);

            DepsProv.SetTheGameModel(gameModel);

            attachedToDefendingEntity.CoordPosition.Returns(new Coord(0, 0));
        }

        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
            typeof(DepsProv).TypeInitializer.Invoke(null, null);
        }

        [Test]
        public void RunRule_PassedCreepClosestToFinishInRangeAndNoCreeps_ReturnsNull()
        {
            var creepClosestToFinishInRangeArgs = new CreepClosestToFinishInRangeArgs(
                attachedToDefendingEntity,
                3f
            );

            var result = DefendModeTargetingRuleService.RunRule(creepClosestToFinishInRangeArgs);

            Assert.AreEqual(result, null);
        }

        [Test]
        public void RunRule_PassedCreepClosestToFinishInRangeAndNoCreepsInRange_ReturnsNull()
        {
            var creepOne = Substitute.For<ICreep>();
            creepOne.Position.Returns(new Coord(10, 10).ToPositionVector());

            var creepTwo = Substitute.For<ICreep>();
            creepTwo.Position.Returns(new Coord(12, 8).ToPositionVector());

            var creepThree = Substitute.For<ICreep>();
            creepThree.Position.Returns(new Coord(7, 14).ToPositionVector());

            var creepList = new List<ICreep>
            {
                creepOne,
                creepTwo,
                creepThree
            };

            creepManager.CreepList.Returns(creepList);

            var creepClosestToFinishInRangeArgs = new CreepClosestToFinishInRangeArgs(
                attachedToDefendingEntity,
                3f
            );

            var result = DefendModeTargetingRuleService.RunRule(creepClosestToFinishInRangeArgs);

            Assert.AreEqual(result, null);
        }

        [Test]
        public void RunRule_PassedCreepClosestToFinishInRangeAndFirstCreepIsInRange_ReturnsListWithOnlyThatCreep()
        {

            var creepOne = Substitute.For<ICreep>();
            creepOne.Position.Returns(new Coord(5, 5).ToPositionVector());

            var creepTwo = Substitute.For<ICreep>();
            creepTwo.Position.Returns(new Coord(3, 3).ToPositionVector());

            var creepThree = Substitute.For<ICreep>();
            creepThree.Position.Returns(new Coord(4, 4).ToPositionVector());

            var creepClosestToFinishInRangeArgs = new CreepClosestToFinishInRangeArgs(
                attachedToDefendingEntity,
                20f
            );

            var creepList = new List<ICreep>
            {
                creepOne,
                creepTwo,
                creepThree
            };

            creepManager.CreepList.Returns(creepList);

            var result = DefendModeTargetingRuleService.RunRule(creepClosestToFinishInRangeArgs);

            Assert.True(result.Count == 1);
            Assert.AreEqual(result[0], creepOne);
        }

        [Test]
        public void RunRule_PassedCreepClosestToFinishInRangeAndAMidListCreepIsInRange_ReturnsListWithOnlyThatCreep()
        {
            var creepOne = Substitute.For<ICreep>();
            creepOne.Position.Returns(new Coord(40, 40).ToPositionVector());

            var creepTwo = Substitute.For<ICreep>();
            creepTwo.Position.Returns(new Coord(3, 3).ToPositionVector());

            var creepThree = Substitute.For<ICreep>();
            creepThree.Position.Returns(new Coord(4, 4).ToPositionVector());

            var creepList = new List<ICreep>
            {
                creepOne,
                creepTwo,
                creepThree
            };

            creepManager.CreepList.Returns(creepList);

            var creepClosestToFinishInRangeArgs = new CreepClosestToFinishInRangeArgs(
                attachedToDefendingEntity,
                10f
            );

            var result = DefendModeTargetingRuleService.RunRule(creepClosestToFinishInRangeArgs);

            Assert.True(result.Count == 1);
            Assert.AreEqual(result[0], creepTwo);
        }

        [Test]
        public void RunRule_PassedCreepClosestToFinishInRangeAndOnlyTheLastCreepIsInRange_ReturnsListWithOnlyThatCreep()
        {
            var creepOne = Substitute.For<ICreep>();
            creepOne.Position.Returns(new Coord(40, 40).ToPositionVector());

            var creepTwo = Substitute.For<ICreep>();
            creepTwo.Position.Returns(new Coord(50, 50).ToPositionVector());

            var creepThree = Substitute.For<ICreep>();
            creepThree.Position.Returns(new Coord(4, 4).ToPositionVector());

            var creepList = new List<ICreep>
            {
                creepOne,
                creepTwo,
                creepThree
            };

            creepManager.CreepList.Returns(creepList);

            var creepClosestToFinishInRangeArgs = new CreepClosestToFinishInRangeArgs(
                attachedToDefendingEntity,
                10f
            );

            var result = DefendModeTargetingRuleService.RunRule(creepClosestToFinishInRangeArgs);

            Assert.True(result.Count == 1);
            Assert.AreEqual(result[0], creepThree);
        }
    }
}