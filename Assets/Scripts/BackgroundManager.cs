﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject BG, BGLocation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D (Collider2D col){
        if (col.transform.tag == "BGDestroyer"){
            Debug.Log("BGPindah");
            BG.transform.position = BGLocation.transform.position;
        }
    }
}
