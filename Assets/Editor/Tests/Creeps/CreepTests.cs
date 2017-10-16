using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using GrimoireTD.Creeps;

namespace GrimoireTD.Tests.CreepTests
{
    public class CreepTests
    {
        private Vector3 startingPosition = new Vector3(1, 2, 3);

        private ICreepTemplate creepTemplate = Substitute.For<ICreepTemplate>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {

        }

        [SetUp]
        public void EachTestSetUp()
        {

        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {

        }

        private CCreep ConstructSubject()
        {
            return new CCreep(creepTemplate, new Vector3());
        }

        [Test]
        public void Ctor_Always_CreatesCreepAtStartingPosition()
        {

        }

        [Test]
        public void Ctor_Always_CreatesCreepWithMaxHitPoints()
        {

        }

        [Test]
        public void Ctor_Always_AddsBaseAttributes()
        {

        }
    }
}