using NUnit.Framework;
using UnityEngine.TestTools;

namespace AxeEngine.Tests
{
    [TestFixture]
    internal class WorldTests
    {
        private World _world;

        [SetUp]
        public void Setup()
        {
            _world = new World();
        }

        [TearDown]
        public void TearDown()
        {
            _world.Dispose();
            LogAssert.ignoreFailingMessages = false;
        }

        [Test]
        public void CreateActorTest_FindWithoutProperty_False()
        {
            var actor = _world.CreateActor();
            Assert.NotNull(actor);

            var filterOptions = FilterOption.Empty.With<TestProperty>();
            var filter = _world.GetFilter(ref filterOptions);
            var actors = filter.Get();
            var isContains = actors.Contains(actor);
            Assert.IsFalse(isContains);
        }

        [Test]
        public void CreateActorTest_FindWithProperty_True()
        {
            var actor = _world.CreateActor();
            TestProperty prop = default;
            actor.AddProp(ref prop);
            Assert.NotNull(actor);

            var filterOptions = FilterOption.Empty.With<TestProperty>();
            var filter = _world.GetFilter(ref filterOptions);
            var actors = filter.Get();
            var isContains = actors.Contains(actor);
            Assert.IsTrue(isContains);
        }

        [Test]
        public void CreateActorTestAndDestroyBeforeCreatingFilter_FindWithProperty_False()
        {
            var actor = _world.CreateActor();
            TestProperty prop = default;
            actor.AddProp(ref prop);
            Assert.NotNull(actor);

            _world.DestroyActor(actor);

            var filterOptions = FilterOption.Empty.With<TestProperty>();
            var filter = _world.GetFilter(ref filterOptions);
            var actors = filter.Get();
            var isContains = actors.Contains(actor);
            Assert.IsFalse(isContains);
        }

        [Test]
        public void CreateActorTestAndDestroyAfterCreatingFilter_FindWithProperty_False()
        {
            var actor = _world.CreateActor();
            TestProperty prop = default;
            actor.AddProp(ref prop);
            Assert.NotNull(actor);


            var filterOptions = FilterOption.Empty.With<TestProperty>();
            var filter = _world.GetFilter(ref filterOptions);

            _world.DestroyActor(actor);

            var actors = filter.Get();
            var isContains = actors.Contains(actor);
            Assert.IsFalse(isContains);
        }

        [Test]
        public void CreateActorTestWithTwoProperties_AddedOnlyOne_True()
        {
            var actor = _world.CreateActor();
            var prop1 = new TestIntProperty() { Value = 1 };
            actor.AddProp(ref prop1);
            var prop2 = new TestIntProperty() { Value = 2 };
            actor.AddProp(ref prop2);
            Assert.NotNull(actor);

            var prop = actor.GetProp<TestIntProperty>();
            Assert.NotNull(prop);
            Assert.AreEqual(2, prop.Value);
        }

        [Test]
        public void CreateActorTest_ValueChanged_True()
        {
            var actor = _world.CreateActor();
            var prop = new TestIntProperty() { Value = 1 };
            actor.AddProp(ref prop);
            prop.Value = 2;
            actor.ReplaceProp(ref prop);

            var changedProp = actor.GetProp<TestIntProperty>();
            Assert.NotNull(changedProp);
            Assert.AreEqual(2, changedProp.Value);
        }

        [Test]
        public void CreateActorTest_GetPropWithoutProperty_ReturnDefault()
        {
            LogAssert.ignoreFailingMessages = true;
            var actor = _world.CreateActor();
            var prop = actor.GetProp<TestProperty>();
            Assert.AreEqual(default(TestProperty), prop);
        }

        [Test]
        public void RemoveActorTest_FindWithProperty_False()
        {
            var actor = _world.CreateActor();
            actor.AddProp<TestProperty>();
            var filterOptions = FilterOption.Empty.With<TestProperty>();
            var filter = _world.GetFilter(ref filterOptions);
            var actors = filter.Get();
            Assert.IsTrue(actors.Count == 1);

            _world.DestroyActor(actor);
            Assert.IsFalse(actors.Count == 1);
        }

        [Test]
        public void ReplaceProp_From2To1_True()
        {
            var actor = _world.CreateActor();
            actor.AddProp(new TestIntProperty() { Value = 2 });
            actor.ReplaceProp(new TestIntProperty() { Value = 1 });
            var prop = actor.GetProp<TestIntProperty>();
            Assert.AreEqual(1, prop.Value);
        }

        [Test]
        public void RemoveProp_GetActor_False()
        {
            var actor = _world.CreateActor();
            actor.AddProp<TestProperty>();
            var filterOptions = FilterOption.Empty.With<TestProperty>();
            var filter = _world.GetFilter(ref filterOptions);
            var actors = filter.Get();
            Assert.IsTrue(actors.Count == 1);

            actor.RemoveProp<TestProperty>();
            Assert.IsFalse(actors.Count == 1);
        }

        [Test]
        public void CreateActor_DestroyActor_IsAliveAndIdIsEqual()
        {
            var actor = _world.CreateActor();
            Assert.IsTrue(actor.IsAlive);
            Assert.IsTrue(actor.Id == 1);

            _world.DestroyActor(actor);
            Assert.IsFalse(actor.IsAlive);

            var actor2 = _world.CreateActor();
            Assert.IsTrue(actor2.IsAlive);
            Assert.IsTrue(actor2.Id == 1);
        }

        private struct TestProperty
        {
        }

        private struct TestIntProperty
        {
            public int Value;
        }
    }
}