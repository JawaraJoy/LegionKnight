using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorSkill : MonoBehaviour
{
    public GameObject PointerObj, ButtonJump;
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(PausedForTapSkill());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update()
    {
        PointerObj = GameObject.FindGameObjectWithTag("Pointer");
    }

    IEnumerator PausedForTapSkill(){
        yield return new WaitForSeconds(6f);
        ButtonJump.SetActive(false);
        PointerObj.GetComponent<Image>().enabled = true;
        Time.timeScale = 0f;
        
    }

    public void NormalTime(){
        Time.timeScale = 1f;
    }
}
