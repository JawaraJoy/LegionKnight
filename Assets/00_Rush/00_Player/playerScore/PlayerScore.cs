using LegionKnight;
using System.Collections;
using Unity.Services.Authentication;
using Unity.Services.Core;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class PlayerScore : MonoBehaviour, IReseter
    {
        [SerializeField]
        private ItemConfig m_ItemConsiderAsScoreConfig;
        [SerializeField, MMReadOnly]
        private int m_Score = 0;
        [SerializeField]
        private int m_ExpFromHalvedScore = 0;
        [SerializeField]
        private UnityEvent<int> m_OnScoreChanged;
        [SerializeField]
        private UnityEvent<Currency> m_OnScoreCurrencyChanged;
        [SerializeField]
        private UnityEvent<int> m_OnExpFromHalvedScoreChanged;
        public UnityEvent<Currency> OnScoreCurrencyChanged => m_OnScoreCurrencyChanged;
        public UnityEvent<int> OnExpFromHalvedScoreChanged => m_OnExpFromHalvedScoreChanged;

        private Currency m_ScoreCurrency;
        public Currency ScoreCurrency => m_ScoreCurrency;
        public int Score => m_Score;
        public int ExpFromHalvedScore => m_ExpFromHalvedScore;

        private IEnumerator Start()
        {
            // Recover a best score saved by versions without a durable submission queue.
            while (UnityServices.State != ServicesInitializationState.Initialized || !AuthenticationService.Instance.IsSignedIn)
                yield return null;
            if (m_ItemConsiderAsScoreConfig == null) yield break;
            var service = UnityService.Instance;
            string scoreKey = "amount" + m_ItemConsiderAsScoreConfig.BaseInfo.Id;
            int legacy = service.HasData(scoreKey) ? service.GetData<int>(scoreKey) : 0;
            SeasonalRanks.Current?.InitializeLegacy(legacy);
            int initial = SeasonalRanks.Current != null ? SeasonalRanks.Current.LegacySeed : legacy;
            if (initial > 0) service.SubmitScoreEntry(initial);
        }





private void SetExpInternal(int exp)
        {
            m_ExpFromHalvedScore = exp;
            OnExpFromHalvedScoreChangedInvoke(m_ExpFromHalvedScore);
            Debug.Log(gameObject.name + m_ExpFromHalvedScore);
        }
        private void AddPlayerExpFromHalvedScoreInternal(int score)
        {
            int halvedScoreToAdd = Mathf.FloorToInt(score / 2f + 1);
            SetExpInternal(m_ExpFromHalvedScore + halvedScoreToAdd);
            Player.Instance.AddPlayerExperience(halvedScoreToAdd);
        }
        public void AddPlayerExpFromHalvedScore(int score)
        {
            AddPlayerExpFromHalvedScoreInternal(score);
        }
private void AddScoreInternal(int score)
        {
            AddPlayerExpFromHalvedScoreInternal(score);
            StageConfig currenStage = RushGameManager.Instance.StageManager.UsedStageConfig;
            if (currenStage.StageMode != StageMode.Collosal) return;

            int bestBeforeBattle = Player.Instance.CurrencyControl.GetCurrencyAmount(m_ItemConsiderAsScoreConfig);
            SeasonalRanks.Current?.Record(score, bestBeforeBattle);
            m_Score += score;
            OnScoreChangedInvoke(m_Score);
            m_ScoreCurrency = new Currency(m_ItemConsiderAsScoreConfig, m_Score);
            OnScoreCurrencyChangedInvoke(m_ScoreCurrency);
            ApplyHighScoreToPlayerInternal();
        }
        public void AddScore(int score)
        {
            AddScoreInternal(score);
        }
        private void ApplyHighScoreToPlayerInternal()
        {
            int playerHighScore = Player.Instance.CurrencyControl.GetCurrencyAmount(m_ItemConsiderAsScoreConfig);
            int bestScore = Mathf.Max(m_Score, playerHighScore);
            // Persist delivery intent before committing the local record. Equal scores can recover a previous failure.
            if (SeasonalRanks.Current == null) UnityService.Instance.SubmitScoreEntry(bestScore);
            if (m_Score > playerHighScore)
            {
                Player.Instance.CurrencyControl.SetCurrencyAmount(m_ItemConsiderAsScoreConfig, m_Score);
            }
        }
        public void ApplyHighScoreToPlayer()
        {
            ApplyHighScoreToPlayerInternal();
        }

private void SetHighScoreInternal(int score)
        {
            SetExpInternal(score);
            m_Score = score;
            OnScoreChangedInvoke(m_Score);
        }

private void OnScoreChangedInvoke(int score)
        {
            m_OnScoreChanged?.Invoke(score);
        }

        private void OnScoreCurrencyChangedInvoke(Currency currency)
        {
            m_OnScoreCurrencyChanged?.Invoke(currency);
        }

        private void OnExpFromHalvedScoreChangedInvoke(int exp)
        {
            m_OnExpFromHalvedScoreChanged?.Invoke(exp);
        }

        public void SetHighScore(int score)
        {
            SetHighScoreInternal(score);
        }

        public void ResetProgression()
        {
            SetHighScoreInternal(0);
        }
    }
    public partial class RushPlayer
    {
        [SerializeField]
        private PlayerScore m_PlayerScore;
        public PlayerScore PlayerScore => m_PlayerScore;
    }
}
