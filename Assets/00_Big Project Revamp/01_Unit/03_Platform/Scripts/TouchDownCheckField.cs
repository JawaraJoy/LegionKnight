using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    [System.Serializable]
    public class TouchDownCheckField : IReseter
    {
        [SerializeField, MMReadOnly]
        private bool m_IsStayPerfect;
        [SerializeField, MMReadOnly]
        private int m_StayPerfectCount;
        [SerializeField]
        private UnityEvent<bool, ISkillContext> m_OnTouchDown;
        [SerializeField]
        private UnityEvent<ISkillContext> m_OnNormalTouchDown;
        [SerializeField]
        private UnityEvent<ISkillContext> m_OnPerfectTouchDown;
        [SerializeField]
        private UnityEvent<int> m_OnStayPerfectCountChange;
        public bool IsStayPerfect => m_IsStayPerfect;
        public int StayPerfectCount => m_StayPerfectCount;
        private void SetIsStayPerfectInternal(bool value, ISkillContext context)
        {
            m_IsStayPerfect = value;
            if (value)
            {
                AddStayPerfectCountInternal(1);
                OnPerfectTouchDownInvoke(context);
            }
            else
            {
                SetStayPerfectCountInternal(0);
                OnNormalTouchDownInvoke(context);
            }
            m_OnTouchDown.Invoke(value, context);
        }
        public void SetIsStayPerfect(bool value, ISkillContext context)
        {
            SetIsStayPerfectInternal(value, context);
        }
        private void AddStayPerfectCountInternal(int add)
        {
            m_StayPerfectCount += add;
        }
        private void SetStayPerfectCountInternal(int value)
        {
            m_StayPerfectCount = value;
        }
        private void OnNormalTouchDownInvoke(ISkillContext context)
        {
            m_OnNormalTouchDown?.Invoke(context);

        }
        private void OnPerfectTouchDownInvoke(ISkillContext context)
        {
            m_OnPerfectTouchDown?.Invoke(context);
            m_OnStayPerfectCountChange?.Invoke(m_StayPerfectCount);
        }

        /// <summary>
        /// Daftarkan callback yang dipanggil setiap kali StayPerfectCount berubah.
        /// Digunakan oleh PlatformHandler untuk mengecek apakah boost threshold tercapai.
        /// </summary>
        public void RegisterBoostCheck(UnityAction<int> callback)
        {
            m_OnStayPerfectCountChange.AddListener(callback);
        }

        /// <summary>
        /// Hapus semua boost check listener. Panggil saat platform dikembalikan ke pool.
        /// </summary>
        public void ClearBoostCheck()
        {
            m_OnStayPerfectCountChange.RemoveAllListeners();
        }

        /// <summary>
        /// Daftarkan callback yang dipanggil setiap kali ada PERFECT landing (bukan normal).
        /// Digunakan oleh PlatformHandler global untuk mengakumulasi count lintas semua platform.
        /// </summary>
        public void RegisterPerfectLandingCallback(UnityAction<ISkillContext> callback)
        {
            m_OnPerfectTouchDown.AddListener(callback);
        }
        public void UnregisterPerfectLandingCallback(UnityAction<ISkillContext> callback)
        {
            m_OnPerfectTouchDown.RemoveListener(callback);
        }

        /// <summary>
        /// Daftarkan callback yang dipanggil setiap kali ada NORMAL landing.
        /// Digunakan untuk reset perfect streak count di PlatformHandler.
        /// </summary>
        public void RegisterNormalLandingCallback(UnityEngine.Events.UnityAction<ISkillContext> callback)
        {
            m_OnNormalTouchDown.AddListener(callback);
        }
        public void UnregisterNormalLandingCallback(UnityEngine.Events.UnityAction<ISkillContext> callback)
        {
            m_OnNormalTouchDown.RemoveListener(callback);
        }

        /// <summary>
        /// Reset state perfect count dan streak. Dipanggil saat platform di-spawn ulang.
        /// </summary>
        public void ResetProgression()
        {
            SetIsStayPerfectInternal(false, null);
            SetStayPerfectCountInternal(0);
        }
    }
}