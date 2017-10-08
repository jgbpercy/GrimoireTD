using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Creeps;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Tests.DefendModeTargetingRuleServiceTests
{
    public class DefendModeTargetingRuleServiceTests
    {
        private IDefendingEntity attachedToDefendingEntity = Substitute.For<IDefendingEntity>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            attachedToDefendingEntity.CoordPosition.Returns(new Coord(0, 0));
        }

        [Test]
        public void RunRule_PassedCreepClosestToFinishInRangeAndNoCreeps_ReturnsNull()
        {
            var creepList = new List<ICreep>();

            var creepClosestToFinishInRangeArgs = new CreepClosestToFinishInRangeArgs(
                attachedToDefendingEntity,
                3f,
                creepList
            );

            var result = DefendModeTargetingRuleService.RunRule(creepClosestToFinishInRangeArgs);

            Assert.AreEqual(result, null);
        }

        [Test]
        public void RunRule_PassedCreepClosestToFinishInRangeAndNoCreepsInRange_ReturnsNull()
        {
            var creepList = new List<ICreep>();

            var creepOne = Substitute.For<ICreep>();
            creepOne.Position.Returns(new Coord(10, 10).ToPositionVector());
            creepList.Add(creepOne);

            var creepTwo = Substitute.For<ICreep>();
            creepTwo.Position.Returns(new Coord(12, 8).ToPositionVector());
            creepList.Add(creepTwo);

            var creepThree = Substitute.For<ICreep>();
            creepThree.Position.Returns(new Coord(7, 14).ToPositionVector());
            creepList.Add(creepThree);

            var creepClosestToFinishInRangeArgs = new CreepClosestToFinishInRangeArgs(
                attachedToDefendingEntity,
                3f,
                creepList
            );

            var result = DefendModeTargetingRuleService.RunRule(creepClosestToFinishInRangeArgs);

            Assert.AreEqual(result, null);
        }

        [Test]
        public void RunRule_PassedCreepClosestToFinishInRangeAndFirstCreepIsInRange_ReturnsListWithOnlyThatCreep()
        {
            var creepList = new List<ICreep>();

            var creepOne = Substitute.For<ICreep>();
            creepOne.Position.Returns(new Coord(5, 5).ToPositionVector());
            creepList.Add(creepOne);

            var creepTwo = Substitute.For<ICreep>();
            creepTwo.Position.Returns(new Coord(3, 3).ToPositionVector());
            creepList.Add(creepTwo);

            var creepThree = Substitute.For<ICreep>();
            creepThree.Position.Returns(new Coord(4, 4).ToPositionVector());
            creepList.Add(creepThree);

            var creepClosestToFinishInRangeArgs = new CreepClosestToFinishInRangeArgs(
                attachedToDefendingEntity,
                20f,
                creepList
            );

            var result = DefendModeTargetingRuleService.RunRule(creepClosestToFinishInRangeArgs);

            Assert.True(result.Count == 1);
            Assert.AreEqual(result[0], creepOne);
        }

        [Test]
        public void RunRule_PassedCreepClosestToFinishInRangeAndAMidListCreepIsInRange_ReturnsListWithOnlyThatCreep()
        {
            var creepList = new List<ICreep>();

            var creepOne = Substitute.For<ICreep>();
            creepOne.Position.Returns(new Coord(40, 40).ToPositionVector());
            creepList.Add(creepOne);

            var creepTwo = Substitute.For<ICreep>();
            creepTwo.Position.Returns(new Coord(3, 3).ToPositionVector());
            creepList.Add(creepTwo);

            var creepThree = Substitute.For<ICreep>();
            creepThree.Position.Returns(new Coord(4, 4).ToPositionVector());
            creepList.Add(creepThree);

            var creepClosestToFinishInRangeArgs = new CreepClosestToFinishInRangeArgs(
                attachedToDefendingEntity,
                10f,
                creepList
            );

            var result = DefendModeTargetingRuleService.RunRule(creepClosestToFinishInRangeArgs);

            Assert.True(result.Count == 1);
            Assert.AreEqual(result[0], creepTwo);
        }

        [Test]
        public void RunRule_PassedCreepClosestToFinishInRangeAndOnlyTheLastCreepIsInRange_ReturnsListWithOnlyThatCreep()
        {
            var creepList = new List<ICreep>();

            var creepOne = Substitute.For<ICreep>();
            creepOne.Position.Returns(new Coord(40, 40).ToPositionVector());
            creepList.Add(creepOne);

            var creepTwo = Substitute.For<ICreep>();
            creepTwo.Position.Returns(new Coord(50, 50).ToPositionVector());
            creepList.Add(creepTwo);

            var creepThree = Substitute.For<ICreep>();
            creepThree.Position.Returns(new Coord(4, 4).ToPositionVector());
            creepList.Add(creepThree);

            var creepClosestToFinishInRangeArgs = new CreepClosestToFinishInRangeArgs(
                attachedToDefendingEntity,
                10f,
                creepList
            );

            var result = DefendModeTargetingRuleService.RunRule(creepClosestToFinishInRangeArgs);

            Assert.True(result.Count == 1);
            Assert.AreEqual(result[0], creepThree);
        }
    }
}