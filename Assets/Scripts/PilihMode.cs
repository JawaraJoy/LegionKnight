using com.adjust.sdk;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilihMode : MonoBehaviour
{
    //public bool ModeStory;
    public GameObject ButtonPlayStory, ButtonPlayClassic;

    public int ModeGamePlay;
    //Dictionary<string, string> modeGame = new Dictionary<string, string>();

    // Update is called once per frame
    void Update()
    {
        /*if (ModeStory == true){
            ButtonPlayClassic.SetActive(false);
        }*/
    }
    
    public void PilihModeClassic(){
        //ModeStory = false;
        ModeGamePlay = 0;
        PlayerPrefs.SetInt("ModeGamePlay", ModeGamePlay);
        //modeGame.Add(AFInAppEvents.CONTENT_VIEW, "Classic");
        //AppsFlyerSDK.AppsFlyer.sendEvent("af_ClassicMode", modeGame);
        //AdjustEvent adjustEvent = new AdjustEvent("jwpb6f");
        //Adjust.trackEvent(adjustEvent);
        //modeGame = new Dictionary<string, string>();
        //ActivityPoint.Instance.AddOrSubActivityPoint(2);
    }

    public void PilihModeStory(){
        //ModeStory = true;
        ModeGamePlay = 1;
        PlayerPrefs.SetInt("ModeGamePlay", ModeGamePlay);
        //AdjustEvent adjustEvent = new AdjustEvent("a3dntc");
        //Adjust.trackEvent(adjustEvent);
        //modeGame.Add(AFInAppEvents.CONTENT_VIEW, "Story");
        //AppsFlyerSDK.AppsFlyer.sendEvent("af_StoryMode", modeGame);
        //modeGame = new Dictionary<string, string>();
        //ActivityPoint.Instance.AddOrSubActivityPoint(3);
    }
}
