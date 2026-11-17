using NUnit.Framework;

namespace MyUnityGame.Tests
{
    public class CollectionStateTests
    {
        private CollectionState state;

        [SetUp]
        public void SetUp()
        {
            state = new CollectionState(5);
        }

        [Test]
        public void Initial_CountIsZero_AndNotCleared()
        {
            Assert.AreEqual(0, state.CollectedCount);
            Assert.IsFalse(state.IsCleared);
        }

        [Test]
        public void TryCollect_NewId_ReturnsTrue_AndCountsOne()
        {
            Assert.IsTrue(state.TryCollect(0));
            Assert.AreEqual(1, state.CollectedCount);
        }

        [Test]
        public void TryCollect_SameIdTwice_CountsOnce()
        {
            state.TryCollect(2);
            Assert.IsFalse(state.TryCollect(2));
            Assert.AreEqual(1, state.CollectedCount);
        }

        [Test]
        public void TryCollect_OutOfRangeId_IsIgnored()
        {
            Assert.IsFalse(state.TryCollect(-1));
            Assert.IsFalse(state.TryCollect(5));
            Assert.AreEqual(0, state.CollectedCount);
        }

        [Test]
        public void FourCollected_IsNotCleared()
        {
            for (int id = 0; id < 4; id++) state.TryCollect(id);
            Assert.AreEqual(4, state.CollectedCount);
            Assert.IsFalse(state.IsCleared);
        }

        [Test]
        public void AllFiveCollected_IsCleared()
        {
            for (int id = 0; id < 5; id++) state.TryCollect(id);
            Assert.AreEqual(5, state.CollectedCount);
            Assert.IsTrue(state.IsCleared);
        }

        [Test]
        public void Reset_RestoresInitialState_AndAllowsCollectingAgain()
        {
            for (int id = 0; id < 5; id++) state.TryCollect(id);
            state.Reset();
            Assert.AreEqual(0, state.CollectedCount);
            Assert.IsFalse(state.IsCleared);
            Assert.IsTrue(state.TryCollect(0));
            Assert.AreEqual(1, state.CollectedCount);
        }
    }
}
