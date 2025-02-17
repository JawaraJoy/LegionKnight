using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Attack : MonoBehaviour
{
    //public int bloodBossEye, bloodEyeRight;
    // Start is called before the first frame update

    public GameObject BossEye;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BossEye = GameObject.FindGameObjectWithTag("Boss");
        //EyeRight = GameObject.FindGameObjectWithTag("EyeRight");
    }

    void OnTriggerEnter2D (Collider2D col){
        //Destroy(gameObject);
        if (col.transform.tag == "EyeLeft"){
            Debug.Log("Hit Left Eye");
            BossEye.GetComponent<BossEye>().HitEyeLeft();
            Destroy(gameObject);
        }
        if (col.transform.tag == "EyeRight"){
            Debug.Log("Hit Right Eye");
            BossEye.GetComponent<BossEye>().HitEyeRight();
            Destroy(gameObject);
        }

        if (col.transform.tag == "Eye"){
            Debug.Log("Hit Right Eye");
            BossEye.GetComponent<BossEye>().HitEye();
            Destroy(gameObject);
        }

        if (col.transform.tag == "Destroyer"){
            //Debug.Log("Hit Right Eye");
            Destroy(gameObject);
        }

        if (col.transform.tag == "CreepA"){
            BossEye.GetComponent<BossEye>().DestroyCreepA();
            Destroy(gameObject);
        }
        if (col.transform.tag == "CreepB"){
            BossEye.GetComponent<BossEye>().DestroyCreepB();
            Destroy(gameObject);
        }
    }
}
