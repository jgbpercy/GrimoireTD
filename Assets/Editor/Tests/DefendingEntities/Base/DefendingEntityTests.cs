using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Dependencies;
using GrimoireTD.Attributes;
using GrimoireTD.Abilities;
using GrimoireTD.Map;
using GrimoireTD.DefendingEntities.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Abilities.DefendMode.Projectiles;

namespace GrimoireTD.Tests.DefendingEntityTests
{
    public abstract class DefendingEntityTests
    {
        //Primitives and Basic Objects
        protected Coord startPosition = new Coord(2, 1);

        protected int defenderAuraRange = 1;

        protected int matchingFlatHexBonus1Resource1Amount = 3;
        protected int matchingFlatHexBonus1Resource2Amount = 5;
        protected int matchingFlatHexBonus2Resource1Amount = 4;
        protected int matchingFlatHexBonus2Resource2Amount = 7;
        protected int otherFlatHexBonus1Resource1Amount = 2;
        protected int otherFlatHexBonus1Resource2Amount = 9;

        //Model and Frame Updater
        protected FrameUpdaterStub frameUpdater;

        protected IReadOnlyGameModel gameModel = Substitute.For<IReadOnlyGameModel>();

        protected IReadOnlyMapData mapData = Substitute.For<IReadOnlyMapData>();

        protected IReadOnlyGameStateManager gameStateManager = Substitute.For<IReadOnlyGameStateManager>();

        protected IReadOnlyEconomyManager economyManager = Substitute.For<IReadOnlyEconomyManager>();

        protected IHexData startHexData = Substitute.For<IHexData>();

        protected CallbackList<IDefenderAura> aurasAtStartHex;

        //Instance Dependency Provider Deps
        protected IAttributes<DEAttrName> attributes = Substitute.For<IAttributes<DEAttrName>>();

        protected IAbilities abilities = Substitute.For<IAbilities>();

        //Template Deps
        protected INamedAttributeModifier<DEAttrName> attributeModifier = Substitute.For<INamedAttributeModifier<DEAttrName>>();

        protected IHexType occupiedHexType = Substitute.For<IHexType>();
        protected IHexType otherHexType = Substitute.For<IHexType>();

        protected IResource resource1 = Substitute.For<IResource>();
        protected IResource resource2 = Substitute.For<IResource>();

        protected IEconomyTransaction matchingFlatHexOccupationBonus1Transaction;
        protected IEconomyTransaction matchingFlatHexOccupationBonus2Transaction;
        protected IEconomyTransaction otherFlatHexOccupationBonusTransaction;

        protected IHexOccupationBonus matchingFlatHexOccupationBonus1 = Substitute.For<IHexOccupationBonus>();
        protected IHexOccupationBonus matchingFlatHexOccupationBonus2 = Substitute.For<IHexOccupationBonus>();
        protected IHexOccupationBonus otherFlatHexOccupationBonus = Substitute.For<IHexOccupationBonus>();

        protected IAbilityTemplate abilityTemplate = Substitute.For<IAbilityTemplate>();
        protected IAbility ability = Substitute.For<IAbility>();

        protected IDefenderAuraTemplate defenderAuraTemplate = Substitute.For<IDefenderAuraTemplate>();
        protected IDefenderAura defenderAura = Substitute.For<IDefenderAura>();

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            //Model and Frame Updater
            economyManager.Resources.Returns(new List<IResource>
            {
                resource1,
                resource2
            });

            gameModel.MapData.Returns(mapData);
            gameModel.GameStateManager.Returns(gameStateManager);
            gameModel.EconomyManager.Returns(economyManager);

            mapData.CoordsInRange(defenderAuraRange, startPosition).Returns(new List<Coord> { startPosition });

            DepsProv.SetTheGameModel(gameModel);

            //Instance Dependency Provider Deps
            DepsProv.DefendingEntityAttributes = () =>
            {
                return attributes;
            };

            DepsProv.Abilities = (attachedToDefendingEntity) =>
            {
                return abilities;
            };
        }

        [SetUp]
        public virtual void EachTestSetUp()
        {
            frameUpdater = new FrameUpdaterStub();

            attributes.ClearReceivedCalls();
            abilities.ClearReceivedCalls();
            startHexData.ClearReceivedCalls();

            aurasAtStartHex = new CallbackList<IDefenderAura>();

            startHexData.DefenderAurasHere.Returns(aurasAtStartHex);
            startHexData.HexType.Returns(occupiedHexType);

            mapData.GetHexAt(startPosition).Returns(startHexData);

            attributes.TryRemoveModifier(Arg.Any<INamedAttributeModifier<DEAttrName>>()).Returns(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(DepsProv).TypeInitializer.Invoke(null, null);
        }

        protected abstract CDefendingEntity ConstructSubject();

        protected virtual void SetUpBaseCharacteristics(IDefendingEntityImprovement baseCharacteristics)
        {
            baseCharacteristics.AttributeModifiers.Returns(new List<INamedAttributeModifier<DEAttrName>>
            {
                attributeModifier
            });

            matchingFlatHexOccupationBonus1Transaction = new CEconomyTransaction(new List<IResourceTransaction>
            {
                new CResourceTransaction(resource1, matchingFlatHexBonus1Resource1Amount),
                new CResourceTransaction(resource2, matchingFlatHexBonus1Resource2Amount)
            });

            matchingFlatHexOccupationBonus2Transaction = new CEconomyTransaction(new List<IResourceTransaction>
            {
                new CResourceTransaction(resource1, matchingFlatHexBonus2Resource1Amount),
                new CResourceTransaction(resource2, matchingFlatHexBonus2Resource2Amount)
            });

            otherFlatHexOccupationBonusTransaction = new CEconomyTransaction(new List<IResourceTransaction>
            {
                new CResourceTransaction(resource1, otherFlatHexBonus1Resource1Amount),
                new CResourceTransaction(resource2, otherFlatHexBonus1Resource2Amount)
            });

            matchingFlatHexOccupationBonus1.ResourceGain.Returns(matchingFlatHexOccupationBonus1Transaction);
            matchingFlatHexOccupationBonus2.ResourceGain.Returns(matchingFlatHexOccupationBonus2Transaction);
            otherFlatHexOccupationBonus.ResourceGain.Returns(otherFlatHexOccupationBonusTransaction);

            matchingFlatHexOccupationBonus1.HexType.Returns(occupiedHexType);
            matchingFlatHexOccupationBonus2.HexType.Returns(occupiedHexType);
            otherFlatHexOccupationBonus.HexType.Returns(otherHexType);

            baseCharacteristics.FlatHexOccupationBonuses.Returns(new List<IHexOccupationBonus>
            {
                matchingFlatHexOccupationBonus1,
                matchingFlatHexOccupationBonus2,
                otherFlatHexOccupationBonus
            });

            baseCharacteristics.Abilities.Returns(new List<IAbilityTemplate>
            {
                abilityTemplate
            });

            abilityTemplate.GenerateAbility(Arg.Any<IDefendingEntity>()).Returns(ability);

            baseCharacteristics.Auras.Returns(new List<IDefenderAuraTemplate>
            {
                defenderAuraTemplate
            });

            defenderAuraTemplate.GenerateDefenderAura(Arg.Any<IDefendingEntity>()).Returns(defenderAura);

            defenderAura.Range.Returns(defenderAuraRange);
        }

        [Test]
        public void Ctor_Always_AddsBaseCharacteristicsAttributeModifiersToAttributes()
        {
            ConstructSubject();

            attributes.Received(1).AddModifier(attributeModifier);
        }

        [Test]
        public void Ctor_Always_AddsBaseCharacteristicsFlatHexOccupationBonusToFlatHexOccupationBonuses()
        {
            var subject = ConstructSubject();

            Assert.True(subject.FlatHexOccupationBonuses.Contains(matchingFlatHexOccupationBonus1));
            Assert.True(subject.FlatHexOccupationBonuses.Contains(matchingFlatHexOccupationBonus2));
            Assert.True(subject.FlatHexOccupationBonuses.Contains(otherFlatHexOccupationBonus));
        }

        [Test]
        public void Ctor_Always_AddsBaseCharacteristicAbilitiesToAbilities()
        {
            ConstructSubject();

            abilities.Received(1).AddAbility(ability);
        }

        [Test]
        public void Ctor_Always_AddsBaseCharacteristicsDefenderAurasToAurasEmitted()
        {
            var subject = ConstructSubject();

            Assert.True(subject.AurasEmitted.Contains(defenderAura));
        }

        [Test]
        public void Ctor_IfBaseCharacteristicsContainsADefenderAura_AppliesAuraToHexesInRange()
        {
            var coordInRange = new Coord(1, 2);

            var hexInRangeData = Substitute.For<IHexData>();

            mapData.GetHexAt(coordInRange).Returns(hexInRangeData);

            mapData.CoordsInRange(defenderAuraRange, startPosition).Returns(new List<Coord>
            {
                startPosition,
                coordInRange
            });

            ConstructSubject();

            startHexData.Received(1).AddDefenderAura(defenderAura);
            hexInRangeData.Received(1).AddDefenderAura(defenderAura);
        }

        [Test]
        public void Ctor_IfStartingHexContainsAnAuraThatAffectsBothDefendingEntityTypes_AppliesAuraToSelf()
        {
            var hexDataWithAura = Substitute.For<IHexData>();

            var auraAttributeModifier = Substitute.For<INamedAttributeModifier<DEAttrName>>();
            var auraAttributeModifierList = new List<INamedAttributeModifier<DEAttrName>>
            {
                auraAttributeModifier
            };

            var aura = Substitute.For<IDefenderAura>();
            aura.DefenderEffectTemplate.Affects.Returns(DefenderEffectAffectsType.BOTH);
            aura.DefenderAuraTemplate.Affects.Returns(DefenderEffectAffectsType.BOTH);
            aura.DefenderEffectTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);
            aura.DefenderAuraTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);

            hexDataWithAura.DefenderAurasHere.Returns(new CallbackList<IDefenderAura> { aura });

            mapData.GetHexAt(startPosition).Returns(hexDataWithAura);

            var subject = ConstructSubject();

            Assert.True(subject.AffectedByDefenderAuras.Contains(aura));
            attributes.Received(1).AddModifier(auraAttributeModifier);
        }

        [Test]
        public void OnAuraAddedToHex_IfAuraAffectsBoth_AppliesAura()
        {
            var auraAttributeModifier = Substitute.For<INamedAttributeModifier<DEAttrName>>();
            var auraAttributeModifierList = new List<INamedAttributeModifier<DEAttrName>>
            {
                auraAttributeModifier
            };

            var aura = Substitute.For<IDefenderAura>();
            aura.DefenderEffectTemplate.Affects.Returns(DefenderEffectAffectsType.BOTH);
            aura.DefenderAuraTemplate.Affects.Returns(DefenderEffectAffectsType.BOTH);
            aura.DefenderEffectTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);
            aura.DefenderAuraTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);

            var subject = ConstructSubject();

            aurasAtStartHex.Add(aura);

            Assert.True(subject.AffectedByDefenderAuras.Contains(aura));
            attributes.Received(1).AddModifier(auraAttributeModifier);
        }

        [Test]
        public void OnAuraRemovedFromHex_IfAuraAffectsBoth_RemovesAura()
        {
            var auraAttributeModifier = Substitute.For<INamedAttributeModifier<DEAttrName>>();
            var auraAttributeModifierList = new List<INamedAttributeModifier<DEAttrName>>
            {
                auraAttributeModifier
            };

            var aura = Substitute.For<IDefenderAura>();
            aura.DefenderEffectTemplate.Affects.Returns(DefenderEffectAffectsType.BOTH);
            aura.DefenderAuraTemplate.Affects.Returns(DefenderEffectAffectsType.BOTH);
            aura.DefenderEffectTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);
            aura.DefenderAuraTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);

            var subject = ConstructSubject();

            aurasAtStartHex.Add(aura);

            aurasAtStartHex.TryRemove(aura);

            Assert.False(subject.AffectedByDefenderAuras.Contains(aura));
            attributes.Received(1).TryRemoveModifier(auraAttributeModifier);
        }

        [Test]
        public void OnEnterBuildModeEvent_Always_FiresOnTriggeredFlatHexOccupationBonusEventWithCorrectTransaction()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnTriggeredFlatHexOccupationBonus>();
            subject.OnTriggeredFlatHexOccupationBonus += eventTester.Handler;

            gameStateManager.OnEnterBuildMode += Raise.EventWith<EAOnEnterBuildMode>();

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => {
                return 
                    args.Transaction.GetTransactionAmount(resource1) == matchingFlatHexBonus1Resource1Amount + matchingFlatHexBonus2Resource1Amount &&
                    args.Transaction.GetTransactionAmount(resource2) == matchingFlatHexBonus1Resource2Amount + matchingFlatHexBonus2Resource2Amount;
            });
        }

        [Test]
        public void CreatedProjectile_Always_FiresCreatedProjectileEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnProjectileCreated>();
            subject.OnProjectileCreated += eventTester.Handler;

            var projectile = Substitute.For<IProjectile>();

            subject.CreatedProjectile(projectile);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.Projectile == projectile);
        }
    }
}