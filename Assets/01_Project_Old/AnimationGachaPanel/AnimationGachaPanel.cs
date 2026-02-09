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
            
            if (m_GachaPulled.Count > 0)
            {
                GachaReward gachaReward = m_GachaPulled[m_GachaIndex];
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
            GachaReward so = m_GachaPulled[m_GachaIndex];
            ShowPull(so);
        }

        private Coroutine m_PlayCoroutine;
        private void ShowPull(GachaReward gachaReward)
        {
            
            if (m_PlayCoroutine != null)
            {
                StopCoroutine(m_PlayCoroutine);
                m_PlayCoroutine = null;
            }
            
            if (gachaReward.Definition is CharacterDefinition character)
            {
                ShowInternal();
                m_PlayCoroutine = StartCoroutine(PlayGachaEffect(gachaReward, character));
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
