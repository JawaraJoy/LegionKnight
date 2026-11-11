using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public struct SummonJob
    {
        public Transform Origin;
        public int SlotIndex;
        public int SlotCount;
    }
}
