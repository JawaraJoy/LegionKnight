using MoreMountains.Tools;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public class AnimationGachaPanel : PanelView
    {
        [SerializeField]
        private SpineAnimDefinition m_PlayGachaCommon;
        [SerializeField]
        private SpineAnimDefinition m_PlayGachaRare;
        [SerializeField]
        private SpineAnimDefinition m_PlayGachaEpic;
        [SerializeField]
        private SpineUI m_SpineVFX;
        [SerializeField]
        private SpineUI m_CharacterSpineUI;
        [SerializeField]
        private AnimationItemView m_AnimationItemView;
        public SpineUI SpineVFX => m_SpineVFX;
        public AnimationItemView AnimationItemView => m_AnimationItemView;

        [SerializeField, MMReadOnly]
        private int m_GachaIndex = 0;
        [SerializeField, MMReadOnly]
        private List<GachaReward> m_GachaPulled = new List<GachaReward>();
        [SerializeField, MMReadOnly]
        private List<GachaReward> m_GachaFiltered = new List<GachaReward>();
        [SerializeField]
        private UnityEvent<SkeletonDataAsset> m_OnSpineAssetPulled;
        [SerializeField]
        private UnityEvent<List<GachaReward>> m_OnPreviewDone;
        protected override void HideInternal()
        {
            base.HideInternal();
            m_OnPreviewDone?.Invoke(m_GachaPulled);
            m_CharacterSpineUI.Hide();
            m_AnimationItemView.Hide();
            m_SpineVFX.Hide();
        }
        public void SpawnGacha(List<GachaReward> gacharRewards)
        {
            m_GachaIndex = 0;
            m_GachaPulled.Clear();
            m_GachaPulled = new List<GachaReward>(gacharRewards);    
            m_GachaFiltered.Clear();
            foreach (GachaReward gaachaReward in gacharRewards)
            {
                if (gaachaReward.Definition is CharacterDefinition)
                {
                    m_GachaFiltered.Add(gaachaReward);
                }
                if (gaachaReward.Definition is StandbyPlatformDefinition)
                {
                    m_GachaFiltered.Add(gaachaReward);
                }
            }
            GachaReward gachaReward = m_GachaFiltered[m_GachaIndex];
            
            if (m_GachaFiltered.Count > 0)
            {
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
            if (m_GachaIndex >= m_GachaFiltered.Count)
            {
                HideInternal();
                return;
            }
            GachaReward so = m_GachaFiltered[m_GachaIndex];
            ShowPull(so);
        }

        private void ShowPull(GachaReward gachaReward)
        {
            
            if (gachaReward.Definition is CharacterDefinition character)
            {
                StartCoroutine(PlayGachaEffect(gachaReward, character));
            }
            else
            {
                m_AnimationItemView.Init(gachaReward);
                m_AnimationItemView.Show();
                m_SpineVFX.Hide();
            }
            ShowInternal();
        }

        private IEnumerator PlayGachaEffect(GachaReward reward, CharacterDefinition character)
        {
            m_AnimationItemView.Init(reward);
            m_AnimationItemView.Hide();
            m_CharacterSpineUI.Hide();
            m_SpineVFX.Show();
            switch (character.Rarity)
            {
                case Rarity.Common:
                    m_SpineVFX.Play(m_PlayGachaCommon);
                    break;
                case Rarity.Rare:
                    m_SpineVFX.Play(m_PlayGachaRare);
                    break;
                case Rarity.Epic:
                    m_SpineVFX.Play(m_PlayGachaEpic);
                    break;
                default:
                    Debug.LogError($"Out of context {typeof(Rarity)}");
                    break;
            }
            m_CharacterSpineUI.SetSkeletonDataAsset(character);
            yield return new WaitForSeconds(3f);
            m_CharacterSpineUI.Show();
        }

        public void ShowAnimationItemView()
        {
            m_AnimationItemView.Show();
            //m_SpineVFX.Hide();
            //m_CharacterSpineUI.Hide();
        }
    }
}
