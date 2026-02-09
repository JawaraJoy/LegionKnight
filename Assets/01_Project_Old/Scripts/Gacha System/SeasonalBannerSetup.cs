using UnityEngine;

namespace LegionKnight
{
    public class SeasonalBannerSetup : MonoBehaviour
    {
        [SerializeField] private BannerDefinition m_Banner;

        private CloudSave Cloud => UnityService.Instance.CloudSave;

        public void ActivateSeason()
        {
            if (!m_Banner.IsSeasonal)
                return;

            long ttl = m_Banner.SeasonDurationSeconds;

            Cloud.SaveData($"{m_Banner.Id}_total", 0, ttl);
            Cloud.SaveData($"{m_Banner.Id}_small", 0, ttl);
            Cloud.SaveData($"{m_Banner.Id}_first", false, ttl);

            Debug.Log($"Seasonal banner activated: {m_Banner.Id}");
        }
    }
}
