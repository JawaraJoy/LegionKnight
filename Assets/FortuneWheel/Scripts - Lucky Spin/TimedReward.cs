using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class TimedReward : MonoBehaviour
{
    DateTime currentDate;
    public List<DaysForClaimed> allReward;
    public Text consecutiveDaysAmountUI;
    //public Text diamondUI, coinUI,characterUI;

    //public int diamond;
    //public int coin;
    public List<string> characterList;
    public Button DaysMissionButton;
    public int DaysMission;

    public GameManager gm;

    void Start()
    {
        //Store the current time when it starts
        currentDate = System.DateTime.Now;
        CheckButtonDaily();
        
        Login();
        CheckConsecutiveDays();
        ResetUI();

    }

    void Update(){
        if (Input.GetKeyDown("t"))
        {
            ResetUI();
            ResetDate();
        }
    }


    public void OnClaimRewardDays(int days)
    {
        string playerPrefsName = "claimedRewardDays" + days;

        //Savee the current system time as a string in the player prefs class
        PlayerPrefs.SetString(playerPrefsName, System.DateTime.Now.ToBinary().ToString());

        print("Saving this date to prefs: " + System.DateTime.Now);
    }

    public bool RequirementDays(int days)
    {
        bool isItDoneYet = true;
        //Grab the old time from the player prefs as a long
        string playerPrefsName = "claimedRewardDays" + days;
        if (PlayerPrefs.GetString(playerPrefsName) != "")
        {
            long temp = Convert.ToInt64(PlayerPrefs.GetString(playerPrefsName));

            //Convert the old time from binary to a DataTime variable
            DateTime oldDate = DateTime.FromBinary(temp);
            
            //Use the Subtract method and store the result as a timespan variable
            TimeSpan difference = currentDate.Subtract(oldDate);

            int hours = difference.Hours + oldDate.Hour;
            int minutes = difference.Minutes + oldDate.Minute;
            int daysLook = difference.Days;
            if (minutes >= 60)
            {
                hours++;
            }
            if (hours >= 24)
            {
                daysLook++;
            }
            if(daysLook >= 2  && days == 1)
            {
                ResetConsecutiveDays();
            }
 
            if (daysLook >= days)
            {
                isItDoneYet = true;
            }
            else
            {
                isItDoneYet = false;
            }
        }
        else
        {
            isItDoneYet = true;
        }
        
        return isItDoneYet;
    }

    public void Login()
    {
        string playerPrefsName = "LoginDaily";
        int days = 1;
        if (PlayerPrefs.GetString(playerPrefsName) != "")
        {
            long temp = Convert.ToInt64(PlayerPrefs.GetString(playerPrefsName));

            //Convert the old time from binary to a DataTime variable
            DateTime oldDate = DateTime.FromBinary(temp);
            print("oldDate: " + oldDate);

            //Use the Subtract method and store the result as a timespan variable
            TimeSpan difference = currentDate.Subtract(oldDate);
            print("Difference from reqDays " + days + ": " + difference);

            int hours = difference.Hours + oldDate.Hour;
            int minutes = difference.Minutes + oldDate.Minute;
            int daysLook = difference.Days;
            if (minutes >= 60)
            {
                hours++;
            }
            if (hours >= 24)
            {
                daysLook++;
            }
            if (daysLook >= 2 && days == 1)
            {
                ResetConsecutiveDays();
            }

            if (daysLook >= days)
            {
                AddConsecutiveDays(playerPrefsName);
            }
            Debug.Log(daysLook);
        }
        else
        {
            AddConsecutiveDays(playerPrefsName);        
        }
    }

    public void AddConsecutiveDays(string playerPrefsName)
    {
        PlayerPrefs.SetInt("ConsecutiveDays",
        PlayerPrefs.GetInt("ConsecutiveDays", 0) + 1);
        PlayerPrefs.SetString(playerPrefsName, System.DateTime.Now.ToBinary().ToString());
        Debug.Log("Adding COnsecuitve");
    }

    public void CheckButtonDaily()
    {
        for (int i = 0; i < allReward.Count; i++)
        {
            allReward[i].buttonShow.GetComponent<Button>().interactable = RequirementDays(allReward[i].requirementDays);
        }
    }

    public bool CheckConsecutiveDays()
    {
        bool consecutive;
        
        if (PlayerPrefs.GetInt("ConsecutiveDays", 0) >= DaysMission)
        {
            DaysMissionButton.interactable = true;
            consecutive = true;
        }
        else
        {
            DaysMissionButton.interactable = false;
            consecutive = false;
        }
       
        return consecutive;
    }

    public void ResetUI()
    {
        consecutiveDaysAmountUI.text = DaysMission - PlayerPrefs.GetInt("ConsecutiveDays", 0) + " Days left";
        //coinUI.text = "Coin : " + gm.Coin;
        //diamondUI.text = "Diamond : " + gm.Diamond;
        //characterUI.text = "";
        for(int i = 0; i < characterList.Count; i++)
        {
            //characterUI.text += "- " + characterList[i] + "\n";
        }
    }

    public void ResetConsecutiveDays()
    {
        PlayerPrefs.SetInt("ConsecutiveDays", 0);
    }

    public void ResetDate()
    {
        string playerPrefsName = "claimedRewardDays1";
        ResetConsecutiveDays();
        PlayerPrefs.SetString(playerPrefsName,"");
        PlayerPrefs.SetString("LoginDaily", "");
        CheckButtonDaily();
    }

}

[System.Serializable]
public struct DaysForClaimed
{
    public int requirementDays;
    public GameObject buttonShow;
}
