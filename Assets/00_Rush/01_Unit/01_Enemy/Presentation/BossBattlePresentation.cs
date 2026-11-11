using Spine;
using Spine.Unity;
using UnityEngine;

namespace Rush
{
    [DisallowMultipleComponent]
    public sealed class BossBattlePresentation : MonoBehaviour
    {
        private const float c_EntranceSeconds = 0.5f;
        private const float c_MinDeathSeconds = 0.85f;
        private const float c_DissolveSeconds = 0.65f;
        private const float c_CallbackGrace = 0.12f;
        [SerializeField] private AudioSource m_DeathSound;
        [SerializeField] private BossDissolve m_Dissolve;

        private Unit m_Unit;
        private EnemyWaveHandler m_Wave;
        private AvatarSpine m_Avatar;
        private Spine.AnimationState m_State;
        private TrackEntry m_DeathEntry;
        private AudioSource m_Tail;
        private bool m_Dying, m_ClipComplete, m_Settled;
        private float m_Elapsed, m_DeathDuration;

        public void Enter(Unit unit, EnemyWaveHandler wave)
        {
            if (m_Unit != null || !gameObject.activeInHierarchy) return;
            m_Unit = unit;
            m_Wave = wave;
            if (!unit.HasBind(out m_Avatar)) return;
            m_Avatar.Show();
            var clip = unit.Config.Entrance;
            if (!CanPlay(clip)) return;
            m_Avatar.PlayClipInterrupt(clip);
            var entry = m_Avatar.SkeletonAnimation.AnimationState.GetTrack(clip.Track);
            if (!clip.Loop) entry.TimeScale = Mathf.Max(entry.TimeScale, entry.Animation.Duration / c_EntranceSeconds);
        }

        public void BeginDeath()
        {
            if (m_Unit == null || m_Dying || !gameObject.activeInHierarchy) return;
            m_Dying = true;
            m_Elapsed = 0f;
            m_DeathDuration = c_MinDeathSeconds;
            var clip = m_Unit.Config.Death;
            if (CanPlay(clip))
            {
                m_Avatar.PlayClipInterrupt(clip);
                m_State = m_Avatar.SkeletonAnimation.AnimationState;
                m_DeathEntry = m_State.GetTrack(clip.Track);
                m_DeathDuration = Mathf.Max(c_MinDeathSeconds, m_DeathEntry.Animation.Duration / m_DeathEntry.TimeScale);
                m_State.Complete += HandleComplete;
            }
            if (m_Avatar != null) m_Dissolve.Begin(m_Avatar.SkeletonAnimation);
            if (m_DeathSound != null && m_DeathSound.clip != null)
            {
                if (m_Tail == null) m_Tail = Instantiate(m_DeathSound, m_Wave.transform);
                m_Tail.transform.position = transform.position;
                m_Tail.PlayOneShot(m_DeathSound.clip);
            }
        }

        private bool CanPlay(AnimationClipConfig clip)
        {
            return clip != null && clip.BaseInfo != null && clip.SpeedRate > 0f &&
                m_Avatar != null && m_Avatar.SkeletonAnimation.Skeleton?.Data.FindAnimation(clip.BaseInfo.Name) != null;
        }

        private void HandleComplete(TrackEntry entry)
        {
            if (entry == m_DeathEntry) m_ClipComplete = true;
        }

        private void Update()
        {
            if (!m_Dying || m_Settled) return;
            float rate = m_Avatar == null ? 1f : m_Avatar.SkeletonAnimation.timeScale *
                (m_Avatar.SkeletonAnimation.AnimationState?.TimeScale ?? 1f);
            m_Elapsed += Time.deltaTime * rate;
            m_Dissolve.SetProgress(Mathf.InverseLerp(m_DeathDuration - c_DissolveSeconds, m_DeathDuration, m_Elapsed));
            if (m_Elapsed < m_DeathDuration) return;
            if (m_DeathEntry != null && !m_ClipComplete && m_Elapsed < m_DeathDuration + c_CallbackGrace) return;
            m_Settled = true;
            m_Wave.DespawnUnit(m_Unit);
        }

        private void OnDestroy()
        {
            if (m_Tail != null) Destroy(m_Tail.gameObject);
        }

        private void OnDisable()
        {
            if (m_State != null) m_State.Complete -= HandleComplete;
            if (m_Dying && !m_Settled && m_Tail != null) m_Tail.Stop();
            m_DeathEntry = null;
            m_State = null;
            m_Unit = null;
            m_Wave = null;
            m_Avatar = null;
            m_ClipComplete = m_Settled = m_Dying = false;
        }
    }
}
