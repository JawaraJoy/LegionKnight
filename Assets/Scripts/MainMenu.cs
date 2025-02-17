//using AppsFlyerSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject CharacterFill, CharacterOutline, MarketFill, MarketOutline, ObsFill, ObsOutline;

    public GameObject MarketPanel, CharacterPanel, CharacterHover, CoinPanel, CoinHover;

    public GameObject SelectMode, SelectLevel, TaskRewardPanel, LokasiAwalTask, LokasiOpenTask, Notification;

    public int Score, TargetScore, Tercapai, IntNotif;

    public ChangePosition changePositionScript;

    public GameObject CharacterPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
        //jika pilih halaman character select
        
        CharacterFill.SetActive(true);
        CharacterOutline.SetActive(false);


        MarketFill.SetActive(false);
        MarketOutline.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        CharacterPrefab = GameObject.FindGameObjectWithTag("Player");
        //IntNotif = PlayerPrefs.GetInt("IntNotif");
        if(CharacterPrefab == null)
        {
            return;
        }

        CharacterPrefab.TryGetComponent(out Player thisPlayer);
        if(thisPlayer !=null)
        {
            thisPlayer.enabled = false;
        }
        
    }

    public void ShowCP(){
        //jika pilih halaman character select
    
        CharacterFill.SetActive(true);
        CharacterOutline.SetActive(false);

       
        MarketFill.SetActive(false);
        MarketOutline.SetActive(true);

 
        ObsFill.SetActive(false);
        ObsOutline.SetActive(true);
    }
    public void ShowMP(){
        //jika pilih halaman character select
        CharacterFill.SetActive(false);
        CharacterOutline.SetActive(true);

    
        MarketFill.SetActive(true);
        MarketOutline.SetActive(false);

    
        ObsFill.SetActive(false);
        ObsOutline.SetActive(true);
    }

    public void ShowOP(){
        //jika pilih halaman character select
        CharacterFill.SetActive(false);
        CharacterOutline.SetActive(true);

  
        MarketFill.SetActive(false);
        MarketOutline.SetActive(true);


        ObsFill.SetActive(true);
        ObsOutline.SetActive(false);
    }

    public void PilihMarket(){
        MarketPanel.SetActive(true);
        //changePositionScript.ChangePositionSelect();
    }

    public void TutupMarket(){
        MarketPanel.SetActive(false);
        //changePositionScript.ChangePositionMenu();
    }

    public void PilihCharacterPanel(){
        CharacterPanel.SetActive(true);
        CharacterHover.SetActive(true);
        CoinPanel.SetActive(false);
        CoinHover.SetActive(false);

  
    }

    public void PilihCoinPanel(){
        CharacterPanel.SetActive(false);
        CharacterHover.SetActive(false);
        CoinPanel.SetActive(true);
        CoinHover.SetActive(true);

  
    }

    public void PilihLifePanel(){
        CharacterPanel.SetActive(false);
        CharacterHover.SetActive(false);
        CoinPanel.SetActive(false);
        CoinHover.SetActive(false);
   
   
    }

    public void BukaSelectMode(){
        SelectMode.SetActive(true);
        SelectLevel.SetActive(false);
    }

    public void BukaSelectLevel(){
        SelectMode.SetActive(false);
        SelectLevel.SetActive(true);
    }

    

    public void TaskRewardBuka(){
        //TaskRewardPanel.SetActive(true);
        TaskRewardPanel.transform.position = LokasiOpenTask.transform.position;

        /*Notification.SetActive(false);
        IntNotif = 0;
        PlayerPrefs.SetInt("IntNotif", IntNotif);*/
    }
    public void TaskRewardTutup(){
        //TaskRewardPanel.SetActive(false);
        TaskRewardPanel.transform.position = LokasiAwalTask.transform.position;
    }

    public void PilihScene(string KeSceneMana){
        SceneManager.LoadScene(KeSceneMana);
    }

    public void RestartGameLangsung()
    {
        Scene s = SceneManager.GetActiveScene();
        SceneManager.LoadScene(s.name);
    }

    public void ReloadActiveScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }
}
