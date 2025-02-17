using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HourlyReward : MonoBehaviour
{
    public float currentSeconds = 7200;
    public STATE state;
    public delegate void OnStateChange(STATE newstate);
    public OnStateChange onStateChange;
    public void SetState(STATE newstate)
    {
        state = newstate;
        onStateChange.Invoke(state);
    }

    public Sprite[] icons;

    public Button chestBTN,claimBTN;
    public Button cutTimeBTN;

    public Text timerTeks,timerTeks1;

    public GameObject dailySpin;
    public GameObject cutTimePanel;
    public GameObject rewardReadyPanel;
    
    public Image chestImg;
    public Image chestImg1;


    public string serverAddress;

    public DateTime currentTime;
    public DateTime claimTime;

    public bool cutTimeAds;
    private void OnEnable()
    {
        Actions.OpenGameWithNotif -= OpenGameWithNotif;
        Actions.OpenGameWithNotif += OpenGameWithNotif;
        onStateChange += StateChanged;
    }
    private void OnDisable()
    {
        onStateChange -= StateChanged;

    }
    private void StateChanged(STATE newstate)
    {
        switch (newstate)
        {
            case STATE.WAITING:
                break;
            case STATE.CLAIMAVAILABLE:
                currentSeconds = 0;
                timerTeks.text = "CLAIM REWARD!";
                timerTeks1.text = "";
                if (cutTimeAds)
                {
                    cutTimeAds = false;
                    PlayerPrefs.SetInt("CutTimeAds", 0);
                }
                chestImg1.sprite = icons[0];
                break;
            case STATE.CLAIMED:
                chestImg1.sprite = icons[1];
                TimeSpan interval = currentTime - claimTime;
                currentSeconds = (float)interval.TotalSeconds * -1;
                break;
            default:
                break;
        }
    }

    private void OpenGameWithNotif()
    {
        rewardReadyPanel.SetActive(true);
    }

    private void Start()
    {
        if(PlayerPrefs.HasKey("CutTimeAds"))
            cutTimeAds = PlayerPrefs.GetInt("CutTimeAds") == 1 ? true : false;
        else
        {
            cutTimeAds = false;
            PlayerPrefs.SetInt("CutTimeAds", 0);
        }

        chestBTN.onClick.AddListener(OpenConfirmPanel);
        claimBTN.onClick.AddListener(Claim);
        cutTimeBTN.onClick.AddListener(CutTime);
        SetState(STATE.WAITING);
        StartCoroutine(GetCurrentTime());
    }


    public IEnumerator GetCurrentTime()
    {
        UnityWebRequest request = UnityWebRequest.Get(serverAddress);
        yield return request.SendWebRequest();

        switch (request.result)
        {
            case UnityWebRequest.Result.Success:
                string data = request.downloadHandler.text;
                currentTime = DateTime.Parse(ReturnStringFromWorldTimeApi(data));
                if (PlayerPrefs.HasKey("HourlyReward"))
                {
                    long temp = Convert.ToInt64(PlayerPrefs.GetString("HourlyReward"));
                    claimTime = DateTime.FromBinary(temp);
                    if (currentTime>=claimTime)
                    {
                        SetState(STATE.CLAIMAVAILABLE);
                    }
                    else
                    {
                        SetState(STATE.CLAIMED);
                    }
                }
                else
                {
                    SetState(STATE.CLAIMAVAILABLE);
                }
                break;
            case UnityWebRequest.Result.ConnectionError:
                StartCoroutine(GetCurrentTime());
                break;
            case UnityWebRequest.Result.ProtocolError:
                StartCoroutine(GetCurrentTime());
                break;
            case UnityWebRequest.Result.DataProcessingError:
                StartCoroutine(GetCurrentTime());
                break;
            default:
                break;
        }
    }

    private string ReturnStringFromWorldTimeApi(string data)
    {
        MyCustomDateTimeClass myObject = new MyCustomDateTimeClass();

        myObject = JsonUtility.FromJson<MyCustomDateTimeClass>(data);

        return myObject.datetime;
    }
    private void Claim()
    {
        claimBTN.interactable = false;
        //#region test editor

        //SetState(STATE.CLAIMED);
        //chestBTN.GetComponent<Image>().sprite = icons[1];
        //claimTime = currentTime;
        //claimTime = claimTime.AddHours(2);
        //StartCoroutine(GetCurrentTime());
        //PlayerPrefs.SetString("HourlyReward", claimTime.ToBinary().ToString());
        //dailySpin.SetActive(true);
        //chestBTN.interactable = true;
        //claimBTN.interactable = true;
        //cutTimePanel.SetActive(false);
        //rewardReadyPanel.SetActive(false);
        //#endregion
        
    }

    private void OpenConfirmPanel()
    {
        
    }
    private void CutTime()
    {
        cutTimePanel.SetActive(false);
        //#region test editor
        //cutTimeAds = true;
        //PlayerPrefs.SetInt("CutTimeAds", 1);
        //#endregion
        if (!cutTimeAds)
        {
            //IronsourceAdsManager.Instance.RequestRewardedAds("CutTime");
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case STATE.CLAIMED:
                currentSeconds -= Time.deltaTime;

                // Menghitung jam, menit, dan detik
                int hours = Mathf.FloorToInt(currentSeconds / 3600);
                int minutes = Mathf.FloorToInt((currentSeconds % 3600) / 60);
                int seconds = Mathf.FloorToInt(currentSeconds % 60);

                // Format waktu menjadi teks
                string timerText = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

                timerTeks.text = timerText;
                timerTeks1.text = timerText;
                if (currentSeconds <= 0)
                {
                    SetState(STATE.CLAIMAVAILABLE);
                }    
                break;
            default:
                break;
        }
    }
}
public enum STATE
{
    WAITING,
    CLAIMAVAILABLE,
    CLAIMED
}