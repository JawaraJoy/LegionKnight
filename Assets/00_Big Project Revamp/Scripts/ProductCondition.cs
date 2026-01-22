using UnityEngine;

namespace LegionKnight
{
    public enum ProductCondition
    {
        Locked = 0,
        NewUnlocked = 1,
        NoticeUnlocked = 2,
    }
    public interface IProductHasCondition
    {
        void ChangeCondition(ProductCondition condition);
        ProductCondition Condition { get; }
    }
}
