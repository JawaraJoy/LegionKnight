using LegionKnight;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class AvatarSpine : View, IUnitExtension, IAvatarSpine
    {
        private SkeletonDataAsset m_SkeletonDataAsset;

        [SerializeField]
        private SkeletonAnimation m_SkeletonAnimation;

        [SerializeField]
        private ClipEventField[] m_Events;
        [SerializeField]
        private SpineClipTrigger[] m_SpineClipTriggers;

        [SerializeField]
        private UnityEvent<ModuleContext> m_OnInitialized;
    
        [SerializeField]
        private UnityEvent<AnimationClipConfig> m_OnClipStart = new();

        [SerializeField]
        private UnityEvent<AnimationClipConfig> m_OnClipDone = new();

        private AnimationClipConfig m_CurrentClip;
        private bool m_IsPaused;
        private float m_PreviousTimeScale = 1f;

        public SkeletonDataAsset SekeletonDataAsset => m_SkeletonDataAsset;
        public SkeletonAnimation SkeletonAnimation => m_SkeletonAnimation;

        private ModuleContext m_ModuleContext;
        public IModuleContext ModuleContext => m_ModuleContext;

        private SpineClipTrigger GetSpineClipTrigger(string animationId)
        {
            SpineClipTrigger spineClipTrigger = m_SpineClipTriggers.FirstOrDefault(x => x.ClipConfig.BaseInfo.Id == animationId);
            if (spineClipTrigger == null)
            {
                return null;
            }
            return spineClipTrigger;
        }
        private bool HasSpineClipTriggerInternal(string animationId, out SpineClipTrigger clipTrigger)
        {
            clipTrigger = GetSpineClipTrigger(animationId);
            return (clipTrigger != null);
        }

        private void OnDestroy()
        {
            var state = m_SkeletonAnimation.AnimationState;
            if (state != null)
            {
                state.Event -= OnSpineEvent;
                state.Complete -= OnSpineComplete;
            }
        }

        private void OnSpineEvent(TrackEntry entry, Spine.Event e)
        {
            if (HasSpineEventInternal(e.Data.Name, out var ev))
            {
                ev.OnTriggeredInvoke();
            }
        }

        private void OnSpineComplete(TrackEntry entry)
        {
            // 🔑 samakan dengan UI → hanya track 0 yang trigger selesai
            if (entry.TrackIndex != 0) return;
            if (m_CurrentClip == null) return;

            OnAnimationDoneInvoke(m_CurrentClip);

            var next = m_CurrentClip.NextAnimation;
            m_CurrentClip = null;

            if (next != null)
            {
                PlayClipInternal(next);
            }
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (m_ModuleContext.Unit.HasBind(out SFXController sfxController))
            {
                sfxController.PlayEntranceSFX();
            }
        }

        [ContextMenu("Initialize Spine")]
        public void Init(Unit unit)
        {
            if (unit.Config is IHasSekeletonAsset hasSkeletonAsset)
            {
                m_SkeletonDataAsset = hasSkeletonAsset.SkeletonDataAsset;
                m_SkeletonAnimation.skeletonDataAsset = m_SkeletonDataAsset;

                m_SkeletonAnimation.Initialize(true);

                var state = m_SkeletonAnimation.AnimationState;

                // 🔁 rebind event (penting kalau skeleton diganti)
                state.Event -= OnSpineEvent;
                state.Complete -= OnSpineComplete;

                state.Event += OnSpineEvent;
                state.Complete += OnSpineComplete;

                m_Content.transform.localScale = unit.Config.UnitScale;
            }

            m_ModuleContext = new ModuleContext(unit, gameObject);
            m_OnInitialized?.Invoke(m_ModuleContext);
            ShowInternal();
        }

        public void FlipX(bool left)
        {
            if (m_SkeletonAnimation.Skeleton == null) return;

            m_SkeletonAnimation.Skeleton.ScaleX = left ? -1 : 1;
        }

        public void PlayClip(AnimationClipConfig clipConfig)
        {
            PlayClipInternal(clipConfig);
        }

        private void PlayClipInternal(AnimationClipConfig clipConfig)
        {
            if (clipConfig == null) return;
            if (m_SkeletonAnimation == null) return;
            if (clipConfig.BaseInfo == null) return;

            var animName = clipConfig.BaseInfo.Name;
            var state = m_SkeletonAnimation.AnimationState;
            if (state == null) return;

            bool hasCurrent = state.GetCurrent(clipConfig.Track) != null;

            TrackEntry entry = hasCurrent
                ? state.AddAnimation(clipConfig.Track, animName, clipConfig.Loop, clipConfig.Delay)
                : state.SetAnimation(clipConfig.Track, animName, clipConfig.Loop);

            entry.TimeScale = clipConfig.SpeedRate;

            m_CurrentClip = clipConfig;
            m_SkeletonAnimation.loop = clipConfig.Loop;
            m_OnClipStart?.Invoke(clipConfig);
            SpineClipOnStartInvoke(clipConfig);

            Debug.Log($"[Spine] Queue Animation: {animName}");
        }

        public void PlayClipInterrupt(AnimationClipConfig clipConfig)
        {
            PlayClipInterruptInternal(clipConfig);
        }

        private void PlayClipInterruptInternal(AnimationClipConfig clipConfig)
        {
            if (clipConfig == null) return;

            var state = m_SkeletonAnimation.AnimationState;
            if (state == null) return;

            var entry = state.SetAnimation(
                clipConfig.Track,
                clipConfig.BaseInfo.Name,
                clipConfig.Loop
            );

            entry.TimeScale = clipConfig.SpeedRate;

            m_CurrentClip = clipConfig;
        }

        public void QueueClip(AnimationClipConfig clipConfig)
        {
            if (clipConfig == null) return;

            var state = m_SkeletonAnimation.AnimationState;
            if (state == null) return;

            var entry = state.AddAnimation(
                clipConfig.Track,
                clipConfig.BaseInfo.Name,
                clipConfig.Loop,
                clipConfig.Delay
            );

            entry.TimeScale = clipConfig.SpeedRate;
        }

        public void SetSkin(string skinName)
        {
            if (m_SkeletonAnimation == null || m_SkeletonAnimation.Skeleton == null) return;

            var skeleton = m_SkeletonAnimation.Skeleton;
            var skin = skeleton.Data.FindSkin(skinName);

            if (skin != null)
            {
                skeleton.SetSkin(skin);
                skeleton.SetupPoseSlots();
                m_SkeletonAnimation.Update(0f);

                Debug.Log($"[Spine] Skin changed to: {skinName}");
            }
            else
            {
                Debug.LogWarning($"[Spine] Skin '{skinName}' not found.");
            }
        }
        public void Pause()
        {
            if (m_IsPaused) return;
            if (m_SkeletonAnimation.AnimationState == null) return;

            var state = m_SkeletonAnimation.AnimationState;

            m_PreviousTimeScale = state.TimeScale;
            state.TimeScale = 0f;

            m_IsPaused = true;
        }

        public void Resume()
        {
            if (!m_IsPaused) return;
            if (m_SkeletonAnimation.AnimationState == null) return;

            var state = m_SkeletonAnimation.AnimationState;

            state.TimeScale = m_PreviousTimeScale <= 0f ? 1f : m_PreviousTimeScale;

            m_IsPaused = false;
        }
        private ClipEventField GetClipEventInternal(string eventName)
        {
            foreach (var e in m_Events)
            {
                if (e != null &&
                    e.EventConfig != null &&
                    e.EventConfig.BaseInfo != null &&
                    e.EventConfig.BaseInfo.Name == eventName)
                {
                    return e;
                }
            }
            return null;
        }
        private bool HasSpineEventInternal(string eventName, out ClipEventField ev)
        {
            ev = GetClipEventInternal(eventName);
            return ev != null;
        }

        private void OnAnimationDoneInvoke(AnimationClipConfig clipConfig)
        {
            Debug.Log($"[Spine] Animation Done: {clipConfig.BaseInfo.Name}");
            m_OnClipDone?.Invoke(clipConfig);
            SpineClipOnDoneInvoke(clipConfig);
        }

        private void SpineClipOnStartInvoke(AnimationClipConfig clipConfig)
        {
            if (HasSpineClipTriggerInternal(clipConfig.BaseInfo.Id, out SpineClipTrigger clipTrigger))
            {
                clipTrigger.OnStartInvoke(clipConfig);
            }
        }
        private void SpineClipOnDoneInvoke(AnimationClipConfig clipConfig)
        {
            if (HasSpineClipTriggerInternal(clipConfig.BaseInfo.Id, out SpineClipTrigger clipTrigger))
            {
                clipTrigger.OnDoneInvoke(clipConfig);
            }
        }

        private void ChangeColorInternal(Color color)
        {
            m_SkeletonAnimation.Skeleton.SetColor(color);
        }

        private Coroutine m_FlashCoroutine;
        private Color m_OriginalColor = Color.white;

        public void FlashDamage()
        {
            if (m_SkeletonAnimation == null || m_SkeletonAnimation.Skeleton == null)
                return;

            if (m_FlashCoroutine != null)
            {
                RushGameManager.Instance.StopCoroutine(m_FlashCoroutine);

                // restore previous base color
                ChangeColorInternal(m_OriginalColor);
            }

            m_FlashCoroutine = RushGameManager.Instance.StartCoroutine(
                DamageColoring(Color.red, 0.1f)
            );
        }

        private IEnumerator DamageColoring(Color damageColor, float duration)
        {
            var skeleton = m_SkeletonAnimation.Skeleton;

            // cache current color
            m_OriginalColor = skeleton.GetColor();

            // apply flash
            ChangeColorInternal(damageColor);

            yield return new WaitForSeconds(duration);

            // restore
            ChangeColorInternal(m_OriginalColor);

            m_FlashCoroutine = null;
        }
    }
}
