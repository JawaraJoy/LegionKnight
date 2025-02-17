using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using AppsDaddyO.TimeMan;
using TMPro;
//using AppsFlyerSDK;
using com.adjust.sdk;

public class DailyReward : MonoBehaviour
{
    public RewardSlotClass[] reward;
    public RewardSlotClass[] rewardsWeekData;
    public int totalReward;
    public GameObject rewardPanelSlot,rewardClaimedPanel,dailySpinPanel;
    public Button claimBTN,closeBTN,closeWindBTN,openDailyReward,resetBTN;
    public Button nextday;
    public TMP_Dropdown selectPriority;

    public bool isClaimAble;
    public delegate void OnClaimable(bool claim);
    public OnClaimable claimAble;

    public LastDateClaim lastDateCLaim;
    public int today;
    public bool login;

    public TMP_Text uiText;

    public int totalProbability;

    public static DailyReward instance;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        rewardsWeekData = new RewardSlotClass[totalReward];
        for (int i = 0; i < rewardsWeekData.Length; i++)
        {
            rewardsWeekData[i] = new RewardSlotClass();
        }
        for (int i = 0; i < reward.Length; i++)
        {
            totalProbability += reward[i].GetAmount();
        }

        claimBTN.onClick.AddListener(claim);
        closeWindBTN.onClick.AddListener(() =>
        {
            TimeManager.timeMan.rewardPanel.SetActive(false);
            openDailyReward.gameObject.SetActive(true);
        });
        openDailyReward.onClick.AddListener(() =>
        {
            TimeManager.timeMan.rewardPanel.SetActive(true);
            openDailyReward.gameObject.SetActive(false);
        });
        resetBTN.onClick.AddListener(() =>
        {
            DataManager.instance.deletdata();
            LoadNewRewardData();
        });
        selectPriority.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(selectPriority);
        });
        nextday.onClick.AddListener(nexday);
        claimAble += onClaim;
        IsiDropdown();
    }
    TimeSpan interval;
    private void Update()
    {
        if(!isClaimAble)
        {
            StartCoroutine(TimeManager.timeMan.GetCurrentTime());
            uiText.text = "Wait for " + (TimeManager.theCurrentTime.Hour - 24) * -1 + ":" + (TimeManager.theCurrentTime.Minute - 60) * -1 + ":" + (TimeManager.theCurrentTime.Second-60)*-1 + " for your next reward!";
            /*if (LanguageManager.instance.GetLanguageIndex() == 0)
            {
                
            }
            else if (LanguageManager.instance.GetLanguageIndex() == 1)
            {
                uiText.text = "Tunggu " + (TimeManager.theCurrentTime.Hour - 24) * -1 + ":" + (TimeManager.theCurrentTime.Minute-60)*-1 + ":" + (TimeManager.theCurrentTime.Second-60)*-1 + " untuk hadiah selanjutnya!";

            }*/
        }
    }
    private void IsiDropdown()
    {
        List<string> options = new List<string>();

        // Add some example options to the list
        options.Add("Low Priority");
        options.Add("High Priority");

        // Add the options list to the Dropdown
        selectPriority.AddOptions(options);
    }

    public void kosongan()
    {
        rewardsWeekData = new RewardSlotClass[totalReward];
        for (int i = 0; i < rewardsWeekData.Length; i++)
        {
            rewardsWeekData[i] = new RewardSlotClass();
        }

    }

    private void nexday()
    {
        if (today >= totalReward)
        {
            DataManager.instance.deletdata();
            LoadNewRewardData();
        }
        else
        {
            today++;
        }
        IsDayClaim(true);
        RefreshUI();
    }

    private void OnEnable()
    {
        claimAble += onClaim;

    }
    private void OnDisable()
    {
        claimAble -= onClaim;
    }

    public void IsDayClaim(bool claim)
    {
        isClaimAble = claim;
        claimAble?.Invoke(isClaimAble);
    }
    private void onClaim(bool claim)
    {
        claimBTN.gameObject.SetActive(claim);
        closeBTN.gameObject.SetActive(!claim);
    }

    public void CheckRewardData()
    {
        DataContainer data = DataManager.instance.loaddata();
        
        if (data == null)
        {
            LoadNewRewardData();
        }
        else
        {
            LoadRewardData(data);
        }
    }
    public void LoadNewRewardData()
    {
        #region Priority Check
        DataContainer activityTimeData = DataManager.instance.LoadActivityTime();
        if (activityTimeData != null)
        {
            if (activityTimeData.activityPoint >= 80)
            {
                for (int i = 0; i < reward.Length; i++)
                {
                    reward[i].changePriority(RewardSlotClass.PRIORITY.High);
                }
                selectPriority.value = 1;
                DropdownValueChanged(selectPriority);
            }
            else if (activityTimeData.activityPoint < 80)
            {
                for (int i = 0; i < reward.Length; i++)
                {
                    reward[i].changePriority(RewardSlotClass.PRIORITY.Low);
                }
                Debug.Log("else");
                selectPriority.value = 0;
                DropdownValueChanged(selectPriority);
            }
        }
        else
        {
            for (int i = 0; i < reward.Length; i++)
            {
                reward[i].changePriority(RewardSlotClass.PRIORITY.High);
            }
            selectPriority.value = 1;
            DropdownValueChanged(selectPriority);
            ActivityPoint.Instance.AddOrSubActivityPoint(1);
        }
        #endregion
        kosongan();
        lastDateCLaim.day = TimeManager.theCurrentTime.Day;
        lastDateCLaim.month = TimeManager.theCurrentTime.Month;
        lastDateCLaim.year = TimeManager.theCurrentTime.Year;
        today = 0;
        IsDayClaim(true);
        
        for (int i = 0; i < totalReward; i++)
        {
            rewardclass a = GenerateByPercentage();
            AddRewardWeekData(a);
            //rewardsWeekData[i] = reward[num];
        }
        SaveRewardData();
    }

    private rewardclass GenerateByPercentage()
    {
        int random = UnityEngine.Random.Range(0, totalProbability);

        // select item based on their probability
        foreach (var item in reward)
        {
            if (random < item.GetAmount())
            {
                return item.GetRewardclass();
            }
            random -= item.GetAmount();
        }

        // return last item if none were selected
        return reward[0].GetRewardclass();
    }

    private bool AddRewardWeekData(rewardclass _item)
    {

        for (int i = 0; i < rewardsWeekData.Length; i++)
        {
            if (rewardsWeekData[i].GetRewardclass() == null)
            {
                rewardsWeekData[i].add(_item);
                break;
            }
            else
                continue;
        }


        RefreshUI();
        return true;
    }

    public void RefreshUI()
    {
        for (int i = 0; i < rewardPanelSlot.transform.childCount; i++)
        {
            try
            {
                rewardPanelSlot.transform.GetChild(i).GetChild(1).GetComponent<Text>().resizeTextForBestFit = false;
                rewardPanelSlot.transform.GetChild(i).GetChild(1).GetComponent<Text>().fontSize = 46;
                rewardPanelSlot.transform.GetChild(i).GetChild(1).GetComponent<Text>().alignment = TextAnchor.UpperCenter;
                rewardPanelSlot.transform.GetChild(i).GetChild(2).GetComponent<Image>().sprite = rewardsWeekData[i].GetRewardclass().image;
                rewardPanelSlot.transform.GetChild(i).GetChild(3).GetComponent<Text>().text = rewardsWeekData[i].GetRewardclass().amount.ToString();
                rewardPanelSlot.transform.GetChild(i).localScale = new Vector3(0.65f, 0.65f, 0);
                if (i == today)
                    if (isClaimAble)
                    {
                        rewardPanelSlot.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = Color.green;
                        uiText.text = "Ready To Claim";
                        /*if (LanguageManager.instance.GetLanguageIndex() == 0)
                        {
                            
                        }
                        else if (LanguageManager.instance.GetLanguageIndex() == 1)
                        {
                            uiText.text = "Siap Diambil";

                        }*/
                        rewardPanelSlot.transform.GetChild(i).GetChild(4).gameObject.SetActive(false);
                    }
                    else
                    {
                        rewardPanelSlot.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = Color.black;
                        uiText.text = "Wait for " + (TimeManager.theCurrentTime.Hour - 24) * -1 + ":" + TimeManager.theCurrentTime.Minute + ":" + TimeManager.theCurrentTime.Second + " for your next reward!";

                        /*if(LanguageManager.instance.GetLanguageIndex()==0)
                        {
                            uiText.text = "Wait for " + (TimeManager.theCurrentTime.Hour - 24) * -1 + ":" + TimeManager.theCurrentTime.Minute + ":" + TimeManager.theCurrentTime.Second + " for your next reward!";
                        }
                        else if(LanguageManager.instance.GetLanguageIndex() == 1)
                        {
                            uiText.text = "Tunggu " + (TimeManager.theCurrentTime.Hour - 24) * -1 + ":" + TimeManager.theCurrentTime.Minute + ":" + TimeManager.theCurrentTime.Second + " untuk hadiah selanjutnya!";

                        }*/
                        rewardPanelSlot.transform.GetChild(i).GetChild(4).gameObject.SetActive(true);
                    }
                else if (i > today)
                {
                    rewardPanelSlot.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color(197, 197, 197, 255);
                    rewardPanelSlot.transform.GetChild(i).GetChild(4).gameObject.SetActive(false);
                }
                else if (i < today)
                {
                    rewardPanelSlot.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = Color.black;
                    rewardPanelSlot.transform.GetChild(i).GetChild(4).gameObject.SetActive(true);
                }
            }
            catch
            {
                rewardPanelSlot.transform.GetChild(i).GetChild(1).GetComponent<Text>().resizeTextForBestFit = true;
                rewardPanelSlot.transform.GetChild(i).GetChild(1).GetComponent<Text>().fontSize = 168;
                rewardPanelSlot.transform.GetChild(i).GetChild(1).GetComponent<Text>().alignment = TextAnchor.UpperLeft;
                rewardPanelSlot.transform.GetChild(i).GetChild(2).GetComponent<Image>().sprite = null;
                rewardPanelSlot.transform.GetChild(i).GetChild(3).GetComponent<Text>().text = "Not Found";
                /*if (LanguageManager.instance.GetLanguageIndex() == 0)
                {
                    
                }
                else if (LanguageManager.instance.GetLanguageIndex() == 1)
                {
                    rewardPanelSlot.transform.GetChild(i).GetChild(3).GetComponent<Text>().text = "Tidak Ditemukan";

                }*/
                rewardPanelSlot.transform.GetChild(i).localScale = Vector3.one;
                rewardPanelSlot.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color(197,197,197,255);
                uiText.text = "error";
                rewardPanelSlot.transform.GetChild(i).GetChild(4).gameObject.SetActive(false);

            }
        }
    }
    public string[] pemisah;
    public void claim()
    {
        if (today == totalReward-1)
        {
            LoadNewRewardData();
        }
        //IronsourceAdsManager.Instance.RequestRewardedAds("Fortune_Wheeler");
        //rewardsWeekData[today].GetRewardclass()._state = rewardclass.STATE.Claimed;
        switch (rewardsWeekData[today].GetRewardclass()._Type)
        {
            case rewardclass.REWARD_TYPE.Coin:
                rewardClaimedPanel.SetActive(true);
                rewardClaimedPanel.transform.GetChild(3).GetComponent<Image>().sprite = rewardsWeekData[today].GetRewardclass().image;
                pemisah = rewardsWeekData[today].GetRewardclass().nama.Split(" ");
                rewardClaimedPanel.transform.GetChild(4).GetComponent<Text>().text = "You Got : " + rewardsWeekData[today].GetRewardclass().amount + " " + pemisah[0];
                FindObjectOfType<GameManager>().Coins += rewardsWeekData[today].GetRewardclass().amount;
                PlayerPrefs.SetInt("Coin", FindObjectOfType<GameManager>().Coins);
                break;
            case rewardclass.REWARD_TYPE.Diamond:
                rewardClaimedPanel.SetActive(true);
                rewardClaimedPanel.transform.GetChild(3).GetComponent<Image>().sprite = rewardsWeekData[today].GetRewardclass().image;
                pemisah = rewardsWeekData[today].GetRewardclass().nama.Split(" ");
                rewardClaimedPanel.transform.GetChild(4).GetComponent<Text>().text = "You Got : " + rewardsWeekData[today].GetRewardclass().amount + " " + pemisah[0];
                FindObjectOfType<GameManager>().Diamond += rewardsWeekData[today].GetRewardclass().amount;
                PlayerPrefs.SetInt("Diamond", FindObjectOfType<GameManager>().Diamond);
                break;
            case rewardclass.REWARD_TYPE.Fortune_Wheeler:
                instance.rewardClaimedPanel.SetActive(true);
                instance.rewardClaimedPanel.transform.GetChild(3).GetComponent<Image>().sprite = instance.rewardsWeekData[instance.today].GetRewardclass().image;
                instance.pemisah = instance.rewardsWeekData[instance.today].GetRewardclass().nama.Split(" ");
                instance.rewardClaimedPanel.transform.GetChild(4).GetComponent<Text>().text = "You Got : " + instance.rewardsWeekData[instance.today].GetRewardclass().amount + " " + instance.pemisah[0];

                break;
            default:
                break;
        }

        lastDateCLaim.day = TimeManager.theCurrentTime.Day;
        lastDateCLaim.month = TimeManager.theCurrentTime.Month;
        lastDateCLaim.year = TimeManager.theCurrentTime.Year;

        //Dictionary<string, string> dailyRewardEvent = new Dictionary<string, string>();
        //dailyRewardEvent.Add(AFInAppEvents.CONTENT_VIEW, "Daily Reward");
        //dailyRewardEvent.Add(AFInAppEvents.DATE_A, lastDateCLaim.ToString());
        //AppsFlyer.sendEvent("af_DailyReward", dailyRewardEvent);
        AdjustEvent dailyReward = new AdjustEvent("33f9df");
        dailyReward.addCallbackParameter("Daily Reward","Claimed");
        Adjust.trackEvent(dailyReward);

        ActivityPoint.Instance.AddOrSubActivityPoint(1);

        IsDayClaim(false);
        RefreshUI();
        SaveRewardData();
    }

    public RewardSlotClass contains(rewardclass item)
    {
        for (int i = 0; i < rewardsWeekData.Length; i++)
        {
            if (rewardsWeekData[i].GetRewardclass()==item)
            {
                return rewardsWeekData[i];
            }
        }
        return null;

    }
    public void LoadRewardData(DataContainer _data)
    {
        for (int i = 0; i < rewardsWeekData.Length; i++)
        {
            AddRewardWeekData(Resources.Load<rewardclass>(_data.nama[i]));
        }
        lastDateCLaim.day = _data.day;
        lastDateCLaim.month = _data.month;
        lastDateCLaim.year = _data.year;
        today = _data.today;
        if (TimeManager.theCurrentTime.Day>lastDateCLaim.day||TimeManager.theCurrentTime.Month>lastDateCLaim.month||TimeManager.theCurrentTime.Year>lastDateCLaim.year)
        {
            today++;
            ActivityPoint.Instance.AddOrSubActivityPoint(1); //login + 1
            IsDayClaim(true);
        }
        else if(TimeManager.theCurrentTime.Day <= lastDateCLaim.day || TimeManager.theCurrentTime.Month <= lastDateCLaim.month || TimeManager.theCurrentTime.Year <= lastDateCLaim.year)
        {
            IsDayClaim(_data.claim);
        }
        else if (TimeManager.theCurrentTime.Day >= lastDateCLaim.day+3 || TimeManager.theCurrentTime.Month >= lastDateCLaim.month+3 || TimeManager.theCurrentTime.Year >= lastDateCLaim.year+3)
        {
            login = false;
            ActivityPoint.Instance.AddOrSubActivityPoint(-12); //kalo ga login -12
            SaveRewardData();
        }


        RefreshUI();
    }
    void SaveRewardData() 
    {
        string[] nama = new string[rewardsWeekData.Length];
        for (int i = 0; i < rewardsWeekData.Length; i++)
        {
            nama[i] = rewardsWeekData[i].GetRewardclass().nama;
        }
        DataManager.instance.SaveData(nama, lastDateCLaim.day,lastDateCLaim.month,lastDateCLaim.year, isClaimAble,today,login);
    }
    private void DropdownValueChanged(TMP_Dropdown dropdown)
    {
        switch (dropdown.value)
        {
            case 0:
                totalProbability = 0;
                for (int i = 0; i < reward.Length; i++)
                {
                    reward[i].changePriority(RewardSlotClass.PRIORITY.Low);
                    totalProbability += reward[i].GetAmount();
                }
                break;
            case 1:
                totalProbability = 0;
                for (int i = 0; i < reward.Length; i++)
                {
                    reward[i].changePriority(RewardSlotClass.PRIORITY.High);
                    totalProbability += reward[i].GetAmount();
                }
                break;
            default: 
                break;
        }
    }
    public void ClaimDailySpin()
    {
        switch (rewardsWeekData[today].GetRewardclass()._Type)
        {
            case rewardclass.REWARD_TYPE.Fortune_Wheeler:
                dailySpinPanel.SetActive(true);
                break;
            default:
                break;
        }
    }
}
[Serializable]
public class LastDateClaim
{
    public int day;
    public int month;
    public int year;
}
