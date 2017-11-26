using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Economy;
using GrimoireTD.Dependencies;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Technical;

namespace GrimoireTD.Tests.EconomyTransactionTests
{
    public class EconomyTransactionTests
    {
        //Primitives and Basic Objects
        string resource1Name = "Resource One";
        string resource2Name = "Resource Two";

        string resource1ShortName = "R1";
        string resource2ShortName = "R2";

        //Model and Frame Updater
        private IResource resource1;
        private IResource resource2;

        private IReadOnlyEconomyManager economyManager = Substitute.For<IReadOnlyEconomyManager>();

        private IReadOnlyGameModel gameModel = Substitute.For<IReadOnlyGameModel>();

        //Instance Dependency Provider Deps


        //Template Deps


        //Other Deps Passed To Ctor or SetUp


        //Other Objects Passed To Methods


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //Model and Frame Updater
            gameModel.EconomyManager.Returns(economyManager);

            DepsProv.SetTheGameModel(gameModel);

            //Instance Dependency Provider Deps


            //Template Deps


            //Other Deps Passed To Ctor or SetUp


            //Other Objects Passed To Methods


        }

        [SetUp]
        public void EachTestSetUp()
        {
            resource1 = Substitute.For<IResource>();
            resource2 = Substitute.For<IResource>();

            resource1.CanDoTransaction(Arg.Any<int>()).Returns(true);
            resource2.CanDoTransaction(Arg.Any<int>()).Returns(true);

            resource1.NameInGame.Returns(resource1Name);
            resource2.NameInGame.Returns(resource2Name);

            resource1.ShortName.Returns(resource1ShortName);
            resource2.ShortName.Returns(resource2ShortName);

            economyManager.Resources.Returns(new List<IReadOnlyResource> { resource1, resource2 });
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(DepsProv).TypeInitializer.Invoke(null, null);
        }

        private CEconomyTransaction ConstructSubject(int resource1Amount, int resource2Amount)
        {
            var resource1Transaction = new CResourceTransaction(resource1, resource1Amount);
            var resource2Transaction = new CResourceTransaction(resource2, resource2Amount);

            return new CEconomyTransaction(new Dictionary<IReadOnlyResource, IResourceTransaction>
            {
                { resource1, resource1Transaction },
                { resource2, resource2Transaction }
            });
        }

        [Test]
        public void EmptyCtor_Always_AddsAllResourcesWithAmountZero()
        {
            var subject = new CEconomyTransaction();

            Assert.AreEqual(0, subject.TransactionDictionary[resource1].Amount);
            Assert.AreEqual(0, subject.TransactionDictionary[resource2].Amount);
        }

        [Test]
        public void EnumerableCtor_Always_AddsAllResourcesWithCombinedValues()
        {
            var resource1Transaction1Amount = 12;
            var resource1Transaction2Amount = 10;

            var resource1Transaction1 = new CResourceTransaction(resource1, resource1Transaction1Amount);
            var resource1Transaction2 = new CResourceTransaction(resource1, resource1Transaction2Amount);

            var resource2Transaction1Amount = 23;
            var resource2Transaction2Amount = 16;

            var resource2Transaction1 = new CResourceTransaction(resource2, resource2Transaction1Amount);
            var resource2Transaction2 = new CResourceTransaction(resource2, resource2Transaction2Amount);

            var subject = new CEconomyTransaction(new List<IResourceTransaction>
            {
                resource1Transaction1,
                resource1Transaction2,
                resource2Transaction1,
                resource2Transaction2,
            });

            Assert.AreEqual(resource1Transaction1Amount + resource1Transaction2Amount, subject.TransactionDictionary[resource1].Amount);
            Assert.AreEqual(resource2Transaction1Amount + resource2Transaction2Amount, subject.TransactionDictionary[resource2].Amount);
        }

        [Test]
        public void DictionaryCtor_Always_AddsAllResourcesWithDictionaryValues()
        {
            var resource1Amount = 25;
            var resource2Amount = 243;

            var subject = ConstructSubject(resource1Amount, resource2Amount);

            Assert.AreEqual(resource1Amount, subject.TransactionDictionary[resource1].Amount);
            Assert.AreEqual(resource2Amount, subject.TransactionDictionary[resource2].Amount);
        }

        [Test]
        public void GetTransactionAmount_Always_ReturnsTheCorrectAmount()
        {
            var resource1Amount = 25;
            var resource2Amount = 90;

            var subject = ConstructSubject(resource1Amount, resource2Amount);

            Assert.AreEqual(resource1Amount, subject.GetTransactionAmount(resource1));
            Assert.AreEqual(resource2Amount, subject.GetTransactionAmount(resource2));
        }

        [Test]
        public void CanDoTransaction_IfResourceReturnsCannotDotransaction_ReturnsFalse()
        {
            var resource1Amount = 54;
            var resource2Amount = 9;

            var subject = ConstructSubject(resource1Amount, resource2Amount);

            resource1.CanDoTransaction(resource1Amount).Returns(false);

            var result = subject.CanDoTransaction();

            Assert.False(result);
        }

        [Test]
        public void CanDoTransaction_IfAllResourcesReturnThatTheyCanDoTransaction_ReturnsTrue()
        {
            var resource1Amount = 10;
            var resource2Amount = 12;

            var subject = ConstructSubject(resource1Amount, resource2Amount);

            var result = subject.CanDoTransaction();

            Assert.True(result);
        }

        [Test]
        public void ToStringOfEmpty_Always_ReturnsNonAbsoluteString()
        {
            var resource1Amount = 51;
            var resource2Amount = -45;

            var subject = ConstructSubject(resource1Amount, resource2Amount);

            var expectedString = resource1Name + ": " + resource1Amount + ", " + resource2Name + ": " + resource2Amount;

            Assert.AreEqual(expectedString, subject.ToString());
        }

        [Test]
        public void ToStringOfAbsolute_PassedTrue_ReturnsAbsoluteString()
        {
            var resource1Amount = -626;
            var resource2Amount = 256;

            var subject = ConstructSubject(resource1Amount, resource2Amount);

            var expectedString = resource1Name + ": " + Mathf.Abs(resource1Amount) + ", " + resource2Name + ": " + Mathf.Abs(resource2Amount);

            Assert.AreEqual(expectedString, subject.ToString(true));
        }

        [Test]
        public void ToStringOfAbsolute_PassedFalse_ReturnsNonAbsoluteString()
        {
            var resource1Amount = -66;
            var resource2Amount = 2356;

            var subject = ConstructSubject(resource1Amount, resource2Amount);

            var expectedString = resource1Name + ": " + resource1Amount + ", " + resource2Name + ": " + resource2Amount;

            Assert.AreEqual(expectedString, subject.ToString(false));
        }

        [Test]
        public void ToStringWithFormat_PassedFullNameSingleLine_ReturnsCorrectString()
        {
            var resource1Amount = -11;
            var resource2Amount = 155;

            var subject = ConstructSubject(resource1Amount, resource2Amount);

            var expectedString = resource1Name + ": " + resource1Amount + ", " + resource2Name + ": " + resource2Amount;

            Assert.AreEqual(expectedString, subject.ToString(EconomyTransactionStringFormat.FullNameSingleLine, false));

        }

        [Test]
        public void ToStringWithFormat_PassedFullNameLineBreak_ReturnsCorrectString()
        {
            var resource1Amount = 51;
            var resource2Amount = -145;

            var subject = ConstructSubject(resource1Amount, resource2Amount);

            var expectedString = resource1Name + ": " + resource1Amount + "\n" + resource2Name + ": " + resource2Amount;

            Assert.AreEqual(expectedString, subject.ToString(EconomyTransactionStringFormat.FullNameLineBreaks, false));
        }

        [Test]
        public void ToStringWithFormat_PassedShortNameSingleLine_ReturnsCorrectString()
        {
            var resource1Amount = -551;
            var resource2Amount = 45;

            var subject = ConstructSubject(resource1Amount, resource2Amount);

            var expectedString = resource1ShortName + ": " + resource1Amount + ", " + resource2ShortName + ": " + resource2Amount;

            Assert.AreEqual(expectedString, subject.ToString(EconomyTransactionStringFormat.ShortNameSingleLine));
        }

        [Test]
        public void ToStringWithFormat_PassedShortNameLineBreaks_ReturnsCorrectString()
        {
            var resource1Amount = -37;
            var resource2Amount = 72;

            var subject = ConstructSubject(resource1Amount, resource2Amount);

            var expectedString = resource1ShortName + ": " + resource1Amount + "\n" + resource2ShortName + ": " + resource2Amount;

            Assert.AreEqual(expectedString, subject.ToString(EconomyTransactionStringFormat.ShortNameLineBreaks));
        }

        [Test]
        public void Abs_Always_ReturnsAbsoluteVersionOfTransaction()
        {
            var resource1Amount = -14;
            var resource2Amount = 66;

            var original = ConstructSubject(resource1Amount, resource2Amount);

            var result = original.Abs();

            Assert.AreEqual(Mathf.Abs(resource1Amount), result.TransactionDictionary[resource1].Amount);
            Assert.AreEqual(Mathf.Abs(resource2Amount), result.TransactionDictionary[resource2].Amount);
        }

        [Test]
        public void Add_Always_ReturnsCombinedTransaction()
        {
            var transaction1Resource1Amount = -6326;
            var transaction1Resource2Amount = 5551;

            var transaction1 = ConstructSubject(transaction1Resource1Amount, transaction1Resource2Amount);

            var transaction2Resource1Amount = 1095;
            var transaction2Resource2Amount = -19891;

            var transaction2 = ConstructSubject(transaction2Resource1Amount, transaction2Resource2Amount);

            var result = transaction1.Add(transaction2);

            Assert.AreEqual(transaction1Resource1Amount + transaction2Resource1Amount, result.TransactionDictionary[resource1].Amount);
            Assert.AreEqual(transaction1Resource2Amount + transaction2Resource2Amount, result.TransactionDictionary[resource2].Amount);
        }

        [Test]
        public void Subtract_Always_ReturnsSubtractedTransaction()
        {
            var transaction1Resource1Amount = -18478;
            var transaction1Resource2Amount = 9828;

            var transaction1 = ConstructSubject(transaction1Resource1Amount, transaction1Resource2Amount);

            var transaction2Resource1Amount = 89234;
            var transaction2Resource2Amount = -908;

            var transaction2 = ConstructSubject(transaction2Resource1Amount, transaction2Resource2Amount);

            var result = transaction1.Subtract(transaction2);

            Assert.AreEqual(transaction1Resource1Amount - transaction2Resource1Amount, result.TransactionDictionary[resource1].Amount);
            Assert.AreEqual(transaction1Resource2Amount - transaction2Resource2Amount, result.TransactionDictionary[resource2].Amount);
        }

        [Test]
        public void MultiplyByInt_Always_ReturnsMultipliedTransaction()
        {
            var resource1Amount = -567;
            var resource2Amount = 562;

            var original = ConstructSubject(resource1Amount, resource2Amount);

            var multiplier = 3;

            var result = original.Multiply(multiplier);

            Assert.AreEqual(resource1Amount * multiplier, result.TransactionDictionary[resource1].Amount);
            Assert.AreEqual(resource2Amount * multiplier, result.TransactionDictionary[resource2].Amount);
        }

        [Test]
        public void MutiplyByFloat_WithoutRoundingMode_ReturnsMultipliedTransactionRoundedToNearest()
        {
            var resource1Amount = 6;
            var resource2Amount = 2;

            var original = ConstructSubject(resource1Amount, resource2Amount);

            var multiplier = 1.1f;

            var result = original.Multiply(multiplier);

            Assert.AreEqual(7, result.TransactionDictionary[resource1].Amount);
            Assert.AreEqual(2, result.TransactionDictionary[resource2].Amount);
        }

        [Test]
        public void MultiplyByFloat_RoundingModeNearst_ReturnsMultipliedTransactionRoundedToNearest()
        {
            var resource1Amount = 6;
            var resource2Amount = 2;

            var original = ConstructSubject(resource1Amount, resource2Amount);

            var multiplier = 1.1f;

            var result = original.Multiply(multiplier, RoundingMode.NEAREST);

            Assert.AreEqual(7, result.TransactionDictionary[resource1].Amount);
            Assert.AreEqual(2, result.TransactionDictionary[resource2].Amount);
        }

        [Test]
        public void MultiplyByFloat_RoundingModeDown_ReturnsMultipliedTransactionRoundedDown()
        {
            var resource1Amount = 6;
            var resource2Amount = 2;

            var original = ConstructSubject(resource1Amount, resource2Amount);

            var multiplier = 1.1f;

            var result = original.Multiply(multiplier, RoundingMode.DOWN);

            Assert.AreEqual(6, result.TransactionDictionary[resource1].Amount);
            Assert.AreEqual(2, result.TransactionDictionary[resource2].Amount);
        }

        [Test]
        public void MultiplyByfloat_RoundingModeUp_ReturnsMultipliedTransactionRoundedUp()
        {
            var resource1Amount = 6;
            var resource2Amount = 2;

            var original = ConstructSubject(resource1Amount, resource2Amount);

            var multiplier = 1.1f;

            var result = original.Multiply(multiplier, RoundingMode.UP);

            Assert.AreEqual(7, result.TransactionDictionary[resource1].Amount);
            Assert.AreEqual(3, result.TransactionDictionary[resource2].Amount);
        }
    }
}