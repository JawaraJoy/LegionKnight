using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLeave : MonoBehaviour
{
    public Animator Anim;
    public GameObject Boss;
    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BossLeaveScene(){
        Anim.SetTrigger("Leave");
        StartCoroutine(BossDestroy());
    }

    IEnumerator BossDestroy(){
        yield return new WaitForSeconds(3f);
        Destroy(Boss);
    }
}
