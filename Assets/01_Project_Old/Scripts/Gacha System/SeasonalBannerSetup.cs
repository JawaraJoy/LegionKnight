using UnityEngine;

namespace LegionKnight
{
    public class SeasonalBannerSetup : MonoBehaviour
    {
        [SerializeField] private BannerConfiguration m_Banner;

        private LocalSave LocalSave => UnityService.Instance.LocalSave;

        public void ActivateSeason()
        {
            if (!m_Banner.IsSeasonal)
                return;

            long ttl = m_Banner.SeasonDurationSeconds;

            LocalSave.SaveData($"{m_Banner.BaseInfo.Id}_total", 0, ttl);
            LocalSave.SaveData($"{m_Banner.BaseInfo.Id}_small", 0, ttl);
            LocalSave.SaveData($"{m_Banner.BaseInfo.Id}_first", false, ttl);

            Debug.Log($"Seasonal banner activated: {m_Banner.BaseInfo.Id}");
        }
    }
}
