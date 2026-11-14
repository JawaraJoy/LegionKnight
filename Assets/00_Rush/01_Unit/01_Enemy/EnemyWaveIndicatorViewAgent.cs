using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class EnemyWaveIndicatorViewAgent : MonoBehaviour
    {
        [SerializeField] private BossEncounterNotice m_NoticePrefab;
        private BossEncounterNotice m_Notice;
        private static readonly Color32 s_Incoming = new(180, 78, 57, 255);
        private static readonly Color32 s_Defeated = new(52, 132, 119, 255);
        private static readonly Color32 s_Victory = new(204, 142, 51, 255);
        private const float c_EntranceNotice = 1.4f, c_DefeatNotice = 1.65f, c_VictoryNotice = 3.2f;

        private void Resolve(out PanelView panel, out WaveIndicatorView indicator)
        {
            panel = null;
            indicator = null;
            var canvas = CanvasManager.Instance;
            if (canvas == null || canvas.MainCanvas == null) return;
            var modern = canvas.GetPanel<NewGameplayPanel>();
            var legacy = canvas.GetPanel<GameplayPanel>();
            bool legacyVisible = legacy != null && legacy.Content != null && legacy.Content.activeInHierarchy;
            PanelView first = legacyVisible && (modern == null || modern.Content == null || !modern.Content.activeInHierarchy)
                ? (PanelView)legacy : modern;
            PanelView second = first == modern ? (PanelView)legacy : modern;
            panel = first != null ? first : second;
            if (first != null && first.HasBinding(out indicator)) return;
            if (second != null) second.HasBinding(out indicator);
        }

        public void SetSlider(int current, int max)
        {
            Resolve(out _, out var indicator);
            if (indicator != null) indicator.SetSlider(current, max);
        }
        public void SetWaveIcon(Sprite icon)
        {
            Resolve(out _, out var indicator);
            if (indicator != null) indicator.SetWaveIcon(icon);
        }
        public void Show()
        {
            Resolve(out _, out var indicator);
            if (indicator != null) indicator.Show();
        }
        public void Hide()
        {
            Resolve(out _, out var indicator);
            if (indicator != null && !indicator.UsesBattleHud) indicator.Hide();
        }
        public void ShowArrival(Unit unit)
        {
            var config = unit.Config as BossUnitConfig;
            if (config != null) Present("BOSS ENCOUNTER", config.BaseInfo.Name.Trim(), config.CollectibleField.Icon, s_Incoming, c_EntranceNotice, BossEncounterNotice.Moment.Encounter, unit);
        }
        public void ShowDefeat(BossUnitConfig boss)
        {
            if (boss != null) Present("BOSS DEFEATED", $"{boss.BaseInfo.Name.Trim()}  ·  +{boss.ExpReward} EXP  ·  +{boss.ScoreReward} score",
                boss.CollectibleField.Icon, s_Defeated, c_DefeatNotice, BossEncounterNotice.Moment.Defeated, boss);
        }
        public void ShowVictory(StageConfig stage)
        {
            var waves = stage.EnemyWaveConfigs;
            var boss = waves[waves.Length - 1].BossWaveField.BossConfig;
            Present("VICTORY", stage.BaseInfo.Name.Trim() + "  ·  Stage clear", boss?.CollectibleField.Icon, s_Victory, c_VictoryNotice);
        }
        private void Present(string title, string detail, Sprite icon, Color tint, float seconds,
            BossEncounterNotice.Moment moment = BossEncounterNotice.Moment.None, Object subject = null)
        {
            Resolve(out var panel, out _);
            if (panel == null || panel.Content == null || m_NoticePrefab == null) return;
            Transform host = panel.Content.transform;
            if (m_Notice == null) m_Notice = Instantiate(m_NoticePrefab, host, false);
            else if (m_Notice.transform.parent != host) m_Notice.transform.SetParent(host, false);
            m_Notice.transform.SetAsLastSibling();
            m_Notice.Present(title, detail, icon, tint, seconds, moment, subject);
        }
        public void ClearNotice()
        {
            if (m_Notice != null) m_Notice.gameObject.SetActive(false);
        }
    }
}
