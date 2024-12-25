using NUnit.Framework;

namespace AxeEngine.Tests
{
    [TestFixture]
    public class FilterTests
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
        }

        [Test]
        public void Filter_WithOneProperty_ContainsActorWithProperty_True()
        {
            var actor = _world.CreateActor();
            actor.AddProp<EmptyProperty>();
            var filterOptions = new FilterOption().With<EmptyProperty>();
            var filter = _world.GetFilter(ref filterOptions);
            var actors = filter.Get();
            Assert.IsTrue(actors.Contains(actor));
        }

        [Test]
        public void Filter_WithTwoProperty_ContainsActorWithOneProperty_False()
        {
            var actor = _world.CreateActor();
            actor.AddProp<EmptyProperty>();
            var filterOptions = new FilterOption().With<EmptyProperty, IntProperty>();
            var filter = _world.GetFilter(ref filterOptions);
            var actors = filter.Get();
            Assert.IsFalse(actors.Contains(actor));
        }

        [Test]
        public void Filter_WithTwoProperty_ContainsActorWithTwoProperty_True()
        {
            var actor = _world.CreateActor();
            actor.AddProp<EmptyProperty>().AddProp<IntProperty>();
            var filterOptions = new FilterOption().With<EmptyProperty, IntProperty>();
            var filter = _world.GetFilter(ref filterOptions);
            var actors = filter.Get();
            Assert.IsTrue(actors.Contains(actor));
        }

        [Test]
        public void Filter_WithOneProperty_ContainsActorWithTwoProperty_True()
        {
            var actor = _world.CreateActor();
            actor.AddProp<EmptyProperty>().AddProp<IntProperty>();
            var filterOptions = new FilterOption().With<EmptyProperty>();
            var filter = _world.GetFilter(ref filterOptions);
            var actors = filter.Get();
            Assert.IsTrue(actors.Contains(actor));
        }

        [Test]
        public void Filter_WithAnyProperty_ContainsActorWithAnyProperty_True()
        {
            var actor = _world.CreateActor();
            actor.AddProp<EmptyProperty>().AddProp<IntProperty>().AddProp<BoolProperty>();
            var filterOptions = new FilterOption().WithAny<BoolProperty>();
            var filter = _world.GetFilter(ref filterOptions);
            var actors = filter.Get();
            Assert.IsTrue(actors.Contains(actor));
        }

        [Test]
        public void Filter_WithAnyProperty_ContainsActorWithAnyProperty_False()
        {
            var actor = _world.CreateActor();
            actor.AddProp<EmptyProperty>().AddProp<IntProperty>();
            var filterOptions = new FilterOption().WithAny<BoolProperty>();
            var filter = _world.GetFilter(ref filterOptions);
            var actors = filter.Get();
            Assert.IsFalse(actors.Contains(actor));
        }

        [Test]
        public void Filter_WithAnyAndWithProperty_ContainsActorWithAnyProperty_False()
        {
            var actor = _world.CreateActor();
            actor.AddProp<EmptyProperty>().AddProp<IntProperty>();
            var filterOptions = new FilterOption().With<IntProperty>().WithAny<BoolProperty>();
            var filter = _world.GetFilter(ref filterOptions);
            var actors = filter.Get();
            Assert.IsFalse(actors.Contains(actor));
        }

        [Test]
        public void Filter_WithoutOneProperty_ContainsActorWithOneProperty_True()
        {
            var actor = _world.CreateActor();
            actor.AddProp<EmptyProperty>().AddProp<IntProperty>();
            var filterOptions = new FilterOption().Without<BoolProperty>();
            var filter = _world.GetFilter(ref filterOptions);
            var actors = filter.Get();
            Assert.IsTrue(actors.Contains(actor));
        }

        [Test]
        public void Filter_WithoutOneProperty_ContainsActorWithOneProperty_False()
        {
            var actor = _world.CreateActor();
            actor.AddProp<EmptyProperty>().AddProp<IntProperty>();
            var filterOptions = new FilterOption().Without<IntProperty>();
            var filter = _world.GetFilter(ref filterOptions);
            var actors = filter.Get();
            Assert.IsFalse(actors.Contains(actor));
        }

        private struct EmptyProperty
        {
        }

        private struct IntProperty
        {
            public int Value;
        }

        private struct BoolProperty
        {
            public int Value;
        }
    }
}