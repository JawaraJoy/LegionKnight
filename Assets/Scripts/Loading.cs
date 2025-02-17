using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Loading : MonoBehaviour
{
    public int timerLoad;
    public string toScene;

    public VideoPlayer videoPlayer;
    public GameObject blackScreen;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayVideo());
        StartCoroutine(LoadGame());
        
    }

    private IEnumerator PlayVideo()
    {
        videoPlayer.Play();
        yield return new WaitForSeconds(1.5f);
        blackScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadGame(){
        yield return new WaitForSeconds(timerLoad);
        SceneManager.LoadScene(toScene);
    }

    public void SkipStory()
    {
        SceneManager.LoadScene(toScene);
    }
}
