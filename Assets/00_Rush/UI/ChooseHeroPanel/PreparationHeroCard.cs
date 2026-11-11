using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class PreparationHeroCard : MonoBehaviour
    {
        [SerializeField] private Button m_Button;
        [SerializeField] private TextMeshProUGUI m_HeroNameLabel;
        [SerializeField] private GameObject m_SelectedMark;
        private HeroUnitConfig m_Config;
        private PreparationHeroLayout m_Layout;

        public void Bind(HeroUnitConfig config, PreparationHeroLayout layout)
        {
            m_Config = config;
            m_Layout = layout;
            m_HeroNameLabel.text = config.BaseInfo.Name;
            m_Button.onClick.RemoveListener(ClosePickerInternal);
            m_Button.onClick.AddListener(ClosePickerInternal);
            UpdateSelectionInternal();
        }

        private void OnEnable()
        {
            UpdateSelectionInternal();
        }

        private void UpdateSelectionInternal()
        {
            if (m_Config != null && m_SelectedMark != null)
                m_SelectedMark.SetActive(Player.Instance.HeroesCollection.SelectedHero == m_Config);
        }

        private void ClosePickerInternal()
        {
            if (m_Layout != null) m_Layout.CloseAfterSelection();
        }
    }
}
