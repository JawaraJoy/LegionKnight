using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Rush;

namespace LegionKnight
{
    public partial class PlatformSelectView : UIView
    {
        [SerializeField]
        private PlatformConfig m_PlatformDefi;
        [SerializeField]
        private Image m_UnitIcon;
        [SerializeField]
        private Image m_RarityColor;
        [SerializeField]
        private Image m_EquipedSign;
        [SerializeField]
        private GameObject m_LockIcon;
        [SerializeField]
        private TextMeshProUGUI m_AmountText;
        [SerializeField]
        private Button m_SelectButton;
        [SerializeField]
        private UnityEvent<PlatformConfig> m_OnPlatformSelected = new();
        public PlatformConfig PlatformDefi => m_PlatformDefi;
        private void SelectPlatformInternal()
        {
            Player.Instance.SelectStandbyPlatform(m_PlatformDefi);
            OnCharacterSelectedInvoke();
        }
        public void SelectPlatform()
        {
            SelectPlatformInternal();
        }
        private void InitInternal()
        {
            PlatformUnit platform = Player.Instance.GetPlatformOwned(m_PlatformDefi);
            InitInternal(platform);
        }
        private void InitInternal(PlatformUnit unit)
        {
            unit.Init();
            m_PlatformDefi = unit.StanbyPlatform;
            m_LockIcon.SetActive(!unit.IsOwned);
            m_SelectButton.interactable = unit.IsOwned;
            m_UnitIcon.sprite = m_PlatformDefi.Icon;
            m_AmountText.text = unit.Amount.ToString();
            m_RarityColor.color = unit.StanbyPlatform.Rarity.Color;

            m_EquipedSign.gameObject.SetActive(unit.IsEquiped);

            m_SelectButton.onClick.RemoveAllListeners();
            m_SelectButton.onClick.AddListener(SelectPlatformInternal);

            HideInternal();
        }

        public void RefreshEquiped()
        {
            PlatformUnit platform = Player.Instance.GetPlatformOwned(m_PlatformDefi);
            m_EquipedSign.gameObject.SetActive(platform.IsEquiped);
        }
        public void Init(PlatformUnit unit)
        {
            InitInternal(unit);
        }

        private void OnCharacterSelectedInvoke()
        {
            m_OnPlatformSelected?.Invoke(m_PlatformDefi);
        }
    }
}
