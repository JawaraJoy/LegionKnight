using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Skills : MonoBehaviour
{
    public GameObject playerScript, SkillPos;
    public GameObject SpawnerObj;
    public GameManager gm;
    public Text BonusRocketText;

    [Header("Shield")]
    public bool ShieldBool;
    public GameObject ShieldObj;
    public float ShieldOffDuration;
    public GameObject ShieldIcon;

    [Header("Rocket")]
    public bool Rocket;
    public float rocketSpeed;
    public float RocketOffDuration;
    public GameObject RocketIcon;

    [Header("Skill")]
    //public bool SkillOnOff;
    //public GameObject SkillTimerObj;
    public Image cooldownImage;

    public bool isTutorial;
    public GameObject TutorialObj;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MasukinPlayer());
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown("s"))
        {
            //Rocket = true;
        }
        if (Input.GetKeyDown("d"))
        {
            //Rocket = false;
        }

        /*if (SkillOnOff == true){
            playerScript.GetComponent<Player>().TriggerPlayerOff();
        }
        if (SkillOnOff == false){
            playerScript.GetComponent<Player>().TriggerPlayerOn();
        }*/


        if(ShieldBool == true){
            playerScript.GetComponent<Player>().TriggerPlayerOff();
            ShieldIcon.SetActive(true);
            ShieldObj.SetActive(true);
            cooldownImage.fillAmount -= 1.0f / ShieldOffDuration * Time.deltaTime;
            
            //ShieldObj.SetActive(true);
        }
        if(ShieldBool == false)
        {
            playerScript.GetComponent<Player>().TriggerPlayerOn();
            ShieldIcon.SetActive(false);
            ShieldObj.SetActive(false);
        }

        if(Rocket == true){
            playerScript.GetComponent<Player>().TriggerPlayerOff();
            //SkillOnOff = true;
            playerScript.GetComponent<Player>().ColliderPlayerOff();
            playerScript.GetComponent<Player>().Anim.SetBool("Jump2", false);
            rocketSpeed = 5;
            RocketIcon.SetActive(true);
            cooldownImage.fillAmount -= 1.0f / RocketOffDuration * Time.deltaTime;
        }
        if(Rocket == false){
            playerScript.GetComponent<Player>().TriggerPlayerOn();
            //SkillOnOff = false;
            playerScript.GetComponent<Player>().ColliderPlayerOn();
            rocketSpeed = 0;
            RocketIcon.SetActive(false);
            //SpawnerObj.GetComponent<CubeSpawner>().RocketOff();
        }
    }

    public void ShieldDurationOn(){
        
        //SkillTimerObj.GetComponent<SkillTimer>().MulaiDurasiShield();
        StartCoroutine(ShieldOff());
        
    }

    IEnumerator ShieldOff(){
        yield return new WaitForSeconds(ShieldOffDuration);
        ShieldBool = false;
        cooldownImage.fillAmount = 1.0f;
        //SkillOnOff = false;
        if(isTutorial == true){
            StartCoroutine(TutorialFinished());
        }
    }


    //ROCKET//////////////////////////////////////
    /*public void RocketOn(){
        Rocket = true;
        playerScript.GetComponent<PlayerWings>().WingsOn();
        SpawnerObj.GetComponent<CubeSpawner>().StartRocket();
        StartCoroutine(StopRocketDuration());
        
        //SkillTimerObj.GetComponent<SkillTimer>().MulaiDurasiRocket();
    }*/

    /*public void RocketOff(){
        
        //SpawnerObj.GetComponent<CubeSpawner>().RocketOff();
        SpawnerObj.GetComponent<CubeSpawner>().StopSpawnObsRocket();
        playerScript.GetComponent<PlayerWings>().WingsOff();
        StartCoroutine(ResumeObs());
        gm.BonusRocketOn();
        BonusRocketText.gameObject.SetActive(true);
        BonusRocketText.text = gm.BonusRocket.ToString();
        BonusRocketText.GetComponent<Text>().text = "+" + gm.BonusRocket;
        StartCoroutine(ResetBonusRocket());
        if(isTutorial == true){
            StartCoroutine(TutorialFinished());
        }
    }*/
    public void test()
    {
        StartCoroutine(ResumeObs());

    }
    IEnumerator ResumeObs(){
        yield return new WaitForSeconds(0.5f);
        Rocket = false;
        yield return new WaitForSeconds(2f);
        SpawnerObj.GetComponent<CubeSpawner>().StartSpawnObs();
        cooldownImage.fillAmount = 1.0f;
        
    }

    /*IEnumerator StopRocketDuration(){
        yield return new WaitForSeconds(RocketOffDuration);
        RocketOff();
    }

    IEnumerator ResetBonusRocket(){
        yield return new WaitForSeconds(1f);
        BonusRocketText.gameObject.SetActive(false);
    }*/

    IEnumerator MasukinPlayer(){
        yield return new WaitForSeconds(1f);
        playerScript = GameObject.FindGameObjectWithTag("Player");
        SkillPos = GameObject.FindGameObjectWithTag("PlayerSkillPos");
        transform.parent = SkillPos.transform;
        transform.position = SkillPos.transform.position;
    }

    IEnumerator TutorialFinished(){
        yield return new WaitForSeconds(2f);
        TutorialObj.SetActive(true);
    }
}
