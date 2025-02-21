using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonJump : MonoBehaviour
{
    //public GameObject PlayerTerpilih;

    public GameObject playerScript;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player");
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JumpPlayer(){
        playerScript.GetComponent<Player>().JumpButton();
    }
    public void JumpUnpress()
    {
        playerScript.GetComponent<Player>().JumpUnPress();
    }
}
