﻿using System.Collections.Generic;
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
        private IMoveEffectComponentTemplate moveEffectComponentTemplate;

        private IUnit unit;

        private Coord targetCoord;

        private List<IBuildModeTargetable> targetList;

        private IStructure structure;

        private CMoveEffectComponent subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            moveEffectComponentTemplate = Substitute.For<IMoveEffectComponentTemplate>();

            unit = Substitute.For<IUnit>();

            targetCoord = new Coord(1, 1);

            targetList = new List<IBuildModeTargetable>
            {
                targetCoord
            };

            structure = Substitute.For<IStructure>();

            subject = new CMoveEffectComponent(moveEffectComponentTemplate);
        }

        [Test]
        public void ExecuteEffect_PassedAUnitAndASingleTargetHex_MovesUnit()
        {
            subject.ExecuteEffect(unit, targetList);

            unit.Received().Move(Arg.Is(targetCoord));
        }

        [Test]
        public void ExecuteEffect_PassedNonUnit_ThrowsException()
        {
            Assert.Throws(typeof(ArgumentException), () => subject.ExecuteEffect(structure, targetList));
        }

        [Test]
        public void ExecuteEffect_PassedMultipleTargets_ThrowsException()
        {
            var multiTargetList = targetList;

            multiTargetList.Add(new Coord(2, 2));

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