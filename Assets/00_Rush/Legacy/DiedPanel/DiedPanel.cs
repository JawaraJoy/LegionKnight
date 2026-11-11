using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string DiedPanel => "Died";
    }
    public class DiedPanel : PanelView
    {
        [SerializeField]
        private Currency m_CoinToPay;
        public override string UniqueId => PanelId.DiedPanel;
        [SerializeField]
        private float m_CountdownTime = 5f;
        [SerializeField]
        private TextMeshProUGUI m_CountdownText;
        [SerializeField]
        private Button m_WatchButton;
        [SerializeField]
        private Button m_PayCoinButton;
        [SerializeField]
        private Button m_CancelButton;
        private void Awake()
        {
            m_WatchButton.onClick.AddListener(WatchAds);
            m_CancelButton.onClick.AddListener(Cancel);
            m_PayCoinButton.onClick.AddListener(PayCoin);
        }

        private IEnumerator Countingdown()
        {
            float time = m_CountdownTime;
            m_WatchButton.interactable = true;
            while (time > 0)
            {
                Debug.Log($"Count Down {time}");
                time -= Time.unscaledDeltaTime;
                m_CountdownText.text = Mathf.CeilToInt(time).ToString();
                yield return null;
            }
            m_CountdownText.text = "0";
            yield return new WaitForEndOfFrame();
            m_WatchButton.interactable = false;
        }
        protected override void OnShowInvoke()
        {
            base.OnShowInvoke();
            bool canUseRessurection = Player.Instance.CanUseResurrectionAds;
            if (canUseRessurection)
            {
                GameManager.Instance.StartCoroutine(Countingdown());
            }
            else
            {
                HideInternal();
                CanvasManager.Instance.GetPanel<GameOverPanel>().Show();
            }

            //Player.Instance.SetPause(true);
        }
        private void Ressurection()
        {
            //GameManager.Instance.LevelDefinition.StartLevel();
            StartCoroutine(Ressurectioning());
        }
        private IEnumerator Ressurectioning()
        {
            HideInternal();
            RevivePanel panel = CanvasManager.Instance.GetPanel<RevivePanel>();
            if (panel != null)
            {
                panel.Show();
            }
            yield return new WaitForSeconds(1);
            //GameManager.Instance.RessurectionPlayer();
            Player.Instance.SetCanUseResurrectionAds(false);
        }
        protected override void OnHideInvoke()
        {
            base.OnHideInvoke();
            GameTimeScale.SetTimeScale(1);

            //Player.Instance.SetPause(false);
        }
        private void WatchAds()
        {
            UnityService.Instance.ShowRewardedAd(Ressurection);
        }
        private void Cancel()
        {
            HideInternal();
            CanvasManager.Instance.GetPanel<GameOverPanel>().Show();
        }

        private void PayCoin()
        {
            int playerCoinAmount = Player.Instance.CurrencyControl.GetCurrencyAmount(m_CoinToPay.ItemConfig);
            if (playerCoinAmount >= m_CoinToPay.Amount)
            {
                Ressurection();
                HideInternal();
                Player.Instance.CurrencyControl.AddCurrencyAmount(m_CoinToPay.ItemConfig, m_CoinToPay.Amount);
            }
            else
            {
                CanvasManager.Instance.GetPanel<TextPopUpPanel>().ShowText("Not enough Coin");
            }
        }
    }
}
