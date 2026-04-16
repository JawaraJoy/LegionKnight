using System.Collections.Generic;

namespace Rush
{
    public class GachaDrawResult
    {
        private readonly List<GachaCollectableConfig> m_Items = new();
        private bool m_WasPityTriggered;
        private bool m_WasFirstDraw;

        public IReadOnlyList<GachaCollectableConfig> Items => m_Items;
        public bool WasPityTriggered => m_WasPityTriggered;
        public bool WasFirstDraw => m_WasFirstDraw;

        internal void AddItem(GachaCollectableConfig item) => m_Items.Add(item);
        internal void SetPityTriggered(bool value) => m_WasPityTriggered = value;
        internal void SetFirstDraw(bool value) => m_WasFirstDraw = value;
    }
}