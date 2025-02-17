using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public GameObject CubeYangAdaKiri, CubeYangAdaKanan;
    // Start is called before the first frame update
    void Start()
    {
        CubeYangAdaKiri = this.gameObject;
        CubeYangAdaKanan = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D (Collision2D col){
        if (col.transform.tag == "Player"){
            StartCoroutine(NoTag());
        }
    }

    void OnTriggerEnter2D (Collider2D col){
        if (col.transform.tag == "CubeDestroyer"){
            Debug.Log("DestroyCube");
            Destroy(this.gameObject);
        }
    }

    IEnumerator NoTag(){
        yield return new WaitForSeconds(0.1f);
        transform.gameObject.tag = "Start";
    }
}
