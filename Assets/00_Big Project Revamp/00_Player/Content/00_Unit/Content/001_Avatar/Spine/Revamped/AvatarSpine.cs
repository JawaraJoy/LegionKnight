using LegionKnight;
using MoreMountains.Tools;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class AvatarSpine : View
    {
        [SerializeField, MMReadOnly]
        private UnitConfig m_Config;

        [SerializeField]
        private SkeletonAnimation m_SkeletonAnimation;

        [SerializeField]
        private AvatarContext m_AvatarContext;

        [SerializeField]
        private ClipEventField[] m_Events;
        [SerializeField]
        private UnityEvent<AvatarContext> m_OnInitialized;
        [SerializeField]
        private UnityEvent<AnimationClipConfig> m_OnClipStart = new();

        [SerializeField]
        private UnityEvent<AnimationClipConfig> m_OnClipDone = new();

        private AnimationClipConfig m_CurrentClip;

        public UnitConfig Config => m_Config;
        public SkeletonAnimation SkeletonAnimation => m_SkeletonAnimation;

        #region Unity

        private void Awake()
        {
            if (m_SkeletonAnimation != null)
            {
                m_SkeletonAnimation.AnimationState.Event += OnSpineEvent;
                m_SkeletonAnimation.AnimationState.Complete += OnSpineComplete;
            }
        }

        private void OnDestroy()
        {
            if (m_SkeletonAnimation != null)
            {
                m_SkeletonAnimation.AnimationState.Event -= OnSpineEvent;
                m_SkeletonAnimation.AnimationState.Complete -= OnSpineComplete;
            }
        }

        #endregion

        #region Spine Callbacks

        private void OnSpineEvent(TrackEntry entry, Spine.Event e)
        {
            if (HasSpineEventInternal(e.Data.Name, out var ev))
            {
                ev.OnTriggeredInvoke();
            }
        }

        private void OnSpineComplete(TrackEntry entry)
        {
            if (m_CurrentClip == null) return;

            OnAnimationDoneInvoke(m_CurrentClip);

            var next = m_CurrentClip.NextAnimation;
            m_CurrentClip = null;

            if (next != null)
            {
                PlayClipInternal(next); // ini juga akan masuk queue
            }
        }

        #endregion

        #region Public API

        [ContextMenu("Initialize Spine")]
        public void Init(Unit unit)
        {
            m_Config = unit.Config;
            m_SkeletonAnimation.skeletonDataAsset = unit.Config.SkeletonDataAsset;
            m_SkeletonAnimation.Initialize(true);
            m_AvatarContext = new AvatarContext(unit, this);
            m_OnInitialized?.Invoke(m_AvatarContext);
        }

        public void FlipX(bool left)
        {
            if (m_SkeletonAnimation != null)
            {
                m_SkeletonAnimation.initialFlipX = left;
            }
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

            bool hasCurrent = state.GetCurrent(0) != null;

            if (!hasCurrent)
            {
                state.SetAnimation(0, animName, clipConfig.Loop);
            }
            else
            {
                state.AddAnimation(0, animName, clipConfig.Loop, 0f);
            }

            m_CurrentClip = clipConfig;
            m_OnClipStart?.Invoke(clipConfig);

            Debug.Log($"[Spine] Queue Animation: {animName}");
        }
        public void PlayClipInterrupt(AnimationClipConfig clipConfig)
        {
            if (clipConfig == null) return;

            m_SkeletonAnimation.AnimationState.SetAnimation(
                0,
                clipConfig.BaseInfo.Name,
                clipConfig.Loop
            );

            m_CurrentClip = clipConfig;
        }
        public void QueueClip(AnimationClipConfig clipConfig)
        {
            if (clipConfig == null) return;

            m_SkeletonAnimation.AnimationState.AddAnimation(
                0,
                clipConfig.BaseInfo.Name,
                clipConfig.Loop,
                0f
            );
        }

        public void SetSkin(string skinName)
        {
            if (m_SkeletonAnimation == null || m_SkeletonAnimation.skeleton == null) return;

            var skeleton = m_SkeletonAnimation.skeleton;
            var skin = skeleton.Data.FindSkin(skinName);

            if (skin != null)
            {
                skeleton.SetSkin(skin);
                skeleton.SetupPoseSlots();
                m_SkeletonAnimation.LateUpdate();
                Debug.Log($"[Spine] Skin changed to: {skinName}");
            }
            else
            {
                Debug.LogWarning($"[Spine] Skin '{skinName}' not found.");
            }
        }

        #endregion

        #region Clip Events

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
        }

        #endregion
    }
}
