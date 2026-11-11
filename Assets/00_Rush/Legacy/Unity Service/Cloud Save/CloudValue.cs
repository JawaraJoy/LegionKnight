using System;
using UnityEngine;

namespace LegionKnight
{
    [Serializable]
    public class CloudValue<T>
    {
        public T value;
        public long lastUpdateUnix;
        public long ttlSeconds; // 0 = never expire
    }
}
