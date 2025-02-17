using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillActivation : MonoBehaviour
{
    public GameObject skillsObj, TutorialObj;

    public bool isTutorial;

    void Start(){
        skillsObj = GameObject.FindGameObjectWithTag("Skills");
        
    }

    void Update(){
    }
    
    /*public void ShielOn(){
        skillsObj.GetComponent<Skills>().ShieldBool = true;
        skillsObj.GetComponent<Skills>().ShieldDurationOn();
        Destroy(gameObject);
        Time.timeScale = 1f;
    }

    public void RocketOn(){
        skillsObj.GetComponent<Skills>().RocketOn();
        skillsObj.GetComponent<Skills>().ShieldDurationOn();
        Destroy(gameObject);
        Time.timeScale = 1f;
    }
    public void TestShield()
    {

        skillsObj.GetComponent<Skills>().ShieldBool = true;
        skillsObj.GetComponent<Skills>().ShieldDurationOn();
        Time.timeScale = 1f;
    }
    public void TestWings()
    {
        skillsObj.GetComponent<Skills>().RocketOn();
        skillsObj.GetComponent<Skills>().ShieldDurationOn();
        Time.timeScale = 1f;

    }*/
}
