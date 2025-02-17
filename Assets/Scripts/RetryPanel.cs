using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RetryPanel : MonoBehaviour
{
    [SerializeField] Button retryBTN, homeBTN;

    private void Start()
    {
        homeBTN.onClick.AddListener(Home);
        retryBTN.onClick.AddListener(Retry);
    }
    public void Home()
    {
        int rand = Random.Range(0, 100);
        if (rand < 10)
        {
            Time.timeScale = 1;
            FindObjectOfType<GameManager>().DestroyPlayer();
            FindObjectOfType<GameManager>().KeGame();

        }

        else
        {
            Time.timeScale = 1;
            FindObjectOfType<GameManager>().DestroyPlayer();
            SceneManager.LoadScene("Game");
        }
    }
    public void Retry()
    {
        int rand = Random.Range(0, 100);
        if(rand < 70)
        {
            //IronsourceAdsManager.Instance.RequestRewardedAds("RestartGame");

        }
        else
        {
            ActivityPoint.Instance.AddOrSubActivityPoint(2);
            FindObjectOfType<GameManager>().PlayerAnim.GetComponent<Player>().SetToIdle();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }
    }
}
