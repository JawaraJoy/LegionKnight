using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWings : MonoBehaviour
{
    public Animator animWings;
    public GameObject SkillScript, WingsObj;
    // Start is called before the first frame update
    void Start()
    {
        SkillScript = GameObject.FindGameObjectWithTag("Skills");
    }

    // Update is called once per frame
    void Update()
    {
        SkillScript = GameObject.FindGameObjectWithTag("Skills");
    }

    public void WingsOff(){
        WingsObj.SetActive(false);
        animWings.SetTrigger("FlyingOff");
    }

    public void WingsOn(){
        WingsObj.SetActive(true);
        animWings.SetTrigger("Flying");
    }
    
}
