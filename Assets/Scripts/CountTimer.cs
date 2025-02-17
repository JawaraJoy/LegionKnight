using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountTimer : MonoBehaviour
{
    public float currentTime;
    public float startingTime;

    public GameObject GameManagerObject;

    [SerializeField] Text countdownText;

    //hanya untuk di hide saja
    public GameObject TextTimer;
    public Text TextCoin;

    //public string placementID;
    //public Button BTN;
    // Start is called before the first frame update
    void Start()
    {
        currentTime = startingTime;
        GameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
        TextTimer.SetActive(true);
        currentTime = 7;
        /*BTN.onClick.AddListener(() =>
        {
            //IronsourceAdsManager.Instance.RequestRewardedAds(placementID);
            Time.timeScale = 0;
        });*/
    }
    private void OnEnable()
    {
        int[] CoinArray = new int[3];
        CoinArray[0] = 10;
        CoinArray[1] = 25;
        CoinArray[2] = 50;
        int rand = Random.Range(0, CoinArray.Length);
        TextCoin.text = CoinArray[rand].ToString() + " Extra Coin";

        //IronsourceAdsManager.Instance.Coin = CoinArray[rand];
        //currentTime = 7;
    }
    // Update is called once per frame
    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        countdownText.text = currentTime.ToString("0");
        if (currentTime <= 0)
        {
            currentTime = 0;
        }
    }
    //tidak stop
    public void StopTimer()
    {

        TextTimer.SetActive(false);
        currentTime = 7;
    }

    public void OpenTimer()
    {
        TextTimer.SetActive(true);
        currentTime = 7;
    }
}
