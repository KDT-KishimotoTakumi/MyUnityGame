using System.Collections.Generic;

namespace MyUnityGame
{
    /// <summary>Collected-item bookkeeping for BallCollect: count, duplicate guard, clear check and reset.</summary>
    public sealed class CollectionState
    {
        private readonly HashSet<int> collectedIds = new HashSet<int>();

        public CollectionState(int totalCount = 5)
        {
            TotalCount = totalCount;
        }

        public int TotalCount { get; }

        public int CollectedCount => collectedIds.Count;

        public bool IsCleared => CollectedCount == TotalCount;

        /// <summary>Counts the item once. Out-of-range or already collected ids are ignored and return false.</summary>
        public bool TryCollect(int itemId)
        {
            if (itemId < 0 || itemId >= TotalCount)
            {
                return false;
            }

            return collectedIds.Add(itemId);
        }

        public void Reset()
        {
            collectedIds.Clear();
        }
    }
}
