using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SelectLevel : MonoBehaviour
{
    public static SelectLevel Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void OnEnable()
    {
        if (CinematicEndGame.Instance == null)
            return;
        if (CinematicEndGame.Instance.isEndGame)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            if(CinematicEndGame.Instance.currentScene <=transform.childCount)
                transform.GetChild(0).GetChild(CinematicEndGame.Instance.currentScene+1).gameObject.SetActive(true);
        }
    }
}
