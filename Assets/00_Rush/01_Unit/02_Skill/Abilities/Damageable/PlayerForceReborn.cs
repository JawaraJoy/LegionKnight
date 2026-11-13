using System.Collections;
using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public class PlayerForceReborn : MonoBehaviour, IReseter
    {
        [SerializeField, MMReadOnly] private Unit m_PlayerHerounit;
        [SerializeField] private ParticleSystem m_ReviveBurst;
        private const float c_HeightAbovePlatform = 3f;
        private const float c_ResumeDelay = 1f;
        private bool m_CanForceReborn;
        private Coroutine m_Revival;

        public bool CanForceReborn => m_CanForceReborn;

        private void Start()
        {
            m_PlayerHerounit = RushPlayer.Instance.Unit;
            SetCanForceRebornInternal(true);
        }

        public void SetCanForceReborn(bool set) => SetCanForceRebornInternal(set);
        private void SetCanForceRebornInternal(bool set) => m_CanForceReborn = set;

        public void ForcingReborn(float delay) => StartRevivalInternal(delay, null);

        public void ReviveAfterReward(float delay, System.Action onRevived) =>
            StartRevivalInternal(delay, onRevived);

        private void StartRevivalInternal(float delay, System.Action onRevived)
        {
            if (m_Revival == null) m_Revival = StartCoroutine(ForcingRebornInternal(delay, onRevived));
        }

        private IEnumerator ForcingRebornInternal(float delay, System.Action onRevived)
        {
            yield return new WaitForSeconds(delay);
            var stage = RushGameManager.Instance.StageManager;
            float resumeAt = Time.time + c_ResumeDelay;
            if (m_PlayerHerounit.HasBind(out Damageable damageable))
            {
                var position = stage.PlatformHandler.LastContactPoint + Vector2.up * c_HeightAbovePlatform;
                RushPlayer.Instance.SetPosition(position);
                yield return damageable.RebornForRewardedRevive(m_PlayerHerounit.Config.RebornConfig);
                if (m_ReviveBurst != null) m_ReviveBurst.Play(true);
                onRevived?.Invoke();
            }
            while (Time.time < resumeAt) yield return null;
            stage.Resume();
            SetCanForceRebornInternal(false);
            m_Revival = null;
        }

        public void ResetProgression()
        {
            StopRevivalInternal();
            SetCanForceRebornInternal(true);
        }

        private void OnDisable() => StopRevivalInternal();

        private void StopRevivalInternal()
        {
            if (m_Revival != null) StopCoroutine(m_Revival);
            m_Revival = null;
            if (m_ReviveBurst != null)
                m_ReviveBurst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    public partial class RushPlayer
    {
        [SerializeField] private PlayerForceReborn m_Reborn;
        public PlayerForceReborn Reborn => m_Reborn;
    }
}
