using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class BannerSelectView : UIView
    {
        [SerializeField]
        private BannerDefinition m_Definition;
        [SerializeField]
        private Image m_Icon;
        [SerializeField]
        private Button m_SelectButton;

        [SerializeField]
        private TextMeshProUGUI m_TitleText;

        private void Start()
        {
            m_Icon.sprite = m_Definition.SmallVisualBanner;
            m_TitleText.text = m_Definition.PromoText;
            m_SelectButton.onClick?.AddListener(() => SelectBanner(m_Definition));
        }
        private void SelectBanner(BannerDefinition defi)
        {
            GameManager.Instance.SelectBanner(defi);
            
        }
    }
}
