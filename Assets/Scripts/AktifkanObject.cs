using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class AktifkanObject : MonoBehaviour
{
    public GameObject Obj;
    public CountTimer countTimer;
    //public GameObject RetryBTN, ReviveBTN;
    public bool isShowAds;
    // Start is called before the first frame update
    void Start()
    {
        Obj.SetActive(true);
    }
    private void OnEnable()
    {
        isShowAds = false;
    }
    private void OnDisable()
    {
        isShowAds = false;
    }
    // Update is called once per frame
    void Update()
    {
        /*if(countTimer.currentTime<=0)
        {
            if(FindObjectOfType<GameManager>().ModeGamePlay == 0)
            {
                if(!isShowAds)
                {
                    //countTimer.currentTime = 7;
                    //countTimer.enabled = false;
                    //ReviveBTN.SetActive(false);
                    //IronsourceAdsManager.Instance.RequestRewardedAds("Revive");
                    
                    //Time.timeScale = 0;
                    //isShowAds = true;
                }
            }
            else if(FindObjectOfType<GameManager>().ModeGamePlay == 1)
            {
                if(!isShowAds)
                {
                    //countTimer.currentTime = 7;
                    //countTimer.enabled = false;
                    //IronsourceAdsManager.Instance.RequestRewardedAds("Revive");
                    //Time.timeScale = 0;
                    //isShowAds = true;
                }
                //Time.timeScale = 0;
            }
        }*/
    }
    public void Retry()
    {
        
        ActivityPoint.Instance.AddOrSubActivityPoint(2);
        FindObjectOfType<GameManager>().PlayerAnim.GetComponent<Player>().SetToIdle();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
