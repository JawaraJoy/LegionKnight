using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{

    public GameObject closeButton1, closeButton2;

    public GameObject[] ButtonReplyRandom;

    void Start(){
        ButtonReplyRandom[1].GetComponent<Button>().onClick.AddListener(() => {
            FindObjectOfType<GameManager>().KeGame();
            FindObjectOfType<GameManager>().DestroyPlayer();
            CloseButtonFalse();
        });
    }

    public void CloseButtonFalse(){
        closeButton1.SetActive(false);
        closeButton2.SetActive(false);
    }

    IEnumerator ShowRandomExitButton(){
        yield return new WaitForSeconds(3f);
        ButtonReplyRandom[1].SetActive(true);
    }

    public void StartRandomExitButton(){
        StartCoroutine(ShowRandomExitButton());
    }
}
