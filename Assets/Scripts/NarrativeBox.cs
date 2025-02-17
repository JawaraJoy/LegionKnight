using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NarrativeBox : MonoBehaviour
{
    //public int NextInt;
    public GameObject NarrativeAwal;
    public GameManager GMScript;
    public GameObject CanvasPlayer;
    public GameObject Spawner, ButtonSkip;

    public int ModeGamePlay, narasiAktif;

    public int narasiTutor, narasiStory;
    public bool isTutorial, isStory;
    // Start is called before the first frame update
    void Start()
    {
        Spawner = GameObject.FindGameObjectWithTag("Spawner");
        CanvasPlayer = GameObject.FindGameObjectWithTag("CanvasPlayer");
        StartCoroutine(MulaiPilihMode());
        narasiTutor = PlayerPrefs.GetInt("narasiTutor");
        narasiStory = PlayerPrefs.GetInt("narasiStory");
    
        
    }

    // Update is called once per frame
    void Update()
    {
        //StartCoroutine(TagCanvasPlayer());
        ModeGamePlay = PlayerPrefs.GetInt("ModeGamePlay");
        
        //CanvasPlayer = GameObject.FindGameObjectWithTag("CanvasPlayer");
        
        if (isTutorial == true){
            narasiTutor = 0;
        }
        
    }
    /*public void NextNarrative(){
        CurrentNarrativeObj.SetActive(false);
        NextNarrativeObj.SetActive(true);
    }*/

    public void PlayGame(){
        StartCoroutine(MulaiMunculSpawner());
        //CanvasPlayer.GetComponent<EventTrigger>().enabled = true;
        //CurrentNarrativeObj.SetActive(false);
        if(isStory == true){
            narasiStory = 1;
        }
        PlayerPrefs.SetInt("narasiStory", narasiStory);
    }

    IEnumerator MunculNarrative(){
        yield return new WaitForSeconds(3f);
        if (narasiStory == 0){
            NarrativeAwal.SetActive(true);
            ButtonSkip.SetActive(true);
        }
        if (narasiStory == 1){
            NarrativeAwal.SetActive(false);
            ButtonSkip.SetActive(false);
            PlayGame();
        }
        
        //CanvasPlayer.GetComponent<EventTrigger>().enabled = false;
    }
    

    IEnumerator MulaiMunculSpawner(){
        yield return new WaitForSeconds(1f);
        GMScript.MunculSpawner();
    }
    
    /*IEnumerator TagCanvasPlayer(){
        yield return new WaitForSeconds(0.2f);
        CanvasPlayer = GameObject.FindGameObjectWithTag("CanvasPlayer");
    }*/

    IEnumerator MulaiPilihMode(){
        yield return new WaitForSeconds(1f);
        if (ModeGamePlay == 0){
            isStory = false;
            //StartCoroutine(TagCanvasPlayer());
            //StartCoroutine(MunculNarrative());
            
        }
        if (ModeGamePlay == 1){
            isStory = true;
            ButtonSkip.SetActive(false);
            StartCoroutine(MunculNarrative());
        }
    }

    public void StartGame(){
        PlayGame();
    }
}
