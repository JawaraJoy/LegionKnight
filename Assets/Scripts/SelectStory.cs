using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStory : MonoBehaviour
{
    public GameObject ActiveObj, NextObj, PreviousObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PilihNext(){
        //Debug.Log("Next");
        ActiveObj.SetActive(false);
        NextObj.SetActive(true);
    }

    public void Prev(){
        ActiveObj.SetActive(false);
        PreviousObj.SetActive(true);
    }
}
