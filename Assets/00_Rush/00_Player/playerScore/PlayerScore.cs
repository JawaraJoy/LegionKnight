using LegionKnight;
using System.Collections;
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

        private const string c_HighScoreResetKey = "rank.runBest.v1.reset";
        private bool m_HighScoreReady;

        private IEnumerator Start()
        {
            while (Player.CurrentWallet == null || UnityService.Instance == null || UnityService.Instance.LocalSave == null)
                yield return null;
            PrepareHighScoreInternal();
        }

        private void PrepareHighScoreInternal()
        {
            if (m_HighScoreReady || m_ItemConsiderAsScoreConfig == null) return;
            var save = UnityService.Instance.LocalSave;
            if (!save.HasData(c_HighScoreResetKey))
            {
                save.ExecuteTransaction(() =>
                {
                    Player.Instance.CurrencyControl.SetCurrencyAmount(m_ItemConsiderAsScoreConfig, 0);
                    save.SaveData(c_HighScoreResetKey, true);
                });
            }
            m_HighScoreReady = true;
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

            m_Score += score;
            SeasonalRanks.Current?.Record(m_Score);
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
            PrepareHighScoreInternal();
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
