using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class SpineUI : UIView
    {
        [SerializeField]
        private SkeletonGraphic m_SkeletonGraphic;

        [SerializeField]
        private UnityEvent<SpineAnimDefinition> m_OnPlay;
        [SerializeField]
        private UnityEvent<SpineAnimDefinition> m_OnCompleted;
        [SerializeField]
        private UnityEvent<SpineAnimDefinition> m_OnPause;
        [SerializeField]
        private UnityEvent<SpineAnimDefinition> m_OnResume;

        [SerializeField]
        private SpineEvent[] m_EventAnimation;

        private SpineEvent GetSpineEvent(SpineAnimDefinition defi)
        {
            SpineEvent spineEvent = null;
            foreach (SpineEvent e in m_EventAnimation)
            {
                if (e.Definition.name == defi.name)
                {
                    spineEvent = e;
                }
            }
            return spineEvent;
        }
        private bool HasEvent(SpineAnimDefinition defi, out SpineEvent eve)
        {
            SpineEvent eveEvent = GetSpineEvent(defi);
            if (eveEvent != null)
            {
                eve = GetSpineEvent(defi);
            }
            else
            {
                eve = null;
            }
            return eveEvent != null;
        }
        public void SetSkeletonDataAsset(CharacterDefinition characterDefi)
        {
            SetSkeletonAssetInternal(characterDefi.SkeletonDataAsset);
        }
        public void SetSkeletonAsset(SkeletonDataAsset skeletonDataAsset)
        {
            
        }
        public void ShowNewSkeleton(SkeletonDataAsset skeletonDataAsset)
        {
            ShowInternal();
            SetSkeletonAssetInternal(skeletonDataAsset);
        }
        private void SetSkeletonAssetInternal(SkeletonDataAsset skeletonDataAsset)
        {
            m_SkeletonGraphic.skeletonDataAsset = skeletonDataAsset;
            m_SkeletonGraphic.Initialize(true);
        }
        public void Play(SpineAnimDefinition anim)
        {
            PlayInternal(anim);
        }

        private void PlayInternal(SpineAnimDefinition anim)
        {
            anim.PlayUI(m_SkeletonGraphic, () => OnCompleteInvoke(anim));
            OnPlayInvoke(anim);
            AddEventCallBack(anim);
        }
        public void PauseUI(SpineAnimDefinition anim)
        {
            anim.PauseUI(m_SkeletonGraphic);
            m_OnPause?.Invoke(anim);
        }
        public void ResumeUI(SpineAnimDefinition anim)
        {
            anim.ResumeUI(m_SkeletonGraphic);
            m_OnResume?.Invoke(anim);
        }

        private void AddEventCallBack(SpineAnimDefinition anim)
        {
            anim.AddEventCallBack(m_SkeletonGraphic, gameObject);
        }

        private void OnPlayInvoke(SpineAnimDefinition anim)
        {
            m_OnPlay?.Invoke(anim);
            if (HasEvent(anim, out SpineEvent spineEvent))
            {
                spineEvent.OnStart?.Invoke();
            }
        }
        private void OnCompleteInvoke(SpineAnimDefinition anim)
        {
            m_OnCompleted?.Invoke(anim);
            if (anim.NextAnim != null)
            {
                PlayInternal(anim.NextAnim);
            }
            if (HasEvent(anim, out SpineEvent spineEvent))
            {
                spineEvent.OnEnd?.Invoke();
            }
        }
    }
}
