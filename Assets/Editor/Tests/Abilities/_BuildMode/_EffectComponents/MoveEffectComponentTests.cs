using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.Map;
using GrimoireTD.DefendingEntities.Structures;
using System;

namespace GrimoireTD.Tests.MoveEffectComponentTests
{
    public class MoveEffectComponentTests
    {
        private Coord targetCoord = new Coord(1, 1);

        private IMoveEffectComponentTemplate moveEffectComponentTemplate = Substitute.For<IMoveEffectComponentTemplate>();

        private IUnit unit = Substitute.For<IUnit>();

        private List<IBuildModeTargetable> targetList;

        private IStructure structure = Substitute.For<IStructure>();

        private CMoveEffectComponent subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            targetList = new List<IBuildModeTargetable>
            {
                targetCoord
            };

            subject = new CMoveEffectComponent(moveEffectComponentTemplate);
        }

        [SetUp]
        public void EachTestSetUp()
        {
            unit.ClearReceivedCalls();
        }

        [Test]
        public void ExecuteEffect_PassedAUnitAndASingleTargetHex_MovesUnit()
        {
            subject.ExecuteEffect(unit, targetList);

            unit.Received(1).Move(Arg.Is(targetCoord));
        }

        [Test]
        public void ExecuteEffect_PassedNonUnit_ThrowsException()
        {
            Assert.Throws(typeof(ArgumentException), () => subject.ExecuteEffect(structure, targetList));
        }

        [Test]
        public void ExecuteEffect_PassedMultipleTargets_ThrowsException()
        {
            var multiTargetList = new List<IBuildModeTargetable>
            {
                targetCoord,
                new Coord(2, 2)
            };

            Assert.Throws(typeof(ArgumentException), () => subject.ExecuteEffect(unit, multiTargetList));
        }

        [Test]
        public void ExecuteEffect_PassedNonHexTarget_ThrowsException()
        {
            var structureTargetList = new List<IBuildModeTargetable>
            {
                structure
            };

            Assert.Throws(typeof(ArgumentException), () => subject.ExecuteEffect(unit, structureTargetList));
        }
    }
}