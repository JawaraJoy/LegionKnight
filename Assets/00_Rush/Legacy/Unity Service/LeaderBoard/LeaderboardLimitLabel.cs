using System.Collections;
using LegionKnight;
using TMPro;
using UnityEngine;

namespace Rush
{
    public sealed class LeaderboardLimitLabel : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_Label;
        private const float c_RefreshInterval = 0.25f;

        private void OnEnable()
        {
            if (m_Label != null) StartCoroutine(ShowLimitWhenReadyInternal());
        }

        private IEnumerator ShowLimitWhenReadyInternal()
        {
            m_Label.text = "TOP RANKS";
            var service = UnityService.Instance;
            var delay = new WaitForSecondsRealtime(c_RefreshInterval);
            while (service != null && !service.IsLeaderboardReady) yield return delay;
            if (service != null) m_Label.text = $"TOP {service.MaxRankToDisplay}";
        }
    }
}
