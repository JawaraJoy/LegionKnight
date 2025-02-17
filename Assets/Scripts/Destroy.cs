using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyPrefabs());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DestroyPrefabs(){
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
