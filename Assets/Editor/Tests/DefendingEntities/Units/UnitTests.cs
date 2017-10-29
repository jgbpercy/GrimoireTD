using NUnit.Framework;
using NSubstitute;
using GrimoireTD.DefendingEntities;
using GrimoireTD.DefendingEntities.Units;

namespace GrimoireTD.Tests.UnitTests
{
    public class UnitTests : DefendingEntityTests.DefendingEntityTests
    {
        //Primitives and Basic Objects


        //Model and Frame Updater


        //Instance Dependency Provider Deps


        //Template Deps
        private IUnitTemplate template = Substitute.For<IUnitTemplate>();


        //Other Deps Passed To Ctor or SetUp


        //Other Objects Passed To Methods


        [OneTimeSetUp]
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();

            //Model and Frame Updater


            //Instance Dependency Provider Deps


            //Template Deps


            //Other Deps Passed To Ctor or SetUp


            //Other Objects Passed To Methods


        }

        [SetUp]
        public override void EachTestSetUp()
        {
            base.EachTestSetUp();
        }

        protected override CDefendingEntity ConstructSubject()
        {
            return new CUnit(template, startPosition);
        }

        private CUnit ConstructUnitSubject()
        {
            return ConstructSubject() as CUnit;
        }
    }
}