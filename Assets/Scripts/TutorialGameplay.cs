using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGameplay : MonoBehaviour
{
    public CubeSpawner cs;
    public GameObject PointerHand,ButtonJump;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StopObstacle());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator StopObstacle(){
        yield return new WaitForSeconds(7.5f);
        PointerHand.SetActive(true);
        ButtonJump.SetActive(true);
        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(3);
        PointerHand.SetActive(false);
    }  

    public void NormalTime(){
       Time.timeScale = 1f;
    }
}
