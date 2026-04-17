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

        private GachaBannerConfig m_Banner;
        private Action<GachaBannerConfig> m_OnSelected;

        private void Awake()
        {
            if (m_Button != null) m_Button.onClick.AddListener(OnClickedInternal);
        }

        public void Setup(GachaBannerConfig banner, Action<GachaBannerConfig> onSelected)
        {
            m_Banner = banner;
            m_OnSelected = onSelected;
            if (m_BannerImage != null) m_BannerImage.sprite = banner.BannerButtonSprite;
            if (m_BannerNameText != null) m_BannerNameText.text = banner.BaseInfo.Name;
        }

        private void OnClickedInternal() => m_OnSelected?.Invoke(m_Banner);
    }
}