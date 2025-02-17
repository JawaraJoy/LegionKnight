using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random=UnityEngine.Random;
using UnityEngine.UI;

public class MovingCubeArahKiri : MonoBehaviour
{
    public static MovingCubeArahKiri CurrentCube { get; private set; }
    
    //public static MovingCube LastCube { get; private set; }

    public GameObject Spawner;
    CubeSpawner SpawnerScript;

    //public float tambahPosisi;

    public bool BonusPoint;
    public bool LevelUp;
    public GameObject GameManagerScript;

    [SerializeField]
    public float moveSpeed;

    public GameObject PlayerAnim;

    Animator Anim;

    public int BonusInt;
    public GameObject BonusCoinEffect;

    public Text praise;
    public string Nice, Awesome, BestJumper, BeCareful;
    public GameObject niceEmot, AwEmot, FailEmot;

    public GameObject DestroyEffect;

    public GameObject SkillObj;

    void Start(){
        SkillObj = GameObject.FindGameObjectWithTag("Skills");
        Spawner = GameObject.FindGameObjectWithTag("Spawner");
        GameManagerScript = GameObject.FindGameObjectWithTag("GameManager");
        PlayerAnim = GameObject.FindGameObjectWithTag("Player");
        if (Spawner.GetComponent<CubeSpawner>().isTraining == false){
            moveSpeed = Random.Range(3f, 4f);
        }
        if (Spawner.GetComponent<CubeSpawner>().isTraining == true){
            moveSpeed = Random.Range(3f, 3f);
        }
        
        Anim = GetComponent<Animator>();
        BonusInt = 0;
        //Spawner.GetComponent<CubeSpawner>().StartSpawnObs();
    }

    private void OnEnable(){
        CurrentCube = this;
    }

    internal void Stop(){
        moveSpeed = 0;
        Anim.SetTrigger("ChatKananEffect");
    }

    internal void Lanjut(){
        moveSpeed = 3;
        //Anim.SetTrigger("ChatKananEffect");
    }

    void Update(){
        if (Input.GetKeyDown("s"))
        {
            Spawner.GetComponent<CubeSpawner>().StopObstacleInTutorial();
            //Spawner.GetComponent<CubeSpawner>().enabled = false;
            //GameManagerScript.GetComponent<GameManager>().ResetBonusPoint();
            //PlayerAnim.GetComponent<Player>().GameOverKiri();
            //StartCoroutine(DestroyObstacle());
            //gameObject.GetComponent<Collider2D>().isTrigger = true;
            Stop();
            //praise.GetComponent<Text>().text = BeCareful;
            //FailEmot.SetActive(true);
        }
        if (Input.GetKeyDown("d"))
        {
            Lanjut();
            //Spawner.GetComponent<CubeSpawner>().enabled = true;
            //Spawner.GetComponent<CubeSpawner>().GameOverStartInvoke();
            //Spawner.GetComponent<CubeSpawner>().TambahPosition();
            Spawner.GetComponent<CubeSpawner>().StartSpawnObs();
        }
        transform.position += transform.right * Time.deltaTime * -moveSpeed;

        if (gameObject.transform.position.x <= 0){
            transform.position = new Vector3(0,transform.position.y,0);
            //Bonus Point
            if(!BonusPoint){
                //Debug.Log("BonusPoint");
                BonusPoint = true;

                //GameManagerScript.GetComponent<GameManager>().TambahBonusPoint();
                //StartCoroutine(GameManagerScript.GetComponent<GameManager>().NonAktifBonusText());
                
            }
        }
        
        
        if(!LevelUp){
            //Debug.Log("Level Medium");
            LevelUp = true;
            if (GameManager.Scores >= 50){
                //Debug.Log("Level Medium");
                moveSpeed = Random.Range(3f, 4f);
            }

            //Hard Level
            if (GameManager.Scores >= 100){
                //Debug.Log("Level Hard");
                moveSpeed = Random.Range(3f, 5f);
            }
            /*if (SkillObj.GetComponent<Skills>().Rocket == false){
                //Medium Level
                
            }*/

            moveSpeed = Random.Range(3f, 3f);
            /*if (SkillObj.GetComponent<Skills>().Rocket == true){
                //Debug.Log("Level Hard");
                
            }*/
        }

        if (GameManager.FinishKah == true){
            GameManagerScript.GetComponent<GameManager>().ResetBonusPoint();
            //PlayerAnim.GetComponent<Player>().GameOverKanan();
            //StartCoroutine(DestroyObstacle());
            //gameObject.GetComponent<Collider2D>().isTrigger = true;
            //Stop();
            moveSpeed = 0;
        }
    }

    void OnCollisionEnter2D (Collision2D col){
        if (col.transform.tag == "Player"){
            if (gameObject.transform.position.x != 0){
                GameManagerScript.GetComponent<GameManager>().ResetBonusPoint();
                StartCoroutine(GameManagerScript.GetComponent<GameManager>().NonAktifBonusText());
                //Debug.Log("Reset Bonus");
                Anim.SetTrigger("ChatKananEffect");
                praise.GetComponent<Text>().text = Nice;
                niceEmot.SetActive(true);
            }
        }
        if (col.transform.tag == "PlayerGameOver"){
            GameManagerScript.GetComponent<GameManager>().GameOver();
            GameManagerScript.GetComponent<GameManager>().ResetBonusPoint();
            PlayerAnim.GetComponent<Player>().GameOverKiri();
            StartCoroutine(DestroyObstacle());
            gameObject.GetComponent<Collider2D>().isTrigger = true;
            Stop();
            praise.GetComponent<Text>().text = BeCareful;
            FailEmot.SetActive(true);
        }

        if (col.transform.tag == "Shield"){
            Debug.Log("ShieldProtected");
            Destroy(gameObject);
            Instantiate(DestroyEffect, transform.position, transform.rotation);
            Spawner.GetComponent<CubeSpawner>().GameOverCancelInvoke();
            Spawner.GetComponent<CubeSpawner>().temp = false;
            Spawner.GetComponent<CubeSpawner>().GameOverStartInvoke();
        }

        if (BonusInt == 0){
            if(BonusPoint == true){
                if (col.transform.tag == "Player"){
                    Anim.SetTrigger("BonusEffectKanan");
                    GameManagerScript.GetComponent<GameManager>().TambahBonusPoint();
                    StartCoroutine(GameManagerScript.GetComponent<GameManager>().NonAktifBonusText());
                    BonusInt = 1;

                    BonusCoinEffect.SetActive(true);
                    praise.GetComponent<Text>().text = Awesome;
                    AwEmot.SetActive(true);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.tag == "Destroyer"){
            //Debug.Log("Destroy");
            Destroy(gameObject);
        }
    }

    IEnumerator CallSpawnCube(){
        yield return new WaitForSeconds(2f);
        FindObjectOfType<CubeSpawner>().TambahPosition();
        
    }

    IEnumerator DestroyObstacle(){
        yield return new WaitForSeconds(1f);
        //Obstacle Breaks
        Destroy(this.gameObject);
    }
}
