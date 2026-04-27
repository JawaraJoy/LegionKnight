using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;

namespace Rush
{
    // Panel that appears in front of preview when a Hero is drawn
    // Plays spine animation then waits for player to close
    public class GachaHeroRevealPanel : PanelView
    {
        [SerializeField] private AvatarSpineUI m_AvatarSpineUI;

        [Header("Info")]
        [SerializeField] private TextMeshProUGUI m_HeroNameText;
        [SerializeField] private TextMeshProUGUI m_RarityText;
        [SerializeField] private Image m_RarityFrame;
        [SerializeField] private AvatarSpineUI m_AuraEffectspine;

        [Header("Reveal Clip")]
        // Clip to play when hero is revealed — configure in inspector
        [SerializeField] private AnimationClipConfig m_RevealClip;
        [SerializeField] private AnimationClipConfig m_EffectClip;

        [Header("Button")]
        [SerializeField] private Button m_CloseButton;

        private System.Action m_OnClosed;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (m_CloseButton != null) m_CloseButton.onClick.AddListener(OnCloseClickedInternal);
        }

        protected override void HideInternal()
        {
            if (m_CloseButton != null) m_CloseButton.onClick.RemoveListener(OnCloseClickedInternal);
            base.HideInternal();
        }

        public void Show(HeroUnitConfig heroConfig, System.Action onClosed)
        {
            m_OnClosed = onClosed;

            // Swap skeleton data to this hero
            if (m_AvatarSpineUI != null)
                m_AvatarSpineUI.SetSkeletonDataAsset(heroConfig.SkeletonDataAsset);

            // Display name and rarity
            if (m_HeroNameText != null)
                m_HeroNameText.text = heroConfig.BaseInfo.Name;

            var rarity = heroConfig.CollectibleField?.RarityConfig;
            if (rarity != null)
            {
                if (m_RarityText != null)
                {
                    m_RarityText.gameObject.SetActive(true);
                    m_RarityText.text = rarity.BaseInfo.Name;
                    m_RarityText.color = rarity.Color;
                }
                if (m_RarityFrame != null)
                {
                    m_RarityFrame.gameObject.SetActive(true);
                    m_RarityFrame.color = rarity.Color;
                }
            }
            else
            {
                if (m_RarityText != null) m_RarityText.gameObject.SetActive(false);
                if (m_RarityFrame != null) m_RarityFrame.gameObject.SetActive(false);
            }
            if (m_AuraEffectspine != null)
                m_AuraEffectspine.PlayClip(m_EffectClip);
                m_AuraEffectspine.SetColor(rarity.Color);
            ShowInternal();

            // Play reveal animation after panel is shown
            if (m_RevealClip != null && m_AvatarSpineUI != null)
                m_AvatarSpineUI.PlayClip(m_RevealClip);
            
        }

        private void OnCloseClickedInternal()
        {
            Hide();
            m_OnClosed?.Invoke();
            m_OnClosed = null;
        }
    }
}