using System;
using System.Globalization;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class BattleGameOverView : MonoBehaviour
    {
        [Serializable] private class HeroField
        {
            public TMP_Text name, rarity;
            public Image splash, frame, rarityMark;
        }

        [Serializable] private class ResultField
        {
            public GameObject scoreColumn;
            public RectTransform experienceColumn;
            public TMP_Text score, experience;
        }

        [Serializable] private class ActionField
        {
            public Button revive, replay, secondaryReplay, home, doubleLoot;
            public RectTransform homeRow;
            public TMP_Text status, seconds;
            public Image timerFill;
        }

        [SerializeField] private BattleReportShare m_ReportShare;
        [SerializeField] private HeroField m_Hero;
        [SerializeField] private ResultField m_Result;
        [SerializeField] private ActionField m_Actions;
        [SerializeField] private GameOverPanel m_Panel;
        [SerializeField] private RevivePanel m_RevivePanel;
        [SerializeField] private EnergyConfirmationPanel m_EnergyPanel;
        [SerializeField] private GameStateConfig m_ReplayState, m_HomeState;
        [SerializeField, Min(0)] private float m_ReviveDuration = 5f;
        [SerializeField, Min(0)] private float m_RebornDelay = 1f;

        private static readonly CultureInfo s_Numbers = CultureInfo.GetCultureInfo("en-US");
        private enum AdPurpose { None, Revive, DoubleLoot }
        private AdPurpose m_Request;
        private LevelPlayService m_Ads;
        private bool m_Rewarded, m_Continuing, m_Doubled;
        private int m_Visit, m_DisplayedSecond;
        private float m_Remaining;
        private StageConfig m_Stage;
        private PlayerScore m_Score;
        private LootStorageManager m_Storage;
        private PlayerForceReborn m_Reborn;

        private void Awake()
        {
            m_Actions.revive.onClick.AddListener(RequestRevive);
            m_Actions.doubleLoot.onClick.AddListener(RequestDoubleLoot);
            m_Actions.replay.onClick.AddListener(RequestReplay);
            m_Actions.secondaryReplay.onClick.AddListener(RequestReplay);
            m_Actions.home.onClick.AddListener(ReturnHome);
            m_ReportShare.OpenButton.onClick.AddListener(OpenShare);
        }

        private void OnEnable()
        {
            m_Visit++;
            m_Continuing = m_Doubled = false;
            m_DisplayedSecond = -1;
            var player = RushPlayer.Instance;
            m_Stage = RushGameManager.Instance.StageManager.UsedStageConfig;
            m_Reborn = player.Reborn;
            m_Score = player.PlayerScore;
            m_Storage = GameManager.Instance.LootStorageManager;
            SetReviveTimeInternal(m_Reborn != null && m_Reborn.CanForceReborn ? m_ReviveDuration : 0f);
            DisplayHero(player.Unit?.Config);
            bool showScore = m_Stage != null && m_Stage.StageMode == StageMode.Collosal;
            m_Result.scoreColumn.SetActive(showScore);
            m_Result.experienceColumn.anchorMin = m_Result.experienceColumn.anchorMax =
                new Vector2(showScore ? .5f : 0f, 1f);
            m_Score.OnScoreCurrencyChanged.AddListener(OnScoreChanged);
            m_Score.OnExpFromHalvedScoreChanged.AddListener(OnExpChanged);
            OnScoreChanged(m_Score.ScoreCurrency);
            OnExpChanged(m_Score.ExpFromHalvedScore);
            m_Storage.OnLootedsChangedEvent.AddListener(OnLootChanged);
            RefreshActions();
        }

        private void OnDisable()
        {
            m_Visit++;
            if (m_Score != null)
            {
                m_Score.OnScoreCurrencyChanged.RemoveListener(OnScoreChanged);
                m_Score.OnExpFromHalvedScoreChanged.RemoveListener(OnExpChanged);
            }
            if (m_Storage != null) m_Storage.OnLootedsChangedEvent.RemoveListener(OnLootChanged);
            m_Request = AdPurpose.None;
            m_Rewarded = false;
            m_Ads = null;
        }

        private void DisplayHero(UnitConfig hero)
        {
            var art = hero?.CollectibleField;
            var rarity = art?.RarityConfig;
            m_Hero.name.text = hero != null ? hero.BaseInfo.Name : string.Empty;
            m_Hero.rarity.text = rarity != null ? rarity.BaseInfo.Name : string.Empty;
            m_Hero.frame.color = m_Hero.rarityMark.color = rarity != null ? rarity.Color : Color.gray;
            m_Hero.rarityMark.enabled = rarity != null;
            m_Hero.splash.sprite = art?.SplashImage != null ? art.SplashImage : art?.Icon;
            m_Hero.splash.enabled = m_Hero.splash.sprite != null;
        }

        private void OnScoreChanged(Currency _) => m_Result.score.text = m_Score.Score.ToString("N0", s_Numbers);
        private void OnExpChanged(int amount) => m_Result.experience.text = "+" + amount.ToString("N0", s_Numbers);
        private void OnLootChanged(System.Collections.Generic.List<LootField> _) => RefreshActions();

        private void Update()
        {
            if (m_Request != AdPurpose.None)
            {
                if (m_Ads != null && m_Ads.HasActiveRequest) return;
                FinishAd();
                return;
            }
            if (m_Remaining <= 0) return;
            SetReviveTimeInternal(m_Reborn.CanForceReborn ? Mathf.Max(0f, m_Remaining - Time.unscaledDeltaTime) : 0f);
            if (m_Remaining == 0) RefreshActions();
            else DisplayCountdown();
        }

        private void SetReviveTimeInternal(float remaining)
        {
            m_Remaining = remaining;
            if (remaining <= 0f) RushPlayer.Instance.CardBonuses.ResetProgression();
        }

        private void DisplayCountdown()
        {
            m_Actions.timerFill.fillAmount = m_ReviveDuration > 0 ? m_Remaining / m_ReviveDuration : 0;
            int second = Mathf.CeilToInt(m_Remaining);
            if (m_DisplayedSecond == second) return;
            m_DisplayedSecond = second;
            m_Actions.seconds.text = second + "s";
        }

        private void RefreshActions()
        {
            bool available = m_Remaining > 0;
            bool unlocked = m_Request == AdPurpose.None;
            m_Actions.status.text = !unlocked ? "Waiting for ad" : available ? "Revive available" : "Revive expired";
            m_Actions.revive.gameObject.SetActive(available);
            m_Actions.replay.gameObject.SetActive(!available);
            m_Actions.secondaryReplay.gameObject.SetActive(available);
            m_Actions.revive.interactable = m_Actions.replay.interactable =
                m_Actions.secondaryReplay.interactable = m_Actions.home.interactable = unlocked;
            m_ReportShare.OpenButton.gameObject.SetActive(!available);
            m_ReportShare.OpenButton.interactable = unlocked && !m_Storage.IsTransferring;
            m_Actions.homeRow.anchorMin = new Vector2(available ? .5f : 0f, 0);
            m_Actions.homeRow.anchorMax = new Vector2(available ? 1f : .5f, 1);
            m_Actions.doubleLoot.gameObject.SetActive(!available && !m_Doubled && m_Storage.Looteds.Count > 0);
            m_Actions.doubleLoot.interactable = unlocked && !m_Storage.IsTransferring;
            DisplayCountdown();
        }

        private void OpenShare()
        {
            if (!isActiveAndEnabled || m_Request != AdPurpose.None || m_Remaining > 0 || m_Storage.IsTransferring || m_Stage == null) return;
            m_ReportShare.Show(RushPlayer.Instance.Unit?.Config, m_Stage.StageMode,
                RushGameManager.Instance.StageManager.BattleWaveNumber, m_Score.Score);
        }

        private void RequestRevive() => RequestAd(AdPurpose.Revive);
        private void RequestDoubleLoot() => RequestAd(AdPurpose.DoubleLoot);

        private void RequestAd(AdPurpose purpose)
        {
            if (!isActiveAndEnabled || m_Request != AdPurpose.None) return;
            if (purpose == AdPurpose.Revive && (m_Remaining <= 0 || !m_Reborn.CanForceReborn)) return;
            if (purpose == AdPurpose.DoubleLoot && (m_Remaining > 0 || m_Doubled || m_Storage.IsTransferring || m_Storage.Looteds.Count == 0)) return;
            var service = UnityService.Instance;
            m_Ads = service.UnityAdsManager.LevelPlayService;
            if (m_Ads == null || !m_Ads.isActiveAndEnabled || m_Ads.HasActiveRequest) return;
            int visit = m_Visit;
            m_Request = purpose;
            m_Rewarded = false;
            RefreshActions();
            service.ShowRewardedAd(() =>
            {
                if (this != null && m_Visit == visit && m_Request == purpose) m_Rewarded = true;
            });
        }

        private void FinishAd()
        {
            var purpose = m_Request;
            bool earned = m_Rewarded;
            m_Request = AdPurpose.None;
            m_Rewarded = false;
            if (earned && purpose == AdPurpose.Revive && m_Reborn.CanForceReborn)
            {
                m_Continuing = true;
                m_Reborn.SetCanForceReborn(false);
                m_Panel.Hide();
                m_Reborn.ReviveAfterReward(m_RebornDelay, m_RevivePanel.Show);
                return;
            }
            if (earned && purpose == AdPurpose.DoubleLoot && !m_Doubled)
            {
                m_Doubled = true;
                m_Storage.DoubleStoredLootsImmediate();
            }
            RefreshActions();
        }

        private void RequestReplay()
        {
            if (!isActiveAndEnabled || m_Request != AdPurpose.None || m_Stage == null) return;
            var config = m_Stage.EnergyConfig;
            int price = m_Stage.EnergyAmountToPay;
            if (config == null || price <= 0) { LeaveBattle(m_ReplayState); return; }
            int visit = m_Visit;
            bool CanReplay() => this != null && isActiveAndEnabled && m_Request == AdPurpose.None && m_Visit == visit;
            if (m_Stage.StageMode == StageMode.Collosal &&
                m_EnergyPanel.HasBinding<ColossalEnergyDialog>(out var dialog))
            {
                dialog.Open(m_Stage, CanReplay, () => LeaveBattle(m_ReplayState));
                return;
            }
            var owned = Player.Instance.EnergyController.GetEnergy(config);
            if (owned == null || !owned.CanPay(price))
            {
                m_EnergyPanel.SetWarningText(new[] { new Energy(config, price - (owned?.Amount ?? 0)) });
                return;
            }
            m_EnergyPanel.SetConfirmationText(new[] { new Energy(config, price) },
                _ => { if (this != null && m_Visit == visit) LeaveBattle(m_ReplayState); },
                missing => { if (this != null && m_Visit == visit) m_EnergyPanel.SetWarningText(missing); });
        }

        private void ReturnHome() => LeaveBattle(m_HomeState);

        private void LeaveBattle(GameStateConfig destination)
        {
            if (!isActiveAndEnabled || m_Request != AdPurpose.None) return;
            m_Panel.Hide();
            RushGameManager.Instance.GameStateManager.ChangeState(destination);
        }

        public void SettleBattleLoot() => SettleBattleLootInternal();
        private void SettleBattleLootInternal()
        {
            if (!m_Continuing && m_Storage != null) m_Storage.TakeLooteds();
        }
    }
}
