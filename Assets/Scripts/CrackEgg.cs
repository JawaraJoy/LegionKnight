using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CrackEgg : MonoBehaviour
{
    public STATE state;

    public Button eggBTN,claimBTN,confirmBTN;

    public Slider timerSlider;

    public Text timerTeks;

    public float timer;

    public int claimedTime;

    public bool showedAds;

    public GameObject dailySpin,crackEggPanel,crackingPanel,rewardedCrackEggPanel,motherPanel,confirmPanel;

    public string serverAddress;

    public DateTime currentTime;
    public DateTime claimTime;
    private void Start()
    {
        eggBTN.onClick.AddListener(()=>motherPanel.SetActive(true));
        confirmBTN.onClick.AddListener(() => StartCoroutine(Cracking()));
        claimBTN.onClick.AddListener(Claim);
        timerSlider.maxValue = timer;
        state = STATE.CLAIMAVAILABLE;
        StartCoroutine(GetCurrentTime());
        showedAds = false;
    }
    #region GetTime

    private IEnumerator GetCurrentTime()
    {
        UnityWebRequest request = UnityWebRequest.Get(serverAddress);
        yield return request.SendWebRequest();

        switch (request.result)
        {
            case UnityWebRequest.Result.Success:
                string data = request.downloadHandler.text;
                currentTime = DateTime.Parse(ReturnStringFromWorldTimeApi(data));
                if (PlayerPrefs.HasKey("CrackEgg"))
                {
                    long temp = Convert.ToInt64(PlayerPrefs.GetString("CrackEgg"));
                    claimTime = DateTime.FromBinary(temp);
                    if (currentTime > claimTime)
                    {
                        int random1 = Mathf.FloorToInt(UnityEngine.Random.Range(0f, 1f));
                        if (random1 == 0)
                        {
                            if (claimedTime < 4)
                            {
                                state = STATE.CLAIMAVAILABLE;
                                crackEggPanel.SetActive(true);
                            }

                        }
                    }
                    else
                    {
                        state = STATE.CLAIMED;
                        crackEggPanel.SetActive(false);
                    }
                    if(currentTime.Day>claimTime.Day)
                    {
                        claimedTime = 0;
                    }
                }
                else
                {
                    state = STATE.CLAIMAVAILABLE;
                    crackEggPanel.SetActive(true);
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
    #endregion

    private void Update()
    {
        if(state == STATE.WAITING)
        {
            if (timer > 0)
            {
                timer -= 1 * Time.deltaTime;
                timerSlider.value = timer;
                timerTeks.text = $"Cracking {(int)timer}s";
            }
            else
            {
                timer = 0;
                confirmPanel.SetActive(false);
                crackingPanel.SetActive(false);
                rewardedCrackEggPanel.SetActive(true);
            }
        }
    }
    private IEnumerator Cracking()
    {
        state = STATE.WAITING;
        confirmPanel.SetActive(false);
        crackingPanel.SetActive(true);
        yield return new WaitForSeconds(5);
        if (!showedAds)
        {
            //IronsourceAdsManager.Instance.RequestRewardedAds("CrackEgg");

        }
    }

    private void Claim()
    {
        state = STATE.CLAIMED;
        claimTime = currentTime;
        claimTime = claimTime.AddHours(6);
        claimedTime++;
        PlayerPrefs.SetString("CrackEgg", claimTime.ToBinary().ToString());
        dailySpin.SetActive(true);
        motherPanel.SetActive(false);
        crackEggPanel.SetActive(false);
    }
}