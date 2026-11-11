using UnityEngine;
using UnityEngine.Events;

namespace Rush
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
        UnityEvent<ProductCondition> OnConditionChanged { get; }
    }

    public static class ProducNoticeUtil
    {

    }
}
