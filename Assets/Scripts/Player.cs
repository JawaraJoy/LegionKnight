using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour 
{

    public GameObject playerObj;
    public LayerMask platformsLayerMask;
    //private Player_Base playerBase;
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider2d;

    public float jumpVelocity;

    public GameObject GameManagerScript, buttonJump, SingleCoin, coinSpawner;

    public Animator Anim;

    public AudioSource GameOverSfx;

    public int Attack;

    public GameObject Spawner;

    public bool JumpStylePlayer;

    public AudioSource sfx;

    public GameObject TriggerKanan, TriggerKiri;

    //public GameObject SkillsObj;
    //public GameObject SkillsObj;
    //public bool Shield;
    //public float ShieldOffTime;

    //public GameObject ShieldObj;

    void Start() 
    {
        
        Anim = GetComponent<Animator>();
        if(buttonJump == null)
        {
            return;
        }
        buttonJump.GetComponent<Button>().enabled = false;
        
    }

    private void Awake() {
        rigidbody2d = transform.GetComponent<Rigidbody2D>();
        boxCollider2d = transform.GetComponent<BoxCollider2D>();
        
        
    }


    private void Update() {
        GameManagerScript = GameObject.FindGameObjectWithTag("GameManager");
        Spawner = GameObject.FindGameObjectWithTag("Spawner");
        //SkillsObj = GameObject.FindGameObjectWithTag("Skills");
        buttonJump = GameObject.FindGameObjectWithTag("ButtonJump");
        /*if (Input.GetKeyDown("s"))
        {
            Shield = true;
        }
        if (Input.GetKeyDown("d"))
        {
            Shield = false;
        }*/
        /*if (IsGrounded()) {
            
            rigidbody2d.velocity = Vector2.up * jumpVelocity;
            //Anim.SetTrigger("Jump");
            
        }else{
            //Anim.SetTrigger("Idle");
        }*/

        
        JumpStylePlayer = GameManagerScript.GetComponent<GameManager>().JumpStyle;
        // Set Animations
        if (JumpStylePlayer == false)
        {
            if (IsGrounded()) 
            {
                if (rigidbody2d.linearVelocity.x == 0) 
                {
                    Anim.SetBool("Jump3", false);
                    Anim.SetBool("Jump", false);
                    
                } 
                else 
                {
                    //playerBase.PlayMoveAnim(new Vector2(rigidbody2d.velocity.x, 0f));
                    if (Random.Range(0, 100) < 50)
                    {
                        Anim.SetBool("Jump3", true);
                    }
                    else
                    {
                        Anim.SetBool("Jump", true);
                    }
                }
            } 
            else 
            {
                Debug.Log("Jump");
                //playerBase.PlayJumpAnim(rigidbody2d.velocity);
                Anim.SetBool("Jump", true);
                /*if (SkillsObj.GetComponent<Skills>().Rocket == false){
                    if (Random.Range(0, 100) < 50)
                    {
                        Anim.SetBool("Jump3", true);
                    }
                    else
                    {
                        Anim.SetBool("Jump", true);
                    }
                }
                if (SkillsObj.GetComponent<Skills>().Rocket == true){
                    Anim.SetBool("Jump3", false);
                    Anim.SetBool("Jump", false);
                    Anim.SetTrigger("Idle");
                }
                /*if (GameManagerScript.GetComponent<GameManager>().JumpStyle = false){
                    Anim.SetBool("Jump", true);
                }
                if (GameManagerScript.GetComponent<GameManager>().JumpStyle = true){
                    Anim.SetBool("Jump2", true);
                }*/
            }
            /*if (SkillsObj.GetComponent<Skills>().Rocket == true){
                Anim.SetTrigger("Idle");
                
            }*/

            if(GameManagerScript.GetComponent<GameManager>().inGameplay == true)
            {
                gameObject.transform.localScale = new Vector3(1,1,1);
                //Debug.Log("Scale 1");
            }
            if(GameManagerScript.GetComponent<GameManager>().inGameplay == false)
            {
                gameObject.transform.localScale = new Vector3(2,2,2);
                //Debug.Log("Scale 2");
            }

            if (Input.GetKeyDown("s"))
            {
                gameObject.transform.localScale = new Vector3(1,1,1);
                //Debug.Log("Scale 1");
            }
            
        }

        if (JumpStylePlayer == true)
        {
            if (IsGrounded()) 
            {
                if (rigidbody2d.linearVelocity.x == 0) 
                {
                    Anim.SetBool("Jump2", false);
                }
            }
            else 
            {
                Anim.SetBool("Jump2", true);
            }
        }
    }

    public void TriggerPlayerOn()
    {
        TriggerKanan.SetActive(true);
        TriggerKiri.SetActive(true);
    }

    public void TriggerPlayerOff()
    {
        TriggerKanan.SetActive(false);
        TriggerKiri.SetActive(false);
    }

    //Rocket Aktif
    public void ColliderPlayerOff()
    {
        playerObj.GetComponent<Collider2D>().enabled = false;
        playerObj.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        playerObj.GetComponent<Rigidbody2D>().simulated = false;
        Anim.SetTrigger("Idle");
        //transform.position += transform.up * Time.deltaTime * SkillsObj.GetComponent<Skills>().rocketSpeed;
        //playerObj.GetComponent<Rigidbody2D>().gravityScale = 0;
        //Anim.SetBool("Jump2", false);
    }

    //Rocket Tidak Aktif
    public void ColliderPlayerOn(){
        playerObj.GetComponent<Collider2D>().enabled = true;
        playerObj.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        playerObj.GetComponent<Rigidbody2D>().simulated = true;
        //transform.position += transform.up * Time.deltaTime * SkillsObj.GetComponent<Skills>().rocketSpeed;
        //playerObj.GetComponent<Rigidbody2D>().simulated = false;
        //playerObj.GetComponent<Rigidbody2D>().gravityScale = 1;
    }

    /*IEnumerator ShieldOff(){
        yield return new WaitForSeconds(ShieldOffTime);
        Shield = false;
    }*/

    public void JumpButton(){
        if (IsGrounded()) {
            buttonJump.GetComponent<Button>().enabled = false;
            rigidbody2d.linearVelocity = Vector2.up * jumpVelocity;
    
            sfx.Play();
        }else{
            
        }
    }

    private bool IsGrounded() {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, 1f, platformsLayerMask);
        return raycastHit2d.collider != null;
        //Anim.SetTrigger("Idle");
        
    }

    void OnCollisionEnter2D (Collision2D col){
        if (col.transform.tag == "CubeKiri"){
            MovingCubeArahKanan.CurrentCube.Stop();
            //ArrowLeft.CurrentCube.Stop();
            //MovingCubeArahKiri.CurrentCube.Stop();
            //transform.gameObject.tag = "Untagged";
            GameManagerScript.GetComponent<GameManager>().TambahScore();
            //GameManagerScript.GetComponent<GameManager>().ResetBonusPoint();
            //Debug.Log("Player Hit CubeKiri");
            buttonJump.GetComponent<Button>().enabled = true;

            coinSpawner = Instantiate(SingleCoin, transform.position, transform.rotation);
            //RPippetHand.transform.position = LokasiRPippetHand.transform.position;
            
        }

        if (col.transform.tag == "CubeKanan"){
            //MovingCubeArahKanan.CurrentCube.Stop();
            MovingCubeArahKiri.CurrentCube.Stop();
            //ArrowRight.CurrentCube.Stop();
            //transform.gameObject.tag = "Untagged";
            GameManagerScript.GetComponent<GameManager>().TambahScore();
            //GameManagerScript.GetComponent<GameManager>().ResetBonusPoint();
            //Debug.Log("Player Hit CubeKanan");
            buttonJump.GetComponent<Button>().enabled = true;

            coinSpawner = Instantiate(SingleCoin, transform.position, transform.rotation);
        }

        if (col.transform.tag == "Start"){
            buttonJump.GetComponent<Button>().enabled = true;
        }   
        
    

        //Untuk Mode Story
        if (col.transform.tag == "ArrowLeft"){
            //MovingCubeArahKanan.CurrentCube.Stop();
            //ArrowLeft.CurrentCube.Stop();
            //MovingCubeArahKiri.CurrentCube.Stop();
            //transform.gameObject.tag = "Untagged";
            GameManagerScript.GetComponent<GameManager>().TambahScore();
            //GameManagerScript.GetComponent<GameManager>().ResetBonusPoint();
            //Debug.Log("Player Hit CubeKiri");
            buttonJump.GetComponent<Button>().enabled = true;

            coinSpawner = Instantiate(SingleCoin, transform.position, transform.rotation);
            //RPippetHand.transform.position = LokasiRPippetHand.transform.position;
            Attack = 1;
            PlayerPrefs.SetInt("Attack", Attack);
            StartCoroutine(ResumeObs());
        }

        if (col.transform.tag == "ArrowRight"){
            //MovingCubeArahKanan.CurrentCube.Stop();
            //MovingCubeArahKiri.CurrentCube.Stop();
            //ArrowRight.CurrentCube.Stop();
            //transform.gameObject.tag = "Untagged";
            GameManagerScript.GetComponent<GameManager>().TambahScore();
            //GameManagerScript.GetComponent<GameManager>().ResetBonusPoint();
            //Debug.Log("Player Hit CubeKanan");
            buttonJump.SetActive(true);

            coinSpawner = Instantiate(SingleCoin, transform.position, transform.rotation);

            Attack = 1;
            PlayerPrefs.SetInt("Attack", Attack);
            StartCoroutine(ResumeObs());
        }
    }

    IEnumerator ResumeObs(){
        yield return new WaitForSeconds(1f);
        Attack = 0;
        PlayerPrefs.SetInt("Attack", Attack);
        Spawner.GetComponent<CubeSpawner>().StartSpawnObs();
    }

    public void EnableRaycastTarget(){
        buttonJump.GetComponent<Image>().raycastTarget = true;
    }

    public void DisableRaycastTarget(){
        buttonJump.GetComponent<Image>().raycastTarget = false;
    }

    public void GameOverKiri(){
        Anim.SetTrigger("DeathToLeft");
        GameOverSfx.Play();
    }
    public void GameOverKanan(){
        Anim.SetTrigger("DeathToRight");
        GameOverSfx.Play();
    }

    public void SetToIdle()
    {
        Anim.SetTrigger("Idle");
    }
    /*public void PlayOnAnim(){
        Anim.SetTrigger("PlayOn");
    }
    public void PlayOffAnim(){
        Anim.SetTrigger("PlayOff");
    }*/

    /*public void HapusSkill(){
        Destroy(SkillsObj);
    }*/

    public void KeIdle(){
        Anim.SetTrigger("Idle");
    }

    void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.tag == "TriggerKiri"){
            Anim.SetTrigger("AlmostFalling");
        }
        if (col.gameObject.tag == "TriggerKanan"){
            Anim.SetTrigger("AlmostFallingRight");
        }


        if (col.gameObject.tag == "TriggerKiri1")
        {
            Anim.SetTrigger("AlmostFallingLeft1");
        }
        if (col.gameObject.tag == "TriggerKanan2")
        {
            Anim.SetTrigger("AlmostFallingRight2");
            Debug.Log("Animate Jump Right 2");
        }
    }

    public void AktifkanButtonJump() {
        buttonJump.SetActive(true);
    }
}
