using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public bool BoolBossOneEye;
    public Animator BossAnim;
    // Start is called before the first frame update
    void Start()
    {
        BossAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (BoolBossOneEye == true){
            BossAnim.SetBool("BossOneEye", true);
        }
        if (BoolBossOneEye == false){
            BossAnim.SetBool("BossOneEye", false);
        }
    }

    public void BossLose(){
        BossAnim.SetTrigger("BossLoseTrigger");
    }
    public void BossComing(){
        BossAnim.SetTrigger("BossComing");
    }
}
