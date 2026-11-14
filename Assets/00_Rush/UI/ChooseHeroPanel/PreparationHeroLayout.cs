using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class PreparationHeroLayout : MonoBehaviour
    {
        [SerializeField] private PreparationPanel m_Panel;
        [SerializeField] private RectTransform m_List;
        [SerializeField] private GridLayoutGroup m_Grid;
        [SerializeField] private GameObject m_Detail;
        [SerializeField] private GameObject m_Actions;
        [SerializeField] private Button m_Picker;
        [SerializeField] private TextMeshProUGUI m_Arrow;
        [SerializeField] private TextMeshProUGUI m_SelectedName;
        [SerializeField] private Image m_SelectedIcon;
        [SerializeField] private TextMeshProUGUI m_HeroNameLabel;
        [SerializeField] private TextMeshProUGUI m_Rarity;
        [SerializeField] private TextMeshProUGUI m_Level;
        [SerializeField] private ScrollRect m_Scroll;
        [SerializeField] private RectTransform m_PlatformContent;
        [SerializeField] private PlatformAlbum m_Album;
        [SerializeField] private HeroProgressFeedback m_ProgressFeedback;
        private bool m_Open;
        private const float c_ListBottom = 0.158f;
        private const float c_HeroListTop = 0.945f;
        private const float c_PlatformListTop = 0.37f;
        private const float c_PlatformListBottom = 0.077f;


        private const int c_HeroColumns = 3;
        private bool IsHero => m_Panel.SelectCharacterMode == SelectCharacterMode.Hero;

        public void ToggleRoster()
        {
            m_Open = IsHero && !m_Open;
            ApplyModeInternal();
            if (m_Open) m_Scroll.verticalNormalizedPosition = 1f;
        }

        public void OpenHeroRoster()
        {
            if (IsHero && !m_Open) ToggleRoster();
        }

        public void ResetMode()
        {
            m_Open = false;
            ApplyModeInternal();
        }

        public void PresentHero(HeroUnitConfig config)
        {
            if (config == null) return;
            var collectible = config.CollectibleField;
            m_SelectedName.text = m_HeroNameLabel.text = config.BaseInfo.Name;
            m_Rarity.text = collectible.RarityConfig.BaseInfo.Name.ToUpperInvariant();
            m_Rarity.color = collectible.RarityConfig.Color;
            m_SelectedIcon.sprite = collectible.Icon;
            var heroes = Player.Instance.HeroesCollection;
            if (m_ProgressFeedback != null) m_ProgressFeedback.Bind(heroes);
            var unit = heroes.GetHeroUnit(config);
            m_Level.text = $"Level {unit.Level}";
        }

        public void CloseAfterSelection()
        {
            if (!m_Open) return;
            m_Open = false;
            ApplyModeInternal();
        }

        private void ApplyModeInternal()
        {
            bool hero = IsHero;
            m_Picker.gameObject.SetActive(hero);
            m_List.anchorMin = new Vector2(0f, hero ? c_ListBottom : c_PlatformListBottom);
            m_List.anchorMax = new Vector2(1f, hero ? c_HeroListTop : c_PlatformListTop);
            m_List.offsetMin = m_List.offsetMax = Vector2.zero;
            m_List.gameObject.SetActive(hero ? m_Open : m_Album == null);
            m_Detail.SetActive(!m_Open);
            m_Actions.SetActive(!m_Open);
            m_Arrow.text = m_Open ? "▼" : "▲";
            var heroContent = (RectTransform)m_Grid.transform;
            m_Scroll.content = hero ? heroContent : m_PlatformContent;
            heroContent.gameObject.SetActive(hero);
            m_PlatformContent.gameObject.SetActive(!hero);
            m_Grid.constraintCount = c_HeroColumns;
            if (hero) ResizeGridInternal();

        }

        private void LateUpdate()
        {
            if (m_Open) ResizeGridInternal();
        }

        private void ResizeGridInternal()
        {
            var viewport = m_Scroll.viewport.rect;
            var padding = m_Grid.padding;
            var spacing = m_Grid.spacing * (c_HeroColumns - 1);
            var size = new Vector2((viewport.width - padding.horizontal - spacing.x) / c_HeroColumns,
                (viewport.height - padding.vertical - spacing.y) / c_HeroColumns);
            if (size.x > 0 && size.y > 0 && m_Grid.cellSize != size) m_Grid.cellSize = size;
        }
    }
}
