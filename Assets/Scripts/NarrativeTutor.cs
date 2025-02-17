using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NarrativeTutor : MonoBehaviour
{
    //public int NextInt;
    public GameObject NarrativeAwal;
    public GameManager GMScript;

    //public int ModeGamePlay;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MunculNarrative());
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void PlayGame(){
        StartCoroutine(MulaiMunculSpawner());
    }

    IEnumerator MunculNarrative(){
        yield return new WaitForSeconds(3f);
        NarrativeAwal.SetActive(true);
    }
    
    IEnumerator MulaiMunculSpawner(){
        yield return new WaitForSeconds(1f);
        //GMScript.MunculSpawner();
    }

    public void StartGame(){
        PlayGame();
    }
}
