using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace Rush
{
    public class GachaBannerButtonUI : MonoBehaviour
    {
        [SerializeField] private Button m_Button;
        [SerializeField] private Image m_BannerImage;
        [SerializeField] private TextMeshProUGUI m_BannerNameText;

        [SerializeField] private Color m_SelectedFace = new Color32(72, 91, 134, 255);
        [SerializeField] private Color m_SelectedLabel = Color.white;
        [SerializeField] private Image m_TabBorder;
        [SerializeField] private Sprite m_ActiveBorder;
        [SerializeField] private Sprite m_InactiveBorder;
        private Color m_NormalFace;
        private Color m_NormalLabel;
        private GachaBannerConfig m_Banner;
        private Action<GachaBannerConfig> m_OnSelected;

        public GachaBannerConfig Banner => m_Banner;

        private void Awake()
        {
            if (m_Button != null) m_Button.onClick.AddListener(OnClickedInternal);
        }

        public void Setup(GachaBannerConfig banner, Action<GachaBannerConfig> onSelected)
        {
            if (m_Banner == null)
            {
                if (m_Button != null && m_Button.targetGraphic != null) m_NormalFace = m_Button.targetGraphic.color;
                if (m_BannerNameText != null) m_NormalLabel = m_BannerNameText.color;
            }
            m_Banner = banner;
            m_OnSelected = onSelected;
            if (m_BannerImage != null) m_BannerImage.sprite = banner.BannerButtonSprite;
            if (m_BannerNameText != null) m_BannerNameText.text = banner.PanelHeroName;
        }

        public void SetActiveBanner(GachaBannerConfig banner)
        {
            bool selected = m_Banner == banner;
            if (m_TabBorder != null)
            {
                m_TabBorder.sprite = selected ? m_ActiveBorder : m_InactiveBorder;
                m_TabBorder.enabled = m_TabBorder.sprite != null;
            }
            if (m_Button != null && m_Button.targetGraphic != null)
                m_Button.targetGraphic.color = selected ? m_SelectedFace : m_NormalFace;
            if (m_BannerNameText != null)
                m_BannerNameText.color = selected ? m_SelectedLabel : m_NormalLabel;
        }

        private void OnClickedInternal() => m_OnSelected?.Invoke(m_Banner);
    }
}