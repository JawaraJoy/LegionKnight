using UnityEngine;

namespace Rush
{
    public partial interface IProgressable
    {
        int Level { get; }
        int MaxLevel { get; }
        void SetLevel(int level);
        void AddLevel(int amount);
    }
    
}
