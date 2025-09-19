using UnityEngine;

namespace LegionKnight
{
    public class DailyRewardManagerAgent : MonoBehaviour
    {
        public void Refresh()
        {
            DailyRewardManager manager = GameManager.Instance.DailyRewardManager;
            if (manager != null)
            {
                manager.Refresh();
            }
            else
            {
                Debug.LogWarning("DailyRewardManagerAgent: DailyRewardManager instance is null.");
            }
        }
    }
}
