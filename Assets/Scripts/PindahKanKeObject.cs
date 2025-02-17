using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PindahKanKeObject : MonoBehaviour
{
    public GameObject pindahKe;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pindahKe = GameObject.FindGameObjectWithTag("WingLocation");
        transform.parent = pindahKe.transform;
        transform.position = pindahKe.transform.position;
    }
}
