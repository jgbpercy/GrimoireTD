using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using GrimoireTD.Creeps;
using GrimoireTD.Attributes;
using GrimoireTD.Dependencies;
using GrimoireTD.TemporaryEffects;
using GrimoireTD.Map;
using GrimoireTD.Technical;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.DefendingEntities;
using System;

namespace GrimoireTD.Tests.CreepTests
{
    public class CreepTests
    {
        //Primitives and Basic Objects
        private Vector3 startingPosition = new Coord(1, 1).ToPositionVector();

        private Coord startNode = new Coord(1, 1);
        private Coord secondNode = new Coord(1, 2);
        private Coord thirdNode = new Coord(2, 2);

        private int maxHitpoints = 10;

        private float defaultDeltaTime = 0.2f;

        private float startSpeed = 0.5f;

        private float damageEffectActualMagnitude = 5f;
        private int damageBlock = 1;
        private float damageResistance = 0.25f;

        private float killingDamageEffectActualMagnitude = 50f;

        private CreepAttrName attributeModifierAttackAttributeModified = CreepAttrName.armorMultiplier;

        private string attributeModifierAttackEffectTypeName = "attribute";
        private string resistanceModifierAttackEffectTypeName = "resistance";
        private string blockModifierAttackEffectTypeName = "block";

        //Model Deps
        private IGameModel gameModel = Substitute.For<IGameModel>();

        private IReadOnlyMapData mapData = Substitute.For<IReadOnlyMapData>();

        //Dependency Provider Deps
        private IAttributes<CreepAttrName> attributes = Substitute.For<IAttributes<CreepAttrName>>();

        private ITemporaryEffectsManager temporaryEffects = Substitute.For<ITemporaryEffectsManager>();

        private IResistances resistances = Substitute.For<IResistances>();

        //Template Deps
        private ICreepTemplate creepTemplate = Substitute.For<ICreepTemplate>();

        private List<INamedAttributeModifier<CreepAttrName>> baseAttributes;

        private INamedAttributeModifier<CreepAttrName> baseAttributeModifier1 = Substitute.For<INamedAttributeModifier<CreepAttrName>>();
        private INamedAttributeModifier<CreepAttrName> baseAttributeModifier2 = Substitute.For<INamedAttributeModifier<CreepAttrName>>();

        //Other Objects Passed To Methods
        private IDefendingEntity defendingEntity = Substitute.For<IDefendingEntity>();

        private IAttackEffect damageAttackEffect = Substitute.For<IAttackEffect>();
        private IAttackEffect killingDamageAttackEffect = Substitute.For<IAttackEffect>();
        private IDamageEffectType damageAttackEffectType = Substitute.For<IDamageEffectType>();

        private IAttackEffect permanentAttributeModifierAttackEffect = Substitute.For<IAttackEffect>();
        private IAttributeModifierEffectType permanentAttributeModifierAttackEffectType = Substitute.For<IAttributeModifierEffectType>();
        private IAttributeModifierEffectType temporaryAttributeModifierAttackEffectType = Substitute.For<IAttributeModifierEffectType>();

        private IAttackEffect permanentResistanceModifierAttackEffect = Substitute.For<IAttackEffect>();
        private IResistanceModifierEffectType permanentResistanceModifierAttackEffectType = Substitute.For<IResistanceModifierEffectType>();
        private IResistanceModifierEffectType temporaryResistanceModifierAttackEffectType = Substitute.For<IResistanceModifierEffectType>();
        private ISpecificDamageEffectType resistanceTypeModified = Substitute.For<ISpecificDamageEffectType>();

        private IAttackEffect permanentBlockModifierAttackEffect = Substitute.For<IAttackEffect>();
        private IBlockModifierEffectType permanentBlockModifierAttackEffectType = Substitute.For<IBlockModifierEffectType>();
        private IBlockModifierEffectType temporaryBlockModifierAttackEffectType = Substitute.For<IBlockModifierEffectType>();
        private ISpecificDamageEffectType blockTypeModified = Substitute.For<ISpecificDamageEffectType>();

        private IAttackEffect temporaryAttributeModifierAttackEffect = Substitute.For<IAttackEffect>();

        private IAttackEffect temporaryResistanceModifierAttackEffect = Substitute.For<IAttackEffect>();

        private IAttackEffect temporaryBlockModifierAttackEffect = Substitute.For<IAttackEffect>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //Model Deps
            mapData.CreepPath.Returns(new List<Coord>
            {
                thirdNode,
                secondNode,
                startNode
            });

            gameModel.MapData.Returns(mapData);

            GameModels.Models.Add(gameModel);

            //Dependency Provider Deps
            DependencyProvider.CreepAttributes = () => attributes;

            DependencyProvider.TemporaryEffectsManager = () => temporaryEffects;

            DependencyProvider.Resistances = (creep, baseResistance) => resistances;

            //Template Deps
            creepTemplate.MaxHitpoints.Returns(maxHitpoints);

            baseAttributes = new List<INamedAttributeModifier<CreepAttrName>>
            {
                baseAttributeModifier1,
                baseAttributeModifier2
            };

            creepTemplate.BaseAttributes.Returns(baseAttributes);

            //Other Objects Passed To Methods
            damageAttackEffect.AttackEffectType.Returns(damageAttackEffectType);
            killingDamageAttackEffect.AttackEffectType.Returns(damageAttackEffectType);

            damageAttackEffect.GetActualMagnitude(defendingEntity).Returns(damageEffectActualMagnitude);
            killingDamageAttackEffect.GetActualMagnitude(defendingEntity).Returns(killingDamageEffectActualMagnitude);

            resistances.GetResistance(damageAttackEffectType).Value.Returns(damageResistance);
            resistances.GetBlock(damageAttackEffectType).Value.Returns(damageBlock);

            permanentAttributeModifierAttackEffect.AttackEffectType.Returns(permanentAttributeModifierAttackEffectType);
            permanentAttributeModifierAttackEffectType.Temporary.Returns(false);
            permanentAttributeModifierAttackEffectType.CreepAttributeName.Returns(attributeModifierAttackAttributeModified);
            permanentAttributeModifierAttackEffectType.EffectName().Returns(attributeModifierAttackEffectTypeName);

            temporaryAttributeModifierAttackEffect.AttackEffectType.Returns(temporaryAttributeModifierAttackEffectType);
            temporaryAttributeModifierAttackEffectType.Temporary.Returns(true);
            temporaryAttributeModifierAttackEffectType.CreepAttributeName.Returns(attributeModifierAttackAttributeModified);
            temporaryAttributeModifierAttackEffectType.EffectName().Returns(attributeModifierAttackEffectTypeName);

            permanentResistanceModifierAttackEffect.AttackEffectType.Returns(permanentResistanceModifierAttackEffectType);
            permanentResistanceModifierAttackEffectType.Temporary.Returns(false);
            permanentResistanceModifierAttackEffectType.ResistanceToModify.Returns(resistanceTypeModified);
            permanentResistanceModifierAttackEffectType.EffectName().Returns(resistanceModifierAttackEffectTypeName);

            temporaryResistanceModifierAttackEffect.AttackEffectType.Returns(temporaryResistanceModifierAttackEffectType);
            temporaryResistanceModifierAttackEffectType.Temporary.Returns(true);
            temporaryResistanceModifierAttackEffectType.ResistanceToModify.Returns(resistanceTypeModified);
            temporaryResistanceModifierAttackEffectType.EffectName().Returns(resistanceModifierAttackEffectTypeName);

            permanentBlockModifierAttackEffect.AttackEffectType.Returns(permanentBlockModifierAttackEffectType);
            permanentBlockModifierAttackEffectType.Temporary.Returns(false);
            permanentBlockModifierAttackEffectType.BlockTypeToModify.Returns(blockTypeModified);
            permanentBlockModifierAttackEffectType.EffectName().Returns(blockModifierAttackEffectTypeName);

            temporaryBlockModifierAttackEffect.AttackEffectType.Returns(temporaryBlockModifierAttackEffectType);
            temporaryBlockModifierAttackEffectType.Temporary.Returns(true);
            temporaryBlockModifierAttackEffectType.BlockTypeToModify.Returns(blockTypeModified);
            temporaryBlockModifierAttackEffectType.EffectName().Returns(blockModifierAttackEffectTypeName);
        }

        [SetUp]
        public void EachTestSetUp()
        {
            attributes.ClearReceivedCalls();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(DependencyProvider).TypeInitializer.Invoke(null, null);
            typeof(GameModels).TypeInitializer.Invoke(null, null);
        }

        private CCreep ConstructSubject()
        {
            return new CCreep(creepTemplate, startingPosition);
        }

        [Test]
        public void Ctor_Always_CreatesCreepAtStartingPosition()
        {
            var subject = ConstructSubject();

            Assert.AreEqual(startingPosition, subject.Position);
        }

        [Test]
        public void Ctor_Always_CreatesCreepWithMaxHitPoints()
        {
            var subject = ConstructSubject();

            Assert.AreEqual(maxHitpoints, subject.CurrentHitpoints);
        }

        [Test]
        public void Ctor_Always_AddsBaseAttributes()
        {
            var subject = ConstructSubject();

            attributes.Received(1).AddModifier(baseAttributeModifier1);
            attributes.Received(1).AddModifier(baseAttributeModifier2);
        }

        [Test]
        public void TargetPosition_Always_ReturnsCurrentPositionWithZPreset()
        {
            var subject = ConstructSubject();

            AssertExt.Approximately(
                new Vector3(startingPosition.x, startingPosition.y, CCreep.TARGET_POSITION_Z_OFFSET), 
                subject.TargetPosition());
        }

        [Test]
        public void CurrentSpeed_WhenRawSpeedAttributeChanged_ChangesValue()
        {
            var subject = ConstructSubject();

            var rawSpeed = 1f;
            var speedMultiplier = 0.3f;

            attributes.Get(CreepAttrName.rawSpeed).Value().Returns(rawSpeed);

            attributes.Get(CreepAttrName.speedMultiplier).Value().Returns(speedMultiplier);

            attributes.Get(CreepAttrName.rawSpeed).OnAttributeChanged += Raise.EventWith(new EAOnAttributeChanged(rawSpeed));

            AssertExt.Approximately(1.3f, subject.CurrentSpeed);
        }

        [Test]
        public void Creep_WhenRawSpeedAttributeChanged_FiresOnSpeedChangedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAttributeChanged>();
            subject.OnSpeedChanged += eventTester.Handler;

            var rawSpeed = 1f;
            var speedMultiplier = 0.3f;

            attributes.Get(CreepAttrName.rawSpeed).Value().Returns(rawSpeed);

            attributes.Get(CreepAttrName.speedMultiplier).Value().Returns(speedMultiplier);

            attributes.Get(CreepAttrName.rawSpeed).OnAttributeChanged += Raise.EventWith(new EAOnAttributeChanged(rawSpeed));

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => CustomMath.Approximately(1.3f, args.NewValue));
        }

        [Test]
        public void CurrentSpeed_WhenSpeedMultiplierAttributeChanged_ChangedValue()
        {
            var subject = ConstructSubject();

            var rawSpeed = 1f;
            var speedMultiplier = 0.3f;

            attributes.Get(CreepAttrName.rawSpeed).Value().Returns(rawSpeed);

            attributes.Get(CreepAttrName.speedMultiplier).Value().Returns(speedMultiplier);

            attributes.Get(CreepAttrName.speedMultiplier).OnAttributeChanged += Raise.EventWith(new EAOnAttributeChanged(speedMultiplier));

            AssertExt.Approximately(1.3f, subject.CurrentSpeed);
        }

        [Test]
        public void Creep_WhenSpeedMultiplierAttributeChanged_FiresOnSpeedChangedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAttributeChanged>();
            subject.OnSpeedChanged += eventTester.Handler;

            var rawSpeed = 1f;
            var speedMultiplier = 0.3f;

            attributes.Get(CreepAttrName.rawSpeed).Value().Returns(rawSpeed);

            attributes.Get(CreepAttrName.speedMultiplier).Value().Returns(speedMultiplier);

            attributes.Get(CreepAttrName.speedMultiplier).OnAttributeChanged += Raise.EventWith(new EAOnAttributeChanged(speedMultiplier));

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => CustomMath.Approximately(1.3f, args.NewValue));
        }

        [Test]
        public void CurrentArmor_WhenRawArmorAttributeChanged_ChangesValue()
        {
            var subject = ConstructSubject();

            var rawArmor = 3f;
            var armorMultiplier = 0.5f;

            attributes.Get(CreepAttrName.rawArmor).Value().Returns(rawArmor);

            attributes.Get(CreepAttrName.armorMultiplier).Value().Returns(armorMultiplier);

            attributes.Get(CreepAttrName.rawArmor).OnAttributeChanged += Raise.EventWith(new EAOnAttributeChanged(rawArmor));

            AssertExt.Approximately(4.5f, subject.CurrentArmor);
        }

        [Test]
        public void Creep_WhenRawArmorAttributeChanged_FiresOnArmorChangedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAttributeChanged>();
            subject.OnArmorChanged += eventTester.Handler;

            var rawArmor = 3f;
            var armorMultiplier = 0.5f;

            attributes.Get(CreepAttrName.rawArmor).Value().Returns(rawArmor);

            attributes.Get(CreepAttrName.armorMultiplier).Value().Returns(armorMultiplier);

            attributes.Get(CreepAttrName.rawArmor).OnAttributeChanged += Raise.EventWith(new EAOnAttributeChanged(rawArmor));

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => CustomMath.Approximately(4.5f, args.NewValue));
        }

        [Test]
        public void CurrentArmor_WhenArmorMultiplierAttributeChanged_ChangesValue()
        {
            var subject = ConstructSubject();

            var rawArmor = 3f;
            var armorMultiplier = 0.5f;

            attributes.Get(CreepAttrName.rawArmor).Value().Returns(rawArmor);

            attributes.Get(CreepAttrName.armorMultiplier).Value().Returns(armorMultiplier);

            attributes.Get(CreepAttrName.armorMultiplier).OnAttributeChanged += Raise.EventWith(new EAOnAttributeChanged(armorMultiplier));

            AssertExt.Approximately(4.5f, subject.CurrentArmor);
        }

        [Test]
        public void Creep_WhenArmorMultiplierAttributeChanged_FiresOnArmorChangedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAttributeChanged>();
            subject.OnArmorChanged += eventTester.Handler;

            var rawArmor = 3f;
            var armorMultiplier = 0.5f;

            attributes.Get(CreepAttrName.rawArmor).Value().Returns(rawArmor);

            attributes.Get(CreepAttrName.armorMultiplier).Value().Returns(armorMultiplier);

            attributes.Get(CreepAttrName.armorMultiplier).OnAttributeChanged += Raise.EventWith(new EAOnAttributeChanged(armorMultiplier));

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => CustomMath.Approximately(4.5f, args.NewValue));
        }

        [Test]
        public void ModelObjectFrameUpdate_AfterConstruction_MovesCreepTowardsFirstNodeAtSpeedGivenByAttributes()
        {
            var subject = ConstructSubject();

            attributes.Get(CreepAttrName.rawSpeed).Value().Returns(startSpeed);

            attributes.Get(CreepAttrName.speedMultiplier).Value().Returns(0f);

            attributes.Get(CreepAttrName.rawSpeed).OnAttributeChanged += Raise.EventWith(new EAOnAttributeChanged(startSpeed));

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            var startToSecondNodeVector = new Vector3(
                secondNode.ToPositionVector().x - startNode.ToPositionVector().x,
                secondNode.ToPositionVector().y - startNode.ToPositionVector().y,
                secondNode.ToPositionVector().z - startNode.ToPositionVector().z
            );

            float startToSecondNodeDistance = Mathf.Sqrt(
                Mathf.Pow(startToSecondNodeVector.x, 2) +
                Mathf.Pow(startToSecondNodeVector.y, 2) +
                Mathf.Pow(startToSecondNodeVector.z, 2)
            );

            float expectedProportionOfTotalDistanceTravelled = (defaultDeltaTime * startSpeed) / startToSecondNodeDistance;

            var expectedPosition = new Vector3(
                startingPosition.x + startToSecondNodeVector.x * expectedProportionOfTotalDistanceTravelled,
                startingPosition.y + startToSecondNodeVector.y * expectedProportionOfTotalDistanceTravelled,
                startingPosition.z + startToSecondNodeVector.z * expectedProportionOfTotalDistanceTravelled
            );

            AssertExt.Approximately(expectedPosition, subject.Position);
        }

        [Test]
        public void ModelObjectFrameUpdate_AfterMovingFarEnoughToReachFirstNode_MovesCreepTowardsNextNode()
        {
            var subject = ConstructSubject();

            attributes.Get(CreepAttrName.rawSpeed).Value().Returns(startSpeed);

            attributes.Get(CreepAttrName.speedMultiplier).Value().Returns(0f);

            attributes.Get(CreepAttrName.rawSpeed).OnAttributeChanged += Raise.EventWith(new EAOnAttributeChanged(startSpeed));

            var startToSecondNodeVector = new Vector3(
                secondNode.ToPositionVector().x - startNode.ToPositionVector().x,
                secondNode.ToPositionVector().y - startNode.ToPositionVector().y,
                secondNode.ToPositionVector().z - startNode.ToPositionVector().z
            );

            float startToSecondNodeDistance = Mathf.Sqrt(
                Mathf.Pow(startToSecondNodeVector.x, 2) +
                Mathf.Pow(startToSecondNodeVector.y, 2) +
                Mathf.Pow(startToSecondNodeVector.z, 2)
            );

            float deltaTimeToReachSecondNode = startToSecondNodeDistance / startSpeed;

            float deltaTimeToPass = deltaTimeToReachSecondNode - (defaultDeltaTime * 0.1f);

            subject.ModelObjectFrameUpdate(deltaTimeToPass);

            var positionAfterMovingToReachFirstNode = subject.Position;

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            var afterMovePositionToThirdNodeVector = new Vector3(
                thirdNode.ToPositionVector().x - positionAfterMovingToReachFirstNode.x,
                thirdNode.ToPositionVector().y - positionAfterMovingToReachFirstNode.y,
                thirdNode.ToPositionVector().z - positionAfterMovingToReachFirstNode.z
            );

            float afterMovePositionToThirdNodeDistance = Mathf.Sqrt(
                Mathf.Pow(afterMovePositionToThirdNodeVector.x, 2) +
                Mathf.Pow(afterMovePositionToThirdNodeVector.y, 2) +
                Mathf.Pow(afterMovePositionToThirdNodeVector.z, 2)
            );

            float expectedProportionOfTotalDistanceTravelled = (defaultDeltaTime * startSpeed) / afterMovePositionToThirdNodeDistance;

            var expectedPosition = new Vector3(
                positionAfterMovingToReachFirstNode.x + afterMovePositionToThirdNodeVector.x * expectedProportionOfTotalDistanceTravelled,
                positionAfterMovingToReachFirstNode.y + afterMovePositionToThirdNodeVector.y * expectedProportionOfTotalDistanceTravelled,
                positionAfterMovingToReachFirstNode.z + afterMovePositionToThirdNodeVector.z * expectedProportionOfTotalDistanceTravelled
            );

            AssertExt.Approximately(expectedPosition, subject.Position);
        }

        [Test]
        public void ModelObjectFrameUpdate_AfterMovingCreep_UpdatesDistanceFromEnd()
        {
            var subject = ConstructSubject();

            attributes.Get(CreepAttrName.rawSpeed).Value().Returns(startSpeed);

            attributes.Get(CreepAttrName.speedMultiplier).Value().Returns(0f);

            attributes.Get(CreepAttrName.rawSpeed).OnAttributeChanged += Raise.EventWith(new EAOnAttributeChanged(startSpeed));

            var startingDistanceFromEnd = subject.DistanceFromEnd;

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            AssertExt.Approximately(startingDistanceFromEnd - startSpeed * defaultDeltaTime, subject.DistanceFromEnd);
        }

        [Test]
        public void ApplyAttackEffects_PassedDamageEffect_ReducesHitpointsByCorrectAmount()
        {
            var subject = ConstructSubject();

            var attackEffects = new List<IAttackEffect>
            {
                damageAttackEffect
            };

            subject.ApplyAttackEffects(attackEffects, defendingEntity);

            Assert.AreEqual(maxHitpoints - 3, subject.CurrentHitpoints);
        }

        [Test]
        public void ApplyAttackEffects_PassedDamageEffect_FiresOnHealthChangedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnHealthChanged>();
            subject.OnHealthChanged += eventTester.Handler;

            var attackEffects = new List<IAttackEffect>
            {
                damageAttackEffect
            };

            subject.ApplyAttackEffects(attackEffects, defendingEntity);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.NewValue == maxHitpoints - 3);
        }

        [Test]
        public void ApplyAttackEffects_PassedDamageEffectWithEnoughDamageToKill_SetsHealthToZero()
        {
            var subject = ConstructSubject();

            var attackEffects = new List<IAttackEffect>
            {
                killingDamageAttackEffect
            };

            subject.ApplyAttackEffects(attackEffects, defendingEntity);

            Assert.AreEqual(0, subject.CurrentHitpoints);
        }

        [Test]
        public void ApplyAttackEffects_PasedDamageEffectWithEnoughDamageToKill_FiresOnDiedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EventArgs>();
            subject.OnDied += eventTester.Handler;

            var attackEffects = new List<IAttackEffect>
            {
                killingDamageAttackEffect
            };

            subject.ApplyAttackEffects(attackEffects, defendingEntity);

            eventTester.AssertFired(1);
        }

        [Test]
        public void ApplyAttackEffects_PassedPermanentAttributeModifierEffect_AddsModifierToAttributes()
        {
            var subject = ConstructSubject();

            var actualMagnitude = 3f;
            permanentAttributeModifierAttackEffect.GetActualMagnitude(defendingEntity).Returns(actualMagnitude);

            var attackEffects = new List<IAttackEffect>()
            {
                permanentAttributeModifierAttackEffect
            };

            subject.ApplyAttackEffects(attackEffects, defendingEntity);

            attributes.Received(1)
                .AddModifier(Arg.Is<INamedAttributeModifier<CreepAttrName>>(
                    x => x.Magnitude == actualMagnitude && x.AttributeName == attributeModifierAttackAttributeModified)
                );
        }

        [Test]
        public void ApplyAttackEffects_PassedPermanentResistanceModifierEffect_AddsModifierToResistances()
        {
            var subject = ConstructSubject();

            var actualMagnitude = 2f;
            permanentResistanceModifierAttackEffect.GetActualMagnitude(defendingEntity).Returns(actualMagnitude);

            var attackEffects = new List<IAttackEffect>()
            {
                permanentResistanceModifierAttackEffect
            };

            subject.ApplyAttackEffects(attackEffects, defendingEntity);

            resistances.Received(1)
                .AddResistanceModifier(Arg.Is<IResistanceModifier>(
                    x => x.Magnitude == actualMagnitude && x.DamageType == resistanceTypeModified)
                );
        }

        [Test]
        public void ApplyAttackEffects_PassedPermanentBlockModifierEffect_AddsModifierToResistances()
        {
            var subject = ConstructSubject();

            var actualMagnitude = 1;
            permanentBlockModifierAttackEffect.GetActualMagnitude(defendingEntity).Returns(actualMagnitude);

            var attackEffects = new List<IAttackEffect>()
            {
                permanentBlockModifierAttackEffect
            };

            subject.ApplyAttackEffects(attackEffects, defendingEntity);

            resistances.Received(1)
                .AddBlockModifier(Arg.Is<IBlockModifier>(
                    x => x.Magnitude == actualMagnitude && x.DamageType == blockTypeModified)
                );
        }

        [Test]
        public void ApplyAttackEffects_PassedTemporaryAttributeModifierEffect_AddsEffectToTemporaryEffects()
        {
            var subject = ConstructSubject();

            var actualMagnitude = 4f;
            temporaryAttributeModifierAttackEffect.GetActualMagnitude(defendingEntity).Returns(actualMagnitude);

            var actualDuration = 5f;
            temporaryAttributeModifierAttackEffect.GetActualDuration(defendingEntity).Returns(actualDuration);

            var attackEffects = new List<IAttackEffect>()
            {
                temporaryAttributeModifierAttackEffect
            };

            subject.ApplyAttackEffects(attackEffects, defendingEntity);

            temporaryEffects.Received(1)
                .ApplyEffect(
                    temporaryAttributeModifierAttackEffectType,
                    actualMagnitude,
                    actualDuration,
                    attributeModifierAttackEffectTypeName,
                    Arg.Any<Action>(),
                    Arg.Any<EventHandler<EAOnTemporaryEffectEnd>>()
                );
        }

        [Test]
        public void ApplyAttackEffects_PassedTemporaryResistanceModifierEffect_AddsEffectToTemporaryEffects()
        {
            var subject = ConstructSubject();

            var actualMagnitude = 5f;
            temporaryResistanceModifierAttackEffect.GetActualMagnitude(defendingEntity).Returns(actualMagnitude);

            var actualDuration = 6f;
            temporaryResistanceModifierAttackEffect.GetActualDuration(defendingEntity).Returns(actualDuration);

            var attackEffects = new List<IAttackEffect>()
            {
                temporaryResistanceModifierAttackEffect
            };

            subject.ApplyAttackEffects(attackEffects, defendingEntity);

            temporaryEffects.Received(1)
                .ApplyEffect(
                    temporaryResistanceModifierAttackEffectType,
                    actualMagnitude,
                    actualDuration,
                    resistanceModifierAttackEffectTypeName,
                    Arg.Any<Action>(),
                    Arg.Any<EventHandler<EAOnTemporaryEffectEnd>>()
                );
        }

        [Test]
        public void ApplyAttackEffects_PassedTemporaryBlockModifierEffect_AddsEffectToTemporaryEffects()
        {
            var subject = ConstructSubject();

            var actualMagnitude = 6f;
            temporaryBlockModifierAttackEffect.GetActualMagnitude(defendingEntity).Returns(actualMagnitude);

            var actualDuration = 7f;
            temporaryBlockModifierAttackEffect.GetActualDuration(defendingEntity).Returns(actualDuration);

            var attackEffects = new List<IAttackEffect>()
            {
                temporaryBlockModifierAttackEffect
            };

            subject.ApplyAttackEffects(attackEffects, defendingEntity);

            temporaryEffects.Received(1)
                .ApplyEffect(
                    temporaryBlockModifierAttackEffectType,
                    actualMagnitude,
                    actualDuration,
                    blockModifierAttackEffectTypeName,
                    Arg.Any<Action>(),
                    Arg.Any<EventHandler<EAOnTemporaryEffectEnd>>()
                );
        }

        [Test]
        public void ApplyAttackEffects_PassedMultipleEffects_AppliesAllEffects()
        {
            var subject = ConstructSubject();

            var actualMagnitude = 3f;
            permanentAttributeModifierAttackEffect.GetActualMagnitude(defendingEntity).Returns(actualMagnitude);

            var attackEffects = new List<IAttackEffect>
            {
                damageAttackEffect,
                permanentAttributeModifierAttackEffect
            };

            subject.ApplyAttackEffects(attackEffects, defendingEntity);

            Assert.AreEqual(maxHitpoints - 3, subject.CurrentHitpoints);

            attributes.Received(1)
                .AddModifier(Arg.Is<INamedAttributeModifier<CreepAttrName>>(
                    x => x.Magnitude == actualMagnitude && x.AttributeName == attributeModifierAttackAttributeModified)
                );

        }
    }
}