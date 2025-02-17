using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random=UnityEngine.Random;
using UnityEngine.UI;

public class ArrowRight : MonoBehaviour
{
    public static ArrowRight CurrentCube { get; private set; }
    
    //public static MovingCube LastCube { get; private set; }

    public GameObject Spawner;
    CubeSpawner SpawnerScript;

    //public float tambahPosisi;

    public bool BonusPoint;
    public bool LevelUp;
    public GameObject GameManagerScript;

    [SerializeField]
    public float moveSpeed, moveSpeedAttack;

    public GameObject PlayerAnim;

    Animator Anim;

    public int BonusInt;
    public GameObject BonusCoinEffect;

    public Text praise;
    public string Nice, Awesome, BestJumper, BeCareful;
    public GameObject niceEmot, AwEmot, FailEmot;

    public int Attack;

    public GameObject GraphicArrow;

    void Start(){
        Spawner = GameObject.FindGameObjectWithTag("Spawner");
        GameManagerScript = GameObject.FindGameObjectWithTag("GameManager");
        PlayerAnim = GameObject.FindGameObjectWithTag("Player");
        moveSpeed = Random.Range(1.5f, 1.5f);
        Anim = GetComponent<Animator>();
        BonusInt = 0;
        Spawner.GetComponent<CubeSpawner>().StopSpawnObs();
    }

    private void OnEnable(){
        CurrentCube = this;
    }

    internal void Stop(){
        moveSpeed = 0;
        Anim.SetTrigger("ChatKananEffect");
    }

    void Update(){
        Attack = PlayerPrefs.GetInt("Attack");
        if (Attack == 0){
            transform.position += transform.right * Time.deltaTime * -moveSpeed;
            
        }

        if (Attack == 1){
            transform.position += transform.up * Time.deltaTime * moveSpeedAttack;
            CurrentCube.GetComponent<BoxCollider2D>().isTrigger = true;
            GraphicArrow.transform.eulerAngles = new Vector3(0f, 0f, -90f);

            CurrentCube.GetComponent<BoxCollider2D>().enabled = false;
            GraphicArrow.GetComponent<BoxCollider2D>().enabled = true;
        }

        /*if (gameObject.transform.position.x <= 0){
            transform.position = new Vector3(0,transform.position.y,0);
            //Bonus Point
            if(!BonusPoint){
                //Debug.Log("BonusPoint");
                BonusPoint = true;

                //GameManagerScript.GetComponent<GameManager>().TambahBonusPoint();
                //StartCoroutine(GameManagerScript.GetComponent<GameManager>().NonAktifBonusText());
                
            }
        }*/
        
        
        if(!LevelUp){
            //Debug.Log("Level Medium");
            LevelUp = true;
            
            //Medium Level
            if (GameManager.Scores >= 50){
                //Debug.Log("Level Medium");
                moveSpeed = Random.Range(1.5f, 1.5f);
            }

            //Hard Level
            if (GameManager.Scores >= 100){
                //Debug.Log("Level Hard");
                moveSpeed = Random.Range(1.5f, 1.5f);
            }
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
        /*if (col.transform.tag == "Player"){
            if (gameObject.transform.position.x != 0){
                GameManagerScript.GetComponent<GameManager>().ResetBonusPoint();
                StartCoroutine(GameManagerScript.GetComponent<GameManager>().NonAktifBonusText());
                //Debug.Log("Reset Bonus");
                Anim.SetTrigger("ChatKananEffect");
                praise.GetComponent<Text>().text = Nice;
                niceEmot.SetActive(true);
            }
        }*/
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
