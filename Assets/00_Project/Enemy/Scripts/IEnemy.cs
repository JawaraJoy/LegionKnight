using UnityEngine;

namespace LegionKnight
{
    public interface IEnemy 
    {
        void Register();
        void UnRegister();

        int DynamicLevel { get; }
    }
}
