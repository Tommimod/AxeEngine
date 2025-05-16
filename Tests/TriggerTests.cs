using System.Collections.Generic;
using NUnit.Framework;

namespace AxeEngine.Tests
{
    [TestFixture]
    public class TriggerTests
    {
        private World _world;

        [SetUp]
        public void SetUp()
        {
            _world = new World();
        }

        [TearDown]
        public void TearDown()
        {
            _world.Dispose();
            _world = null;
        }

        [Test]
        public void Trigger_ShouldBeAdded_True()
        {
            var actor = _world.CreateActor();
            var ability = new TestAddedAbility(_world);
            _world.AbilityManager.AddAbility(ability);
            actor.AddProp<TestStruct>();

            _world.AbilityManager.PerformUpdate();
            Assert.AreEqual(1, actor.GetProp<TestStruct>().Value);
        }

        [Test]
        public void Trigger_ShouldBeAdded_False()
        {
            var actor = _world.CreateActor();
            actor.AddProp<TestStruct>();
            var ability = new TestAddedAbility(_world);
            _world.AbilityManager.AddAbility(ability);

            _world.AbilityManager.PerformUpdate();
            Assert.AreNotEqual(1, actor.GetProp<TestStruct>().Value);
        }

        [Test]
        public void Trigger_ShouldBeReplaced_True()
        {
            var actor = _world.CreateActor();
            var ability = new TestReplacedAbility(_world);
            _world.AbilityManager.AddAbility(ability);

            actor.AddProp<TestStruct>();
            _world.AbilityManager.PerformUpdate();
            Assert.AreEqual(0, actor.GetProp<TestStruct>().Value);

            actor.ReplaceProp(new TestStruct { Value = 1 });
            _world.AbilityManager.PerformUpdate();
            Assert.AreEqual(2, actor.GetProp<TestStruct>().Value);
        }

        [Test]
        public void Trigger_ShouldBeRemoved_True()
        {
            var actor = _world.CreateActor().AddProp<TestStruct>();
            var ability = new TestRemovedAbility(_world);
            _world.AbilityManager.AddAbility(ability);

            actor.RemoveProp<TestStruct>();
            _world.AbilityManager.PerformUpdate();
            Assert.AreEqual(1, actor.GetProp<TestStruct>().Value);
        }

        private struct TestStruct
        {
            public int Value;
        }

        private class TestAddedAbility : AReactiveAbility
        {
            public TestAddedAbility(World world) : base(world)
            {
            }

            public override Trigger TriggerBy()
            {
                return new Trigger().Added<TestStruct>();
            }

            public override bool IsCanExecute(IActor actor)
            {
                return true;
            }

            public override void Execute(HashSet<IActor> actors)
            {
                foreach (var actor in actors)
                {
                    actor.ReplaceProp(new TestStruct { Value = 1 });
                }
            }
        }

        private class TestReplacedAbility : AReactiveAbility
        {
            public TestReplacedAbility(World world) : base(world)
            {
            }

            public override Trigger TriggerBy()
            {
                return new Trigger().Replaced<TestStruct>();
            }

            public override bool IsCanExecute(IActor actor)
            {
                return true;
            }

            public override void Execute(HashSet<IActor> actors)
            {
                foreach (var actor in actors)
                {
                    actor.ReplaceProp(new TestStruct { Value = 2 });
                }
            }
        }

        private class TestRemovedAbility : AReactiveAbility
        {
            public TestRemovedAbility(World world) : base(world)
            {
            }

            public override Trigger TriggerBy()
            {
                return new Trigger().Removed<TestStruct>();
            }

            public override bool IsCanExecute(IActor actor)
            {
                return true;
            }

            public override void Execute(HashSet<IActor> actors)
            {
                foreach (var actor in actors)
                {
                    actor.AddProp(new TestStruct { Value = 1 });
                }
            }
        }
    }
}