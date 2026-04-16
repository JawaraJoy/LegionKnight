using UnityEngine;
using UnityEngine.UI;
using System;

namespace Rush
{
    public class GachaBannerButtonUI : MonoBehaviour
    {
        [SerializeField] private Button m_Button;
        [SerializeField] private Image m_BannerImage;

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
        }

        private void OnClickedInternal() => m_OnSelected?.Invoke(m_Banner);
    }
}