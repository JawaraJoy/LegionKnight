using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Rush;

namespace LegionKnight
{
    public class AnimationGachaPanel : PanelView
    {
        [SerializeField]
        private AnimationClipConfig m_PlaySummonAnimationClip;
        [SerializeField]
        private AvatarSpineUI m_SpineVFX;
        [SerializeField]
        private AvatarSpineUI m_CharacterSpineUI;
        [SerializeField]
        private AnimationItemView m_AnimationItemView;
        public AvatarSpineUI SpineVFX => m_SpineVFX;
        public AnimationItemView AnimationItemView => m_AnimationItemView;

        [SerializeField, MMReadOnly]
        private int m_GachaIndex = 0;
        [SerializeField, MMReadOnly]
        private List<GachaRewardConfig> m_GachaPulled = new List<GachaRewardConfig>();
        [SerializeField]
        private UnityEvent<List<GachaRewardConfig>> m_OnPreviewDone;
        protected override void HideInternal()
        {
            base.HideInternal();
            m_OnPreviewDone?.Invoke(m_GachaPulled);
            m_CharacterSpineUI.Hide();
            m_AnimationItemView.Hide();
            m_SpineVFX.Hide();
        }
        public void SpawnGacha(List<GachaRewardConfig> gacharRewards)
        {
            m_GachaIndex = 0;
            m_GachaPulled.Clear();
            m_GachaPulled = new List<GachaRewardConfig>(gacharRewards);    
            
            if (m_GachaPulled.Count > 0)
            {
                GachaRewardConfig gachaReward = m_GachaPulled[m_GachaIndex];
                ShowPull(gachaReward);
            }
            else
            {
                HideInternal();
            }
        }
        public void ShowNextPull()
        {
            m_GachaIndex++;
            if (m_GachaIndex >= m_GachaPulled.Count)
            {
                HideInternal();
                return;
            }
            GachaRewardConfig so = m_GachaPulled[m_GachaIndex];
            ShowPull(so);
        }

        private Coroutine m_PlayCoroutine;
        private void ShowPull(GachaRewardConfig gachaReward)
        {
            
            if (m_PlayCoroutine != null)
            {
                StopCoroutine(m_PlayCoroutine);
                m_PlayCoroutine = null;
            }
            
            if (gachaReward.GachaItemConfig is HeroUnitConfig heroConfig)
            {
                ShowInternal();
                m_PlayCoroutine = StartCoroutine(PlayGachaEffect(gachaReward, heroConfig));
            }
            else
            {
                ShowInternal();
                m_AnimationItemView.Init(gachaReward);
                m_AnimationItemView.Show();
                m_SpineVFX.Hide();
            }
        }
        [SerializeField]
        private float m_RevealDelay = 3f;
        private IEnumerator PlayGachaEffect(GachaRewardConfig reward, HeroUnitConfig heroConfig)
        {
            m_AnimationItemView.Init(reward);
            m_AnimationItemView.Hide();
            m_CharacterSpineUI.Hide();
            m_SpineVFX.Show();
            m_SpineVFX.PlayClip(m_PlaySummonAnimationClip);
            m_SpineVFX.SetColor(heroConfig.CollectibleField.RarityConfig.Color);
            m_CharacterSpineUI.SetSkeletonDataAsset(heroConfig.SkeletonDataAsset);
            yield return new WaitForSeconds(m_RevealDelay);
            m_CharacterSpineUI.Show();
        }

        // called on button
        public void ShowAnimationItemView()
        {
            m_AnimationItemView.Show();
            //m_SpineVFX.Hide();
            //m_CharacterSpineUI.Hide();
        }
    }
}
