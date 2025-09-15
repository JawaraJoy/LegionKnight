using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public interface IAnoy
    {
        AnoyDefinition AnoyDefinition { get; }
        void StopAnoy();
        void OnInteruptUpdateInvoke(int interuptCount);
    }
}
