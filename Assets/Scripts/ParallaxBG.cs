using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBG : MonoBehaviour
{
    public Transform bg1, bg2;

    private float size;
    // Start is called before the first frame update
    void Start()
    {
        size = bg1.GetComponent<BoxCollider2D>().size.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.y >= bg2.position.y){
            bg1.position = new Vector3(bg1.position.x, bg2.position.y + size, bg1.position.z);
            SwitchBG();
        }
    }

    private void SwitchBG(){
        Transform temp = bg1;
        bg1 = bg2;
        bg2 = temp;
    }
}
