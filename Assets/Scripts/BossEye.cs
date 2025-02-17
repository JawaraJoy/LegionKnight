using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BossEye : MonoBehaviour
{
    public float bloodEyeLeft, bloodEyeRight, BloodEye;
    public GameObject EyeLeft, EyeRight, BossObject, GameManager, BloodEffectLeft, BloodEffectRight;
    public GameObject BloodObj1L, BloodObj2L, BloodObj3L;
    public GameObject BloodObj1R, BloodObj2R, BloodObj3R;

    public float HealthBoss, HealthBossOneEye;

    public float DestroyEye;
    public Animator BossAnim;

    public GameObject CreepA, CreepB;
    public GameObject CreepABlood, CreepBBlood;

    // Start is called before the first frame update
    void Start()
    {
        //HealthBoss = bloodEyeLeft + bloodEyeRight;
        bloodEyeLeft = HealthBoss / 2f;
        bloodEyeRight = HealthBoss / 2f;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("-")){
            DestroyEye += 1;
        }

        if (Input.GetKeyDown("g")){
            HealthBoss -= 1;
        }
        //Finish
        /*if (DestroyEye == 2){
            Debug.Log("Berhasil Mengalahkan Musuh");
            //BossObject.SetActive(false);
            GameManager.GetComponent<GameManager>().FinishGame();
        }*/


        //Effect Mata Kiri/////////////////
        if (bloodEyeLeft == 4){
            BloodObj1L.SetActive(true);
        }
        if (bloodEyeLeft == 2){
            BloodObj2L.SetActive(true);
        }
        if (bloodEyeLeft == 0){
            BloodObj3L.SetActive(true);
        }

        //Effect Mata Kanan/////////////////
        if (bloodEyeRight == 4){
            BloodObj1R.SetActive(true);
        }
        if (bloodEyeRight == 2){
            BloodObj2R.SetActive(true);
        }
        if (bloodEyeRight == 0){
            BloodObj3R.SetActive(true);
        }

        //Effect Mata Satu/////////////////
        if (HealthBossOneEye == 4){
            BloodObj1R.SetActive(true);
        }
        if (HealthBossOneEye == 2){
            BloodObj2R.SetActive(true);
        }
        if (HealthBossOneEye == 0){
            BloodObj3R.SetActive(true);
        }

        
    }

    public void HitEyeLeft(){
        BossAnim.SetTrigger("BossHitRight");
        bloodEyeLeft -= 1;
        HealthBoss -= 1;
        BloodEffectLeft.SetActive(true);
        StartCoroutine(HideEffectBlood());
        if (bloodEyeLeft == 0){
            Debug.Log("Mata Kiri Hancur");
            DestroyEye += 1;
            
            EyeLeft.SetActive(false);
        }

        if (DestroyEye == 2){
            Debug.Log("Berhasil Mengalahkan Musuh");
            //BossObject.SetActive(false);
            GameManager.GetComponent<GameManager>().FinishGame();
        }
    }

    public void HitEyeRight(){
        BossAnim.SetTrigger("BossHitLeft");
        bloodEyeRight -= 1;
        HealthBoss -= 1;
        BloodEffectRight.SetActive(true);
        StartCoroutine(HideEffectBlood());
        if (bloodEyeRight == 0){
            Debug.Log("Mata Kiri Hancur");
            DestroyEye += 1;
            EyeRight.SetActive(false);
        }

        if (DestroyEye == 2){
            Debug.Log("Berhasil Mengalahkan Musuh");
            //BossObject.SetActive(false);
            GameManager.GetComponent<GameManager>().FinishGame();
        }
    }

    public void HitEye(){
        BossAnim.SetTrigger("BossHitRight");
        //BloodEye -= 1;
        HealthBossOneEye -= 1;
        BloodEffectLeft.SetActive(true);
        StartCoroutine(HideEffectBlood());
        if (HealthBossOneEye == 0){
            Debug.Log("Mata Kiri Hancur");
            //DestroyEye += 1;
            EyeLeft.SetActive(false);
            GameManager.GetComponent<GameManager>().FinishGame();
        }

        /*if (DestroyEye == 2){
            Debug.Log("Berhasil Mengalahkan Musuh");
            //BossObject.SetActive(false);
            GameManager.GetComponent<GameManager>().FinishGame();
        }*/
    }

    IEnumerator HideEffectBlood(){
        yield return new WaitForSeconds(2f);
        BloodEffectLeft.SetActive(false);
        BloodEffectRight.SetActive(false);
        CreepABlood.SetActive(false);
        CreepBBlood.SetActive(false);
    }

    public void DestroyCreepA(){
        CreepA.SetActive(false);
        CreepABlood.SetActive(true);
        StartCoroutine(HideEffectBlood());
    }

    public void DestroyCreepB(){
        CreepB.SetActive(false);
        CreepBBlood.SetActive(true);
        StartCoroutine(HideEffectBlood());
    }
}
