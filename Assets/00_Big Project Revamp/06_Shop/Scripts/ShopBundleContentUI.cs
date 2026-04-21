using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class ShopBundleContentUI : MonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_AmountText;

        public void Setup(ShopBundleEntry entry)
        {
            SetupInternal(entry.Collectible, entry.Amount);
        }

        public void SetupFromIAP(IAPBundleEntry entry)
        {
            SetupInternal(entry.Collectible, entry.Amount);
        }

        private void SetupInternal(CollectibleConfig collectible, int amount)
        {
            if (m_Icon != null) m_Icon.sprite = collectible.CollectibleField?.Icon;
            if (m_NameText != null) m_NameText.text = collectible.BaseInfo.Name;

            if (m_AmountText != null)
            {
                bool show = amount >= 2;
                m_AmountText.gameObject.SetActive(show);
                if (show) m_AmountText.text = $"x{amount}";
            }
        }
    }
}