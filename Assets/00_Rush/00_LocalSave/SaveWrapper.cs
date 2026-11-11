using System;
using UnityEngine;

namespace Rush
{
    [Serializable]
    public class SaveWrapper<T>
    {
        public T value;
        public long lastUpdateUnix;
        public long ttlSeconds;
        public int version;
        public string hash; // optional anti tamper
    }
}
