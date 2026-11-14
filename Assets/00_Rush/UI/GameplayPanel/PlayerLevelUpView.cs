using System.Collections.Generic;
using System.Globalization;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class PlayerLevelUpView : MonoBehaviour
    {
        [SerializeField] private RectTransform m_Board, m_Emblem;
        [SerializeField] private CanvasGroup m_Sheet, m_Rewards, m_Unlock, m_Spark;
        [SerializeField] private TMP_Text m_Level, m_Transition, m_ExpLabel, m_Exp, m_Empty, m_Received, m_Messages;
        [SerializeField] private Image m_Fill;
        [SerializeField] private ScrollRect m_Scroll;
        [SerializeField] private PlayerLevelRewardRow m_Template;
        [SerializeField] private PlayModeSelection m_Mode;
        private readonly List<PlayerLevelRewardRow> m_Rows = new();
        private readonly List<LootField> m_Loot = new();
        private PlayerProgression m_Player;
        private int m_From, m_To, m_LastExp = -1, m_LastMax = -1;
        private float m_Time;
        private const float c_End = 1.32f;

        public void Present(PlayerProgression player, int previous)
        {
            m_Player = player;
            m_From = previous;
            Fit();
            m_Time = 0;
            RefreshLevels();
            Draw();
        }

        private void OnRectTransformDimensionsChange() => Fit();

        private void Fit()
        {
            if (!m_Board) return;
            var area = ((RectTransform)transform).rect.size;
            float scale = Mathf.Min(area.x / 480f, area.y / 800f);
            if (scale > 0) m_Board.localScale = Vector3.one * scale;
        }

        private void OnDisable() => m_Player = null;

        private void LateUpdate()
        {
            if (!m_Player) return;
            if (m_To != m_Player.GetCurrentLevel()) RefreshLevels();
            if (m_Time < c_End)
            {
                m_Time = Mathf.Min(c_End, m_Time + Time.unscaledDeltaTime);
                Draw();
            }
            else UpdateExperience(1);
        }

        private void RefreshLevels()
        {
            m_To = m_Player.GetCurrentLevel();
            m_Transition.text = $"Level {m_From}  →  Level {m_To}";
            m_Unlock.gameObject.SetActive(m_From < m_Mode.ColossalUnlockLevel && m_To >= m_Mode.ColossalUnlockLevel);
            m_Loot.Clear();
            for (int level = m_From + 1; level <= m_To; level++)
            {
                var loot = m_Player.GetLevelReward(level)?.LootFields;
                if (loot == null) continue;
                foreach (var item in loot)
                {
                    if (item?.ItemLoot == null || item.Amount <= 0) continue;
                    var existing = m_Loot.Find(entry => entry.ItemLoot == item.ItemLoot);
                    if (existing != null) existing.AddAmount(item.Amount);
                    else m_Loot.Add(item.Clone());
                }
            }
            for (int i = 0; i < m_Loot.Count; i++)
            {
                if (i == m_Rows.Count) m_Rows.Add(Instantiate(m_Template, m_Scroll.content, false));
                var row = m_Rows[i];
                row.transform.SetSiblingIndex(i);
                row.gameObject.SetActive(true);
                row.Display(m_Loot[i]);
            }
            for (int i = m_Loot.Count; i < m_Rows.Count; i++) m_Rows[i].gameObject.SetActive(false);
            m_Empty.gameObject.SetActive(m_Loot.Count == 0);
            m_Received.gameObject.SetActive(m_Loot.Count > 0);
            var messages = new List<string>();
            foreach (var achievement in Player.Instance.GetAchievementHolds())
                if (achievement.Definition.Type == AchieveType.Message && !string.IsNullOrWhiteSpace(achievement.UnclockedMessage))
                    messages.Add(achievement.UnclockedMessage);
            m_Messages.text = string.Join("\n", messages);
            m_Messages.gameObject.SetActive(messages.Count > 0);
            ((RectTransform)m_Scroll.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_Unlock.gameObject.activeSelf ? 83 : 188);
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_Scroll.content);
            m_Scroll.StopMovement();
            m_Scroll.verticalNormalizedPosition = 1;
            m_ExpLabel.text = m_Player.AtMaxLevel ? "Max level" : "Next level";
            m_LastExp = m_LastMax = -1;
            if (m_Time >= c_End) m_Level.SetText("{0}", m_To);
            m_Player.SetLevelUpTriggered(false);
        }

        private float Phase(float start, float duration) =>
            Mathf.SmoothStep(0, 1, Mathf.Clamp01((m_Time - start) / duration));

        private void Draw()
        {
            m_Sheet.alpha = Phase(-0.04f, .22f);
            float entrance = Phase(.04f, .35f);
            float pulse = Mathf.Sin(Mathf.PI * Phase(.33f, .42f));
            m_Emblem.localScale = Vector3.one * (.92f + .08f * entrance + .055f * pulse);
            m_Level.SetText("{0}", m_Time < .39f ? m_From : m_To);
            m_Level.alpha = m_Time < .39f ? 1 - Phase(.24f, .15f) : Phase(.39f, .14f);
            m_Spark.alpha = Mathf.Max(0, Mathf.Sin(Mathf.PI * Phase(.38f, .55f)));
            m_Rewards.alpha = Phase(.74f, .34f);
            m_Unlock.alpha = Phase(.96f, .34f);
            UpdateExperience(Phase(.57f, .45f));
        }

        private void UpdateExperience(float fraction)
        {
            int max = m_Player.GetCurrentMaxExperience();
            int xp = m_Player.AtMaxLevel ? max : Mathf.Clamp(m_Player.GetCurrentExperience(), 0, max);
            int shown = Mathf.RoundToInt(xp * fraction);
            if (shown == m_LastExp && max == m_LastMax) return;
            m_LastExp = shown;
            m_LastMax = max;
            m_Fill.fillAmount = max > 0 ? (float)shown / max : 0;
            m_Exp.text = m_Player.AtMaxLevel ? "MAX" : string.Format(CultureInfo.InvariantCulture, "{0:N0} / {1:N0} EXP", shown, max);
        }
    }
}
