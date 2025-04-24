using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class PlatformSelectView : UIView
    {
        [SerializeField]
        private StandbyPlatformDefinition m_PlatformPrefab;
        [SerializeField]
        private Image m_UnitIcon;
        [SerializeField]
        private GameObject m_LockIcon;
        [SerializeField]
        private TextMeshProUGUI m_AmountText;
        [SerializeField]
        private Button m_SelectButton;
        [SerializeField]
        private UnityEvent<StandbyPlatformDefinition> m_OnPlatformSelected = new();
        public StandbyPlatformDefinition PlatformPrefab => m_PlatformPrefab;
        private void OnEnable()
        {
            InitInternal();
        }
        private void SelectPlatformInternal()
        {
            Player.Instance.SelectStandbyPlatform(m_PlatformPrefab);
            OnCharacterSelectedInvoke();
        }
        public void SelectPlatform()
        {
            SelectPlatformInternal();
        }
        private void InitInternal()
        {
            PlatformUnit platform = Player.Instance.GetPlatformOwned(m_PlatformPrefab);
            InitInternal(platform);
        }
        private void InitInternal(PlatformUnit unit)
        {
            unit.Init();
            m_PlatformPrefab = unit.StanbyPlatform;
            m_LockIcon.SetActive(!unit.IsOwned);
            m_SelectButton.interactable = unit.IsOwned;
            m_UnitIcon.sprite = m_PlatformPrefab.Icon;
            m_AmountText.text = unit.Amount.ToString();

            m_SelectButton.onClick.RemoveAllListeners();
            m_SelectButton.onClick.AddListener(SelectPlatformInternal);
        }

        private void OnCharacterSelectedInvoke()
        {
            m_OnPlatformSelected?.Invoke(m_PlatformPrefab);
        }
    }
}
