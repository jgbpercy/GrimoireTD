using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Creeps;

namespace GrimoireTD.Tests.WaveTests
{
    public class WaveTests
    {
        //Primitives and Basic Objects
        private float timing1 = 0.5f;
        private float timing2 = 0.85f;
        private float timing3 = 0.25f;

        //Template Deps
        private ICreepTemplate creepTemplate1 = Substitute.For<ICreepTemplate>();
        private ICreepTemplate creepTemplate2 = Substitute.For<ICreepTemplate>();
        private ICreepTemplate creepTemplate3 = Substitute.For<ICreepTemplate>();

        private ISpawn spawn1 = Substitute.For<ISpawn>();
        private ISpawn spawn2 = Substitute.For<ISpawn>();
        private ISpawn spawn3 = Substitute.For<ISpawn>();

        private IWaveTemplate template = Substitute.For<IWaveTemplate>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //Template Deps
            spawn1.Creep.Returns(creepTemplate1);
            spawn2.Creep.Returns(creepTemplate2);
            spawn3.Creep.Returns(creepTemplate3);

            spawn1.Timing.Returns(timing1);
            spawn2.Timing.Returns(timing2);
            spawn3.Timing.Returns(timing3);

            template.Spawns.Returns(new List<ISpawn>
            {
                spawn1,
                spawn2,
                spawn3
            });
        }

        private CWave ConstructSubject()
        {
            return new CWave(template);
        }

        [Test]
        public void Spawns_AfterConstruction_ContainsExpectedTimingsAndCreeps()
        {
            var subject = ConstructSubject();

            Assert.AreEqual(3, subject.Spawns.Count);

            AssertExt.Approximately(timing1, subject.Spawns.ElementAt(0).Key);
            Assert.AreEqual(creepTemplate1, subject.Spawns.ElementAt(0).Value);

            AssertExt.Approximately(timing1 + timing2, subject.Spawns.ElementAt(1).Key);
            Assert.AreEqual(creepTemplate2, subject.Spawns.ElementAt(1).Value);

            AssertExt.Approximately(timing1 + timing2 + timing3, subject.Spawns.ElementAt(2).Key);
            Assert.AreEqual(creepTemplate3, subject.Spawns.ElementAt(2).Value);
        }

        [Test]
        public void NextSpawnTime_AfterConstruction_ReturnsSpawnTimeOfFirstCreep()
        {
            var subject = ConstructSubject();

            var result = subject.NextSpawnTime();

            AssertExt.Approximately(timing1, result);
        }

        [Test]
        public void NextSpawnTime_AfterOneCreepSpawned_ReturnsSpawnTimeOfNextCreep()
        {
            var subject = ConstructSubject();

            subject.DequeueNextCreep();

            var result = subject.NextSpawnTime();

            AssertExt.Approximately(timing1 + timing2, result);
        }
    
        [Test]
        public void CreepsRemaining_AfterOneCreepButNotAllCreepsSpawned_ReturnsTrue()
        {
            var subject = ConstructSubject();

            subject.DequeueNextCreep();

            var result = subject.CreepsRemaining();

            Assert.True(result);
        }

        [Test]
        public void CreepsRemaining_AfterAllCreepsSpawned_ReturnsFalse()
        {
            var subject = ConstructSubject();

            subject.DequeueNextCreep();
            subject.DequeueNextCreep();
            subject.DequeueNextCreep();

            var result = subject.CreepsRemaining();

            Assert.False(result);
        }

        [Test]
        public void DequeueNextCreep_AfterConstruction_ReturnsFirstCreep()
        {
            var subject = ConstructSubject();

            var result = subject.DequeueNextCreep();

            Assert.AreEqual(creepTemplate1, result);
        }

        [Test]
        public void DequeuNextCreep_AfterOneCreepSpawned_ReturnsSecondCreep()
        {
            var subject = ConstructSubject();

            subject.DequeueNextCreep();

            var result = subject.DequeueNextCreep();

            Assert.AreEqual(creepTemplate2, result);
        }

        [Test]
        public void DequeueNextCreep_AfterOneCreepSpawned_RemovesOneCreepFromSpawns()
        {
            var subject = ConstructSubject();

            subject.DequeueNextCreep();

            Assert.False(subject.Spawns.Values.Contains(creepTemplate1));
            Assert.True(subject.Spawns.Values.Contains(creepTemplate2));
        }

        [Test]
        public void DequeueNextCreep_AfterAllCreepsSpawned_ThrowsException()
        {
            var subject = ConstructSubject();

            subject.DequeueNextCreep();
            subject.DequeueNextCreep();
            subject.DequeueNextCreep();

            Assert.Throws(typeof(ArgumentOutOfRangeException), () => subject.DequeueNextCreep());
        }
    }
}