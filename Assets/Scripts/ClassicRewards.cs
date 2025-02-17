using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicRewards : MonoBehaviour
{
    public int Tercapai;
    public int Score, TargetScore;
    public GameObject GoButton, ClaimedObject, Notification;

    public int claim;

    public string IDReward, IDClaim;

    public int BonusCoinReward;
    
    public int IntNotif;
    // Start is called before the first frame update
    void Start()
    {
        Tercapai = PlayerPrefs.GetInt(IDReward);
        claim = PlayerPrefs.GetInt(IDClaim);
    }

    // Update is called once per frame
    void Update()
    {
        
        Score = PlayerPrefs.GetInt("LastScore");
        IntNotif = PlayerPrefs.GetInt("IntNotif");

        if (Score > TargetScore){
            Tercapai = 1;
            
            PlayerPrefs.SetInt(IDReward, Tercapai); //this is the fvkproblemmmm
            //PlayerPrefs.SetInt("Tercapai", Tercapai);

        
        }

        if (Tercapai == 1){
            GoButton.SetActive(false);
            
            //IntNotif = 1;
            //PlayerPrefs.SetInt("IntNotif", IntNotif);
            Notification.SetActive(true);
        }

        if (claim == 1){
            ClaimedObject.SetActive(true);
            PlayerPrefs.SetInt(IDClaim, claim);
            Notification.SetActive(false);
        }
 
    }

    public void ClaimReward(){
        claim = 1;
        
    }

    public void ClaimCoinReward(){
        //GameManager.Coin += BonusCoinReward;
        //PlayerPrefs.SetInt("Coin", GameManager.Coin);
    }
}
