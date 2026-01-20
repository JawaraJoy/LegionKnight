using Rush;
using System.Collections;
using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public class DiedPanel : PanelView
    {
        [SerializeField]
        private float m_CountdownTime = 5f;
        [SerializeField]
        private TextMeshProUGUI m_CountdownText;

        private IEnumerator Countingdown()
        {

            float time = m_CountdownTime;

            while (time > 0)
            {
                Debug.Log($"Count Down {time}");
                time -= Time.deltaTime;
                m_CountdownText.text = Mathf.CeilToInt(time).ToString();
                yield return null;
            }
            m_CountdownText.text = "0";
            yield return new WaitForEndOfFrame();
        }
        protected override void ShowInternal()
        {
            base.ShowInternal();
            bool canUseRessurection = Player.Instance.CanUseResurrectionAds;
            if (canUseRessurection)
            {
                StartCoroutine(Countingdown());
            }
            else
            {
                HideInternal();
                CanvasManager.Instance.GetPanel<GameOverPanel>().Show();
            }

            Player.Instance.SetPause(true);
        }
    }
}
