using LegionKnight;
using UnityEngine;

namespace Rush
{
    public sealed class PlayReviewPrompt : MonoBehaviour
    {
        private const int c_MinimumLevel = 3;
        private const string c_Bridge = "com.jawarajoy.aetherjump.review.PlayReview";
        private bool m_LevelUpShown;
        private int m_LastRequestedLevel;

        public void OnLevelUpShown() => m_LevelUpShown = true;

        public void OnLevelUpDismissed()
        {
            // Hide can also be sent while panels are being initialized or reset.
            if (!m_LevelUpShown) return;
            m_LevelUpShown = false;
#if UNITY_ANDROID && !UNITY_EDITOR
            int level = Player.Instance.GetPlayerLevel();
            if (level < c_MinimumLevel || level == m_LastRequestedLevel) return;
            m_LastRequestedLevel = level;
            try
            {
                using var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                using var bridge = new AndroidJavaClass(c_Bridge);
                bridge.CallStatic("request", activity);
            }
            catch (AndroidJavaException exception)
            {
                Debug.LogWarning("[Play Review] Unavailable: " + exception.Message);
            }
#endif
        }

        private void OnDisable() => m_LevelUpShown = false;
    }
}
