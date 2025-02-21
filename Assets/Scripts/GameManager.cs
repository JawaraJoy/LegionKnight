using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
//using AppsFlyerSDK;
using UnityEngine.Android;
using com.adjust.sdk;
using Random = UnityEngine.Random;
using CloudOnce;

public class GameManager : MonoBehaviour
{
    public static int Scores;
    public int Coins;
    public int Diamond;
    public static int LastScores;
    public int HighScoreInt;

    public int BonusPoint;
    public int BonusRocket;

    //public int Coin;

    public Text ScoreText, CoinText, DiamondText, BonusPointText, Highscores, LastScoreText;

    public GameObject GameOverPanel, PanelRevive, LosePanel, NarrativeFinishPanel;

    public GameObject Spawner, CameraObject;
    public GameObject PlayerScript;
    public GameObject PlayerAnim, CanvasObject, Ground;
    public GameObject PauseButton;

    public GameObject UIGameplay, MainMenuPanel, RandomGameoverScript, RewardScript, AdmobBannerScript, GameManagerObj;

    //public GPGSLeaderboards lbScrips;

    public GameObject music;

    public GameObject PlayMorePanel, MenuPanel;

    public GameObject BossObject;

    public int Attack;

    public int ModeGamePlay = -1;

    public string LevelBerapa;
    public int SudahTamat, LevelStage, Key;
    public Text KeyText;

    public bool JumpStyle;

    public GameObject BossAttention;
    public bool oneTime;
    //public bool oneTimeSkill;
    public int ScoreBerapaBossComing, ScoreBerapaAttention, ScoreSekarang;

    public static bool FinishKah;

    //public GameObject SpawnerSkill;

    public CharacterDatabase characterDatabase;

    //public int ScoreBerapaSkillSpawn;
    //public GameObject SkillObj;

    public bool inGameplay;

    public bool isTraining;

    public bool isGameOver;

    public bool isReviveUsed;

    //public GameObject TimerSkillObj;

    Dictionary<string, string> bosComingEvent = new Dictionary<string, string>();

    private bool isReach200;
    public float volume = 1;

    /*public void disableSkill()
    {
        SpawnerSkill.SetActive(false);
    }*/

    void Start()
    {
        MunculSpawner();
        InterstitialShow = PlayerPrefs.GetInt("InterstitialShow");

        if (inGameplay == true)
        {
            Scores = 0;
        }
        Key = PlayerPrefs.GetInt("Key");
        LevelStage = PlayerPrefs.GetInt("LevelStage");

        music = GameObject.FindGameObjectWithTag("music");
        //LosePanel = GameObject.FindGameObjectWithTag("LosePanel");
        LosePanel.SetActive(false);

        BonusPointText.gameObject.SetActive(false);

        Highscores.text = PlayerPrefs.GetInt("HighScores", 0).ToString();
        CoinText.text = PlayerPrefs.GetInt("Coins", 0).ToString();
        DiamondText.text = PlayerPrefs.GetInt("Diamond", 0).ToString();
        KeyText.text = PlayerPrefs.GetInt("Key", 0).ToString();
        //CoinText.text = PlayerPrefs.GetInt("Coin", 0).ToString();
        //ScoreText.text = PlayerPrefs.GetInt("Score", 0).ToString();
        if (PlayerPrefs.HasKey("FPS"))
        {
            Application.targetFrameRate = PlayerPrefs.GetInt("FPS");
        }
        else
        {
            Application.targetFrameRate = 30;
            PlayerPrefs.SetInt("FPS", 30);
        }

        //Score = PlayerPrefs.GetInt("Score");
        Coins = PlayerPrefs.GetInt("Coins");
        Diamond = PlayerPrefs.GetInt("Diamond");
        LastScores = PlayerPrefs.GetInt("LastScores");
        music.GetComponent<AudioSource>().Play();

        Attack = 0;
        PlayerPrefs.SetInt("Attack", Attack);
        FinishKah = false;
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        if (PlayerPrefs.HasKey("Volume"))
            volume = PlayerPrefs.GetFloat("Volume");
        else
            volume = 1;
        ChangeVolume(volume);

        
    }

    public void ChangeVolume(float vol)
    {
        AudioSource[] listAllAudio = FindObjectsOfType<AudioSource>();
        for (int i = 0; i < listAllAudio.Length; i++)
        {
            listAllAudio[i].volume = vol;
        }
        PlayerPrefs.SetFloat("Volume", vol);
    }

    public void AddEventTemp()
    {
        //Dictionary<string, string> AchievementEvent = new Dictionary<string, string>();
        //AchievementEvent.Add(AFInAppEvents.TUTORIAL_COMPLETION, "Tutorial");
        //AchievementEvent.Add(AFInAppEvents.ACHIEVEMENT_UNLOCKED, "Use Revive");
        //// AchievementEvent.Add(AFInAppEvents.ACHIEVEMENT_UNLOCKED, "Get Bonus Coin");
        ////AchievementEvent.Add(AFInAppEvents.ACHIEVEMENT_UNLOCKED, "Reach 200 Scores");

        //Dictionary<string, string> bosComingEventtemp = new Dictionary<string, string>();
        //bosComingEventtemp.Add(AFInAppEvents.ACHIEVEMENT_UNLOCKED, "Reach 100 Scores");
        //bosComingEventtemp.Add(AFInAppEvents.SCORE, "100");
        //bosComingEventtemp.Add(AFInAppEvents.EVENT_START, "Fight the Boss");
        //bosComingEventtemp.Add(AFInAppEvents.EVENT_END, "Defeat the Boss");

        //Dictionary<string, string> dailyRewardEvent = new Dictionary<string, string>();
        //dailyRewardEvent.Add(AFInAppEvents.CONTENT_VIEW, "Daily Reward");
        //dailyRewardEvent.Add(AFInAppEvents.DATE_A, AppsDaddyO.TimeMan.TimeManager.theCurrentTime.ToString());

        //Dictionary<string, string> modeGame = new Dictionary<string, string>();
        //Dictionary<string, string> modeGame = new Dictionary<string, string>();
        //modeGame.Add(AFInAppEvents.CONTENT_VIEW, "Classic");

        //AppsFlyer.sendEvent("af_ModeGame", modeGame);
        //AppsFlyer.sendEvent("af_DailyReward", dailyRewardEvent);
        //AppsFlyer.sendEvent("af_Fight Boss", bosComingEvent);
        //AppsFlyer.sendEvent("af_tutorial", AchievementEvent);
    }
    public void PlayMoreShow()
    {
        PlayMorePanel.SetActive(true);
    }

    public void PlayMoreHide()
    {
        PlayMorePanel.SetActive(false);
    }

    public void OpenMoreGames()
    {
        Application.OpenURL("https://play.google.com/store/apps/developer?id=LINKIT360");
    }

    public void MenuPanelShow()
    {
        MenuPanel.SetActive(true);
    }

    public void MenuPanelHide()
    {
        MenuPanel.SetActive(false);
    }

    public void Review()
    {
        Application.OpenURL("https://play.google.com/store/apps/developer?id=LINKIT360");
    }

    public void Keluar()
    {
        SaveCoin();
        Application.Quit();
    }

    public void CoinDummyTambah()
    {
        Coins += 100;
        Diamond += 100;
        Key += 1;
        PlayerPrefs.SetInt("Coins", Coins);
        PlayerPrefs.SetInt("Diamond", Diamond);
        PlayerPrefs.SetInt("Key", Key);
    }
    public void CoinDummyKurang()
    {
        Coins -= 100;
        Diamond -= 100;
        PlayerPrefs.SetInt("Coins", Coins);
        PlayerPrefs.SetInt("Diamond", Diamond);
    }

    public void KeyDummyTambah(int amount)
    {
        Key += amount;
        PlayerPrefs.SetInt("Key", Key);
    }

    public void TambahScore()
    {
        Scores += 1;
        Coins += 1;
        LastScores += 1;
        /*if (ModeGamePlay == 0)
            if (Score >= 200)
                if (!isReach200)
                {
                    ActivityPoint.Instance.AddOrSubActivityPoint(3);
                    isReach200 = true;
                }*/
        //Debug.Log("TambahScore");
        //ScoreText.text = Score.ToString();

        //PlayerPrefs.SetInt("Coin", Score);
        PlayerPrefs.SetInt("Coins", Coins);
        PlayerPrefs.SetInt("LastScores", LastScores);
    }
    /*public void TambahScore(int amount)
    {
        Score += amount;
        Coin += amount;
        LastScore += amount;
        //Debug.Log("TambahScore");
        //ScoreText.text = Score.ToString();

        //PlayerPrefs.SetInt("Coin", Score);
        PlayerPrefs.SetInt("Coin", Coin);
        PlayerPrefs.SetInt("LastScore", LastScore);
    }*/
    public int currentScene;
    public void EndGame()
    {
        Destroy(PlayerScript);
        CinematicEndGame.Instance.EndGame(FindObjectOfType<GameManager>().currentScene);
    }
    /*public void BonusRocketOn()
    {
        BonusRocket = 5;
        Score += 5;
        Coin += 5;
        LastScore += 5;
        PlayerPrefs.SetInt("Score", Score);
        PlayerPrefs.SetInt("LastScore", LastScore);
    }*/

    public void TambahBonusPoint()
    {
        BonusPoint += 1;
        BonusPointText.gameObject.SetActive(true);
        PlayerPrefs.SetInt("Coins", Coins);

        if (BonusPoint == 1)
        {
            //BonusPointText.text = BonusPoint.ToString("+" + BonusPointText);
            //BonusPointText.GetComponent<Text>().text = "+" + BonusPoint;
            BonusPointText.text = BonusPoint.ToString();
            Scores += 1;
            Coins += 1;
            LastScores += 1;
        }
        if (BonusPoint == 2)
        {
            BonusPointText.text = BonusPoint.ToString();
            Scores += 2;
            Coins += 2;
            LastScores += 2;
        }
        if (BonusPoint == 3)
        {
            BonusPointText.text = BonusPoint.ToString();
            Scores += 3;
            Coins += 3;
            LastScores += 3;
        }
        /*if (BonusPoint == 4){
            BonusPointText.text = BonusPoint.ToString();
            Score += 4;
        }
        if (BonusPoint == 5){
            //BonusPointText.text = BonusPoint.ToString();
            
            Score += 5;
        }*/
    }

    public void ResetBonusPoint()
    {
        BonusPoint = 0;
        BonusPointText.GetComponent<Text>().text = "Reset Bonus";
        //BonusPointText.gameObject.SetActive(false);
        JumpStyle = false;
    }
    /// <summary>
    /// /////////////////////////////////////////////////////////////////////UPDATE////////////////////
    /// </summary>

    /*public void PlayGame(string KeMana)
    {
        Spawner.GetComponent<CubeSpawner>().Mode = 0;
        PlayerPrefs.SetInt("Mode", Spawner.GetComponent<CubeSpawner>().Mode);
        StartCoroutine(StartGame());
        //CanvasObject.GetComponent<CanvasScript>().PlayOnScene();
        LastScores = 0;
        PlayerPrefs.SetInt("LastScores", LastScores);
        UIGameplay.SetActive(true);
        MainMenuPanel.SetActive(false);
        Globalvar.CallAnalytics("Game_Clasic");
        SceneManager.LoadScene(KeMana);
    }*/

    public void PlayGames(string KeMana)
    {
        LastScores = 0;
        PlayerPrefs.SetInt("LastScores", LastScores);
        SceneManager.LoadScene(KeMana);
    }

    void Update()
    {
        Coins = PlayerPrefs.GetInt("Coins");
        LastScores = PlayerPrefs.GetInt("LastScores");
        HighScoreInt = PlayerPrefs.GetInt("HighScores");

        ScoreSekarang = Scores;
        Spawner = GameObject.FindGameObjectWithTag("Spawner");
        PlayerAnim = GameObject.FindGameObjectWithTag("Player");
        PlayerScript = GameObject.FindGameObjectWithTag("Player");
        /*if (PauseButton != null)
            PauseButton.SetActive(!isGameOver);*/
        //Key = PlayerPrefs.GetInt("Key");
        //LevelStage = PlayerPrefs.GetInt("LevelStage");
        SudahTamat = PlayerPrefs.GetInt(LevelBerapa);

        //PlayerPrefs.SetInt("Score", Score);
        ScoreText.text = Scores.ToString();
        Highscores.text = HighScoreInt.ToString();

        ModeGamePlay = PlayerPrefs.GetInt("ModeGamePlay");
        if(ModeGamePlay == 0){
            Debug.Log("Main di Classic");
            //Spawner.GetComponent<CubeSpawner>().Mode = 0;
            
        }

        //PlayerPrefs.SetInt("Coin", Coin);
        CoinText.text = Coins.ToString();
        DiamondText.text = Diamond.ToString();
        LastScoreText.text = LastScores.ToString();
        KeyText.text = Key.ToString();
        KeyText.text = PlayerPrefs.GetInt("Key", 0).ToString();

        BonusPointText.GetComponent<Text>().text = "+" + BonusPoint;

        if (BonusPoint > 3)
        {
            BonusPoint = 3;
            Scores += 3;
            Coins += 3;
            LastScores += 3;
        }
        if (BonusPoint >= 3)
        {
            JumpStyle = true;
        }
        if (Input.GetKeyDown("x"))
        {
            PlayerPrefs.DeleteAll();
            characterDatabase.resetData();
            SceneManager.LoadScene("Game");

            //lbScrips.UpdateLeaderboardScore();
        }

        if (Input.GetKeyDown("h"))
        {
            FinishGame();
        }
        if (Input.GetKeyDown("i"))
        {
            LevelStage += 1;
            PlayerPrefs.SetInt("LevelStage", LevelStage);
        }

        if (Input.GetKeyDown("c"))
        {
            Spawner.GetComponent<CubeSpawner>().GameOverCancelInvoke();
        }

        if (Input.GetKeyDown("="))
        {
            CoinDummyTambah();
        }
        if (Input.GetKeyDown("-"))
        {
            CoinDummyKurang();
        }

        //Setup Mode Story/////////////////////////////////////////////////////////////
        /*if (GameManager.Score >= 3){
            BossObject.SetActive(true);
        }*/



        if (Spawner.GetComponent<CubeSpawner>().Mode == 1)
        {
            //PlayGameStory1();
            LangsungMaenStory();

            if (Scores >= ScoreBerapaBossComing)
            {
                BossObject.SetActive(true);
                BossObject.GetComponent<Boss>().BossComing();
                //SpawnerSkill.SetActive(false);
                //TimerSkillObj.SetActive(false);
                //bosComingEvent.Add(AFInAppEvents.ACHIEVEMENT_UNLOCKED, "Reach 100 Scores");
                //bosComingEvent.Add(AFInAppEvents.SCORE, "100");
                //bosComingEvent.Add(AFInAppEvents.EVENT_START, "Fight the Boss");
                //AppsFlyer.sendEvent("af_Fight Boss", bosComingEvent);
                AdjustEvent fightTheBoss = new AdjustEvent("a2fh1d");
                fightTheBoss.addCallbackParameter("Achievements", "Reach 100 Scores");
                fightTheBoss.addCallbackParameter("Achievements", "Fight the Boss");
                Adjust.trackEvent(fightTheBoss);
                //Spawner.GetComponent<CubeSpawner>().CubeSpawnerBoss();
            }
            if (Scores >= ScoreBerapaAttention)
            {
                if (!oneTime)
                {
                    BossAttention.SetActive(true);
                    
                    oneTime = true;
                }
            }
        }

        /*if (Score >= ScoreBerapaSkillSpawn)
        {
            if (isGameOver == false)
            {
                if (!oneTimeSkill)
                {
                    SpawnerSkill.SetActive(true);
                    SkillObj.GetComponent<SkillSpawner>().MulaiSpawnSkills();
                    oneTimeSkill = true;
                }

            }
            if (isTraining == true)
            {
                if (!oneTimeSkill)
                {

                    SpawnerSkill.SetActive(true);
                    SkillObj.GetComponent<SkillSpawner>().MulaiSpawnSkills();
                    oneTimeSkill = true;
                }


            }
            if (isGameOver == true)
            {
                SpawnerSkill.SetActive(false);
            }
        }*/
    }

    /*if (Score == ScoreBerapaAttention){
        BossAttention.SetActive(true);
    }*/



    public void KeMenu()
    {
        //AdsManager.Instance.LoadInterstitial();
        Destroy(PlayerScript);
        SceneManager.LoadScene("Game");
        Scores = 0;
        PlayerPrefs.SetInt("Coins", Coins);
        PlayerPrefs.SetInt("Diamond", Diamond);
        
    }

    public void ResetAll()
    {
        Destroy(PlayerScript);
        PlayerPrefs.DeleteAll();
        characterDatabase.resetData();
        ActivityPoint.Instance.activityPoint = 0;
        string path = Application.persistentDataPath;
        DirectoryInfo directoryInfo = new DirectoryInfo(path);

        // Menghapus semua file yang ada di directory persistentDataPath
        foreach (FileInfo file in directoryInfo.GetFiles())
        {
            file.Delete();

        }
        DailyReward.instance.LoadNewRewardData();
        if (Application.isEditor)
            SceneManager.LoadScene("Game");
    }

    public IEnumerator NonAktifBonusText()
    {
        yield return new WaitForSeconds(1f);
        BonusPointText.gameObject.SetActive(false);
        BonusPointText.text = BonusPoint.ToString();
    }

    public int InterstitialShow;

    public void GameOver()
    {
        InterstitialShow++;
        PlayerPrefs.SetInt("InterstitialShow", InterstitialShow);
        if(InterstitialShow == 4){
            Debug.Log("Interstitial Not Showing");
            PlayerPrefs.SetInt("InterstitialShow", InterstitialShow);
        }
        if(InterstitialShow == 5){

            //AdsManager.Instance.LoadInterstitial();

            InterstitialShow = 3;
            Debug.Log("Interstitial Showing");
            PlayerPrefs.SetInt("InterstitialShow", InterstitialShow);
        }
        
        Globalvar.CallAnalytics("GameOver");
        isGameOver = true;

        Time.timeScale = 1f;

        //        SkillObj.SetActive(false);
        if (Scores > PlayerPrefs.GetInt("HighScores", 0))
        {
            Debug.Log("Highscores Hit");
            HighScoreInt = Scores;
            PlayerPrefs.SetInt("HighScores", Scores);
            PlayerPrefs.SetInt("HighScores", HighScoreInt);
            Leaderboards.LegionKnight.SubmitScore(Scores);
            Highscores.text = Scores.ToString();
            //lbScrips.UpdateLeaderboardScore();
        }
        if (isTraining)
        {
            /*if (SkillObj.GetComponent<SkillSpawner>().currentSpawnObject != null)
            {
                Destroy(SkillObj.GetComponent<SkillSpawner>().currentSpawnObject);
                //ScoreBerapaSkillSpawn = Score + 3;
            }*/
        }
        if (Scores >= ScoreBerapaBossComing)
        {
            //bosComingEvent.Add(AFInAppEvents.EVENT_END, "Defeated by Boss");
            //AppsFlyer.sendEvent("af_Fight Boss", bosComingEvent);
        }

        music.GetComponent<AudioSource>().Pause();
        PlayerScript.GetComponent<Player>().DisableRaycastTarget();
        StartCoroutine(GameOverCount());
        PlayerScript.GetComponent<Player>().enabled = false;
        //Destroy(Spawner);
        Spawner.GetComponent<CubeSpawner>().GameOverCancelInvoke();
        //Score -= 1;
        StartCoroutine(NonAktifBonusText());

        //RewardScript.GetComponent<Rewards>().RewardCharacters();



    }

    //Untuk Mode Story
    public void FinishGame()
    {
        isGameOver = true;
        if (Scores > PlayerPrefs.GetInt("HighScores", 0))
        {
            PlayerPrefs.SetInt("HighScores", Scores);
            Leaderboards.LegionKnight.SubmitScore(Scores);
            Highscores.text = Scores.ToString();
            //lbScrips.UpdateLeaderboardScore();
        }
        if (currentScene == 5)
        {
            PlayerPrefs.SetInt("OpenLegendaryShop", 1);
        }
        ActivityPoint.Instance.AddOrSubActivityPoint(3);
        //bosComingEvent.Add(AFInAppEvents.EVENT_END, "Defeat the Boss");
        //AppsFlyer.sendEvent("af_Fight Boss", bosComingEvent);

        music.GetComponent<AudioSource>().Pause();

        PlayerScript.GetComponent<Player>().DisableRaycastTarget();
        //StartCoroutine(FinishCount());
        PlayerScript.GetComponent<Player>().enabled = false;
        //Destroy(Spawner);
        Spawner.GetComponent<CubeSpawner>().GameOverCancelInvoke();
        //Score -= 1;
        StartCoroutine(NonAktifBonusText());

        //RewardScript.GetComponent<Rewards>().RewardCharacters();

        BossObject.GetComponent<Boss>().BossLose();


        Key += 1;
        PlayerPrefs.SetInt("Key", Key);
        //SpawnerSkill.SetActive(false);
        FinishKah = true;
        /*switch (SwitchAdsNetwork.instance.switchAds)
        {
            case SwitchAds.UnityAds:
                break;
            case SwitchAds.Ironsource:
                IronSource.Agent.displayBanner();
                break;
            case SwitchAds.GoogleAdMob:
                break;
            default:
                break;
        }*/
        DialogueSystem.Instance.SetShowState(NARASISTATE.END);
    }

    //public static void StopObstacle(){

    /*if (Score > PlayerPrefs.GetInt("HighScore", 0)){
        PlayerPrefs.SetInt("HighScore", Score);
        Highscore.text = Score.ToString();
        //lbScrips.UpdateLeaderboardScore();
    }*/

    //music.GetComponent<AudioSource>().Pause();

    //PlayerScript.GetComponent<Player>().DisableRaycastTarget();
    //StartCoroutine(GameOverCount());
    //PlayerScript.GetComponent<Player>().enabled = false;
    //Destroy(Spawner);
    //Spawner.GetComponent<CubeSpawner>().GameOverCancelInvoke();
    //Score -= 1;
    //StartCoroutine(NonAktifBonusText());

    //RewardScript.GetComponent<Rewards>().RewardCharacters();



    //}

    /*public void MuteMusic(){
        music.mute = !music.mute;
    }*/

    public int MunculPanelGameOver;
    public int deathCount = 0;
    public GameObject retryPanel;
    IEnumerator GameOverCount()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(3f);

        int rand = Random.Range(0, 100);
        if (Scores >= MunculPanelGameOver)
        {
            if (rand <= 50)
                //IronsourceAdsManager.Instance.RequestInterstitialAds("GameOver");
            //else
            //    IronsourceAdsManager.Instance.RequestRewardedAds("GameOver");

            GameOverPanel.SetActive(true);
            if (deathCount == 0)
            {
                GameOverPanel.GetComponent<ExitButton>().StartRandomExitButton();
                LosePanel.SetActive(true);
                deathCount++;
            }
            else if (deathCount > 0)
            {
                retryPanel.SetActive(true);
                deathCount = 0;
            }
        }

        else if (Scores < MunculPanelGameOver)
        {
            Destroy(PlayerScript);
            KeGame();
        }

        if(isReviveUsed == true){
            PanelRevive.SetActive(false);
            retryPanel.SetActive(true);
        }
    }

    public void BukaPanelFinish()
    {
        FinishPanel.SetActive(true);
        NarrativeFinishPanel.SetActive(false);
    }

    public void DestroyPlayer()
    {
        Destroy(PlayerScript);
    }

    public GameObject FinishPanel;
    /*IEnumerator FinishCount(){
        yield return new WaitForSeconds(3f);
        NarrativeFinishPanel.SetActive(true);
        */
    /*if (Score >= 10){
        GameOverPanel.SetActive(true);
        RandomGameoverScript.GetComponent<RandomGameOver>().RandomPanelGameOver();
        GameOverPanel.GetComponent<ExitButton>().StartRandomExitButton();
    }*/

    /*if (Score < 10){
        KeGame();
    }*/

    //}

    public GameObject AutoSkipReviveObj;

    /*public IEnumerator Revive()
    {
        
    }*/
    public void ReviveTutorial()
    {
        #region fungsi lama
        music.GetComponent<AudioSource>().UnPause();
        isGameOver = false;
        //oneTimeSkill = false;
        GameOverPanel.SetActive(false);
        PanelRevive.SetActive(false);
        RandomGameoverScript.GetComponent<RandomGameOver>().HidePanelReviveReward();
        //PanelRevive.GetComponent<CountTimer>().OpenTimer();
        PlayerAnim.GetComponent<Player>().SetToIdle();
        Spawner.GetComponent<CubeSpawner>().GameOverStartInvoke();
        PlayerScript.GetComponent<Player>().enabled = true;
        PlayerScript.GetComponent<Player>().EnableRaycastTarget();
        /*if (SkillObj.GetComponent<SkillSpawner>().currentSpawnObject != null)
            SkillObj.GetComponent<SkillSpawner>().currentSpawnObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;*/
        #endregion
    }

    public void RevivePlayer(){
        Debug.Log("Revive");
        isGameOver = false;
        isReviveUsed = true;
        //oneTimeSkill = false;
        //GameOverPanel.SetActive(false);
        PanelRevive.SetActive(false);
        RandomGameoverScript.GetComponent<RandomGameOver>().HidePanelReviveReward();
        //PanelRevive.GetComponent<CountTimer>().OpenTimer();
        PlayerAnim.GetComponent<Player>().SetToIdle();
        Spawner.GetComponent<CubeSpawner>().GameOverStartInvoke();
        PlayerScript.GetComponent<Player>().enabled = true;
        PlayerScript.GetComponent<Player>().EnableRaycastTarget();
    }

    public void FinishTutorial()
    {
        //Dictionary<string, string> AchievementEvent = new Dictionary<string, string>();
        //AchievementEvent.Add(AFInAppEvents.TUTORIAL_COMPLETION, "Tutorial");
        //AppsFlyer.sendEvent("af_tutorial",AchievementEvent);
        AdjustEvent adjustEvent = new AdjustEvent("4f1wi3");
        adjustEvent.addCallbackParameter("Tutorial", "Completed");
        Adjust.trackEvent(adjustEvent);
        KeMenu();
        Destroy(PlayerScript);
    }


    /*public void ClaimReward(){
        RewardPanel.SetActive(false);
        Application.LoadLevel(0);
        //Score = 0;
    }*/

    public void StopTimer()
    {
        //GameOverPanel.GetComponent<CountTimer>().StopTimer();
    }
    public void KeGame()
    {
        ModeGamePlay = -1;
        PlayerPrefs.SetInt("ModeGamePlay", ModeGamePlay);
        Scores = 0;
        PlayerPrefs.SetInt("Coins", Coins);
        PlayerPrefs.SetInt("Diamond", Diamond);
        int rand = Random.Range(0, 100);
        if (rand < 90)
        {

            //IronsourceAdsManager.Instance.RequestInterstitialAds("KeGame");

        }
        SceneManager.LoadScene("Game");
        //   UnityAdsManager.Instance.ReqeustAds("KeGame");
    }

    public void ResetScore()
    {
        Scores = 0;
        PlayerPrefs.SetInt("Coins", Coins);
        PlayerPrefs.SetInt("Diamond", Diamond);
    }

    /*IEnumerator NonAktifDestroyer(){
        yield return new WaitForSeconds(0.1f);
        GameObject.Find("Player").transform.GetChild(0).gameObject.SetActive(false);
    }*/


    

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.5f);
        PlayerAnim.GetComponent<Rigidbody2D>().gravityScale = 1;
        PlayerAnim.GetComponent<Animator>().enabled = true;
        //PlayerAnim.GetComponent<Player>().PlayOnAnim();
        CameraObject.GetComponent<CameraFollow>().enabled = true;
        //Ground.SetActive(true);

        yield return new WaitForSeconds(1f);
        if (Spawner.GetComponent<CubeSpawner>().Mode == 0)
        {
            Spawner.GetComponent<CubeSpawner>().enabled = true;
        }

    }

    //Setting untuk mulai classic////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void MunculSpawner()
    {
        CountDownObj.SetActive(true);
        StartCoroutine(CountdownStart());

    }
    /*public void PlayGameStory1(string KeScene)
    {
        //Spawner.GetComponent<CubeSpawner>().Mode = 1;
        //PlayerPrefs.SetInt("Mode", Spawner.GetComponent<CubeSpawner>().Mode);
        if (KeScene == "Story1")
        {
            if (PlayerPrefs.GetInt("ModeGamePlay") == 1)
            {
                AdjustEvent adjustEvent = new AdjustEvent("6fiyt3");
                Adjust.trackEvent(adjustEvent);
                if (!PlayerPrefs.HasKey("CinematicStory"))
                {
                    PlayerPrefs.SetInt("CinematicStory", 1);
                    SceneManager.LoadScene("GameStory");
                }
                else
                {
                    SceneManager.LoadScene(KeScene);
                    Score = 0;
                    LastScores = 0;
                    PlayerPrefs.SetInt("LastScores", LastScores);
                    PlayerPrefs.SetInt("Coins", Coins);
                    PlayerPrefs.SetInt("Diamond", Diamond);
                    Globalvar.CallAnalytics("GameStory_Story1");
                }
            }
            else
            {
                SceneManager.LoadScene(KeScene);
                Score = 0;
                LastScores = 0;
                PlayerPrefs.SetInt("LastScores", LastScores);
                PlayerPrefs.SetInt("Coins", Coins);
                PlayerPrefs.SetInt("Diamond", Diamond);
                Globalvar.CallAnalytics("GameStory_Story1");
            }
        }
        else
        {
            if (KeScene == "Story2")
            {
                AdjustEvent adjustEvent = new AdjustEvent("reenw8");
                Adjust.trackEvent(adjustEvent);
                Globalvar.CallAnalytics("GameStory_Story2");

            }
            else if (KeScene == "Story3")
            {
                AdjustEvent adjustEvent = new AdjustEvent("5qzjot");
                Adjust.trackEvent(adjustEvent);
                Globalvar.CallAnalytics("GameStory_Story3");

            }
            else if (KeScene == "Story4")
            {
                AdjustEvent adjustEvent = new AdjustEvent("e7ic3l");
                Adjust.trackEvent(adjustEvent);
                Globalvar.CallAnalytics("GameStory_Story4");

            }
            else if (KeScene == "Story5")
            {
                AdjustEvent adjustEvent = new AdjustEvent("hzpsun");
                Adjust.trackEvent(adjustEvent);
                Globalvar.CallAnalytics("GameStory_Story5");

            }

            SceneManager.LoadScene(KeScene);
            Score = 0;
            LastScores = 0;
            PlayerPrefs.SetInt("LastScores", LastScores);
            PlayerPrefs.SetInt("Coins", Coins);
            PlayerPrefs.SetInt("Diamond", Diamond);

        }
    }*/

    public void LangsungMaenStory()
    {
        StartCoroutine(StartGame());
        //CanvasObject.GetComponent<CanvasScript>().PlayOnScene();
        //LastScore = 0;
        UIGameplay.SetActive(true);
        MainMenuPanel.SetActive(false);
        //Spawner.GetComponent<CubeSpawner>().Mode = 1;

    }

    public GameObject BuyCoinSuccessPanel;
    public void BuyCoin(int amount)
    {
        Coins += amount;
        PlayerPrefs.SetInt("Coins", Coins);
    }
    public void BuyDiamond(int amount)
    {
        Diamond += amount;
        PlayerPrefs.SetInt("Diamond", Diamond);
    }
    public void BuyDiamondAndKey(int amount, int _key)
    {
        Diamond += amount;
        Key += _key;
        PlayerPrefs.SetInt("Diamond", Diamond);
        PlayerPrefs.SetInt("Key", Key);
    }
    public void Buy500Coin()
    {
        Coins += 500;
        PlayerPrefs.SetInt("Coins", Coins);
        BeliBerhasilEffect();
    }
    public void Buy1000Coin()
    {
        Coins += 1000;
        PlayerPrefs.SetInt("Coin", Coins);
        BeliBerhasilEffect();
    }

    public void BuyCoinSuccessPanelClose()
    {
        BuyCoinSuccessPanel.SetActive(false);
    }

    public void BeliBerhasilEffect()
    {
        BuyCoinSuccessPanel.SetActive(true);
        StartCoroutine(ShowBonusCoinEffect());
        SaveCoin();
    }


    public GameObject PurchaseFailedNotif;
    public void PurchaseFailed()
    {
        PurchaseFailedNotif.SetActive(true);
        StartCoroutine(HidePurchaseFailed());
    }

    IEnumerator HidePurchaseFailed()
    {
        yield return new WaitForSeconds(1f);
        PurchaseFailedNotif.SetActive(false);
    }

    public void ClaimCoin1()
    {
        Coins += 25;
        Scores += 25;
        PlayerPrefs.SetInt("Coin", Coins);
    }
    public void ClaimCoin2()
    {
        Coins += 50;
        Scores += 50;
        PlayerPrefs.SetInt("Coin", Coins);
    }
    public void ClaimCoin3()
    {
        Coins += 75;
        Scores += 75;
        PlayerPrefs.SetInt("Coin", Coins);
    }

    public GameObject BonusAdsCoinEffect, ClaimedPanel;
    public void BerhasilDapatBonusCoin()
    {
        ClaimedPanel.SetActive(true);
        StartCoroutine(ShowBonusCoinEffect());
    }

    IEnumerator ShowBonusCoinEffect()
    {
        yield return new WaitForSeconds(1f);
        BonusAdsCoinEffect.SetActive(true);
        yield return new WaitForSeconds(2f);
        BonusAdsCoinEffect.SetActive(false);
    }

    public void SaveCoin()
    {
        PlayerPrefs.SetInt("Scores", Scores);
        PlayerPrefs.SetInt("Coins", Coins);
        PlayerPrefs.SetInt("Diamond", Diamond);
    }

    public int CountDownInt;
    public GameObject CountDownObj;
    IEnumerator CountdownStart()
    {
        yield return new WaitForSeconds(CountDownInt);
        Spawner.GetComponent<CubeSpawner>().enabled = true;
    }

    public void AddBonusCoin(int BonusCoin)
    {
        Scores += BonusCoin;
        Coins += BonusCoin;
        PlayerPrefs.SetInt("Coins", Coins);
        ScoreText.text = BonusCoin.ToString();
    }

    /*public void HapusSkillRetryScene()
    {
        Score = 0;
        LastScore = 0;
        PlayerPrefs.SetInt("Score", Score);
        PlayerPrefs.SetInt("LastScore", LastScore);
        ScoreText.text = Coin.ToString();
        PlayerPrefs.SetInt("Coin", Coin);
        //PlayerScript.GetComponent<Player>().HapusSkill();
        PlayerScript.GetComponent<Player>().KeIdle();
    }*/

    public void PausedGame()
    {
        Time.timeScale = 0f;
    }

    public void UnPausedGame()
    {
        Time.timeScale = 1f;
    }

    public void UnPausedMusic()
    {

    }
    public void DeleteAllData()
    {
    }

    public void RestartGame()
    {
        //Destroy(PlayerScript);
        PlayerAnim.GetComponent<Player>().SetToIdle();
        PlayerScript.GetComponent<Player>().enabled = true;
        PlayerScript.GetComponent<Player>().EnableRaycastTarget();
        // Mendapatkan nama scene yang aktif saat ini
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        // Me-reload scene yang aktif saat ini
        SceneManager.LoadScene(currentSceneName);
    }

}
