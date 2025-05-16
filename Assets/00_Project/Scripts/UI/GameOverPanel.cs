using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string GameOverPanelId = "GameOver";
    }
    public partial class GameOverPanel : PanelView
    {
        [SerializeField]
        private string m_GameplaySceneName = "ReworkGameplay";
        public override string UniqueId => PanelId.GameOverPanelId;
        [SerializeField]
        private Button m_RessurectionButton;
        [SerializeField]
        private Button m_PlayAgainButton;
        [SerializeField]
        private Button m_HomeButton;
        [SerializeField]
        private float m_CountdownTime = 5f;
        [SerializeField]
        private TextMeshProUGUI m_CountdownText;

        private void Awake()
        {
            m_RessurectionButton.onClick.AddListener(WatchAds);
            m_PlayAgainButton.onClick.AddListener(StoreLevelScoreInternal);
            m_HomeButton.onClick.AddListener(StoreLevelScoreInternal);
        }
        public void PlayAgain()
        {
            //GameManager.Instance.LoadScene(m_GameplaySceneName);
            GameManager.Instance.Play();
        }
        protected override void OnShowInvoke()
        {
            base.OnShowInvoke();
            //GameTimeScale.SetTimeScale(0);
            UnityService.Instance.LoadRewardedAd();

            bool canUseRessurection = Player.Instance.CanUseResurrectionAds;
            if (canUseRessurection)
            {
                StartCoroutine(Countingdown());
            }
            m_RessurectionButton.gameObject.SetActive(canUseRessurection);

            
            Player.Instance.SetPause(true);
        }
        protected override void OnHideInvoke()
        {
            base.OnHideInvoke();
            GameTimeScale.SetTimeScale(1);
            
            Player.Instance.SetPause(false);
        }

        private void StoreLevelScoreInternal()
        {
            GameManager.Instance.StoreLevelScore();
        }

        public void Ressurection()
        {
            //GameManager.Instance.LevelDefinition.StartLevel();
            GameManager.Instance.RessurectionPlayer();
            HideInternal();
            Player.Instance.SetCanUseResurrectionAds(false);
        }

        private void WatchAds()
        {
            UnityService.Instance.ShowRewardedAd(Ressurection);
        }

        private IEnumerator Countingdown()
        {
            
            float time = m_CountdownTime;
            m_RessurectionButton.gameObject.SetActive(true);
            
            while (time > 0)
            {
                Debug.Log( $"Count Down {time}");
                time -= Time.deltaTime;
                m_CountdownText.text = Mathf.CeilToInt(time).ToString();
                yield return null;
            }
            m_CountdownText.text = "0";
            yield return new WaitForSeconds(1f);
            m_RessurectionButton.gameObject.SetActive(false);
        }
    }
}
