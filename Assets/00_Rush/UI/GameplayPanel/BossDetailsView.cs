using System.Collections.Generic;
using System.Globalization;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class BossDetailsView : MonoBehaviour
    {
        [SerializeField] private UnitDetailPanel m_Panel;
        [SerializeField] private TMP_Text m_NameLabel, m_Level, m_Attack, m_Defense, m_Health, m_Empty;
        [SerializeField] private Image m_RarityFrame, m_PortraitFallback;
        [SerializeField] private AvatarSpineUI m_Avatar;
        [SerializeField] private SkeletonGraphic m_Graphic;
        [SerializeField] private SkeletonAnimation m_Animation;
        [SerializeField] private RectTransform m_PortraitArea;
        [SerializeField] private Material m_PmaMaterial, m_StraightMaterial;
        [System.Serializable]
        private struct IdlePortrait
        {
            public SkeletonDataAsset Rig;
            // Visible artwork across the idle cycle, measured in skeleton units.
            public Rect VisibleBounds;
        }
        [SerializeField] private IdlePortrait[] m_IdlePortraits;
        [SerializeField] private ScrollRect m_Scroll;
        [SerializeField] private BossAbilityCard m_Template;
        [SerializeField] private SkillDescriptionPanel m_SkillDetail;
        private readonly List<BossAbilityCard> m_Cards = new();
        private readonly HashSet<SkillConfig> m_Listed = new();
        private Unit m_Owner;
        private SkillController m_Skills;
        private Damageable m_HP;
        private int m_Generation, m_LastLevel, m_LastHealth;
        private bool m_NeedsFit;

        public void Display(Unit unit)
        {
            Unbind();
            m_Owner = unit;
            m_Generation = unit.SpawnGeneration;
            unit.HasBind(out m_Skills);
            unit.HasBind(out m_HP);
            if (m_Skills != null)
            {
                m_Skills.OnSkillAdded?.AddListener(OnRosterChanged);
                m_Skills.OnSkillRemoved?.AddListener(OnRosterChanged);
            }
            m_NameLabel.text = unit.Config.BaseInfo.Name.Trim();
            var rarity = unit.Config.CollectibleField.RarityConfig;
            m_RarityFrame.color = rarity != null ? rarity.Color : Color.white;
            Canvas.ForceUpdateCanvases();
            ShowPortrait(unit);
            RefreshStats();
            PopulateCards();
            m_Scroll.StopMovement();
            m_Scroll.verticalNormalizedPosition = 1f;
        }

        private void LateUpdate()
        {
            if (m_Owner == null || !m_Owner.isActiveAndEnabled || m_Owner.SpawnGeneration != m_Generation ||
                (m_HP != null && !m_HP.IsAlive))
            {
                if (m_Panel.IsShown) m_Panel.Hide();
                return;
            }
            if (m_NeedsFit) FitPortrait();
            if (m_LastLevel != m_Owner.Progression.Level || (m_HP != null && m_LastHealth != m_HP.MaxHealth))
            {
                RefreshStats();
                PopulateCards();
            }
        }

        private void RefreshStats()
        {
            m_LastLevel = m_Owner.Progression.Level;
            var stats = m_Owner.BaseStats;
            if (m_Owner.HasBind(out StatController modifiers)) stats = modifiers.GetFinalStat(stats);
            m_LastHealth = m_HP != null ? m_HP.MaxHealth : stats.Health;
            m_Level.SetText("Lv. {0}", m_LastLevel);
            m_Attack.text = stats.Attack.ToString("N0", CultureInfo.InvariantCulture);
            m_Defense.text = (m_HP != null ? m_HP.Defense : stats.Defense).ToString("N0", CultureInfo.InvariantCulture);
            m_Health.text = m_LastHealth.ToString("N0", CultureInfo.InvariantCulture);
        }

        private void ShowPortrait(Unit unit)
        {
            var config = unit.Config;
            var rig = config.SkeletonDataAsset;
            m_NeedsFit = false;
            m_PortraitFallback.sprite = config.CollectibleField.Icon;
            m_PortraitFallback.enabled = rig == null && m_PortraitFallback.sprite != null;
            m_Avatar.gameObject.SetActive(rig != null);
            if (rig == null) return;
            var atlas = rig.atlasAssets.Length > 0 ? rig.atlasAssets[0].PrimaryMaterial : null;
            m_Graphic.material = atlas != null && atlas.IsKeywordEnabled("_STRAIGHT_ALPHA_INPUT")
                ? m_StraightMaterial : m_PmaMaterial;
            m_Graphic.rectTransform.localScale = Vector3.one;
            m_Graphic.rectTransform.anchoredPosition = Vector2.zero;
            m_Avatar.Init(unit);
            if (config.Idle != null) m_Avatar.PlayClipInterrupt(config.Idle);
            // Init can update the skeleton earlier in this same frame. Apply the newly selected idle immediately.
            m_Animation.Update(0f);
            m_Graphic.UpdateMesh();
            m_NeedsFit = true;
            FitPortrait();
        }

        private void FitPortrait()
        {
            var bounds = new Bounds();
            bool hasMesh = m_Graphic.allowMultipleCanvasRenderers
                ? m_Graphic.GetMeshBoundsMultipleRenderers(ref bounds)
                : m_Graphic.GetMeshBoundsSingleRenderer(ref bounds);
            if (!hasMesh || bounds.size.x <= 0 || bounds.size.y <= 0) return;
            foreach (var portrait in m_IdlePortraits)
            {
                if (portrait.Rig != m_Graphic.SkeletonDataAsset || portrait.VisibleBounds.width <= 0 ||
                    portrait.VisibleBounds.height <= 0) continue;
                Rect visible = portrait.VisibleBounds;
                bounds = new Bounds(visible.center * m_Graphic.MeshScale, visible.size * m_Graphic.MeshScale);
                break;
            }
            Vector2 space = m_PortraitArea.rect.size;
            float scale = Mathf.Min(space.x / bounds.size.x, space.y / bounds.size.y) * .93f;
            m_Graphic.rectTransform.localScale = Vector3.one * scale;
            m_Graphic.rectTransform.anchoredPosition = -(Vector2)bounds.center * scale;
            m_NeedsFit = false;
        }

        private void OnRosterChanged(Skill skill) => PopulateCards();

        private void PopulateCards()
        {
            m_Listed.Clear();
            int count = 0;
            foreach (var config in m_Owner.Config.Skills)
            {
                if (config == null || !m_Listed.Add(config)) continue;
                Skill runtime = null;
                if (m_Skills != null) m_Skills.HasSkill(config, out runtime);
                AddCard(count++, config, runtime);
            }
            if (m_Skills != null)
            {
                foreach (var skill in m_Skills.Skills)
                    if (skill != null && skill.SkillConfig != null && m_Listed.Add(skill.SkillConfig))
                        AddCard(count++, skill.SkillConfig, skill);
            }
            for (int i = count; i < m_Cards.Count; i++) m_Cards[i].gameObject.SetActive(false);
            m_Empty.gameObject.SetActive(count == 0);
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_Scroll.content);
            Canvas.ForceUpdateCanvases();
        }

        private void AddCard(int index, SkillConfig config, Skill runtime)
        {
            if (index == m_Cards.Count) m_Cards.Add(Instantiate(m_Template, m_Scroll.content, false));
            var card = m_Cards[index];
            card.gameObject.SetActive(true);
            card.Display(config, runtime, m_Skills, m_Owner.Config, m_LastHealth, m_SkillDetail);
        }

        private void Unbind()
        {
            if (m_Skills != null)
            {
                m_Skills.OnSkillAdded?.RemoveListener(OnRosterChanged);
                m_Skills.OnSkillRemoved?.RemoveListener(OnRosterChanged);
            }
            m_Skills = null;
            m_Owner = null;
            m_HP = null;
            m_NeedsFit = false;
        }

        private void OnDisable() => Unbind();
    }
}
