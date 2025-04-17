using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class PlatformSelectView : UIView
    {
        [SerializeField]
        private StanbyPlatform m_PlatformPrefab;
        [SerializeField]
        private Image m_UnitIcon;
        [SerializeField]
        private GameObject m_LockIcon;
        [SerializeField]
        private Button m_SelectButton;
        [SerializeField]
        private UnityEvent<StanbyPlatform> m_OnPlatformSelected = new();
        public StanbyPlatform PlatformPrefab => m_PlatformPrefab;
        private void OnEnable()
        {
            InitInternal();
        }
        private void SelectPlatformInternal()
        {
            Player.Instance.SetUsedStandbyPlatform(m_PlatformPrefab);
            OnCharacterSelectedInvoke();
        }
        public void SelectPlatform()
        {
            SelectPlatformInternal();
        }
        private void InitInternal()
        {
            PlatformOwned platform = Player.Instance.GetPlatformOwned(m_PlatformPrefab);
            InitInternal(platform);
        }
        private void InitInternal(PlatformOwned unit)
        {
            m_PlatformPrefab = unit.StanbyPlatform;
            m_LockIcon.SetActive(!unit.IsOwned);
            m_SelectButton.interactable = unit.IsOwned;
            m_SelectButton.onClick.RemoveAllListeners();
            m_SelectButton.onClick.AddListener(SelectPlatformInternal);
        }

        private void OnCharacterSelectedInvoke()
        {
            m_OnPlatformSelected?.Invoke(m_PlatformPrefab);
        }
    }
}
