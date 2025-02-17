using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide : MonoBehaviour
{
    public int GuideInt;
    public GameObject GuideObj, ReadyObj;

    public string GuideID;

    public GameManager GMScript;
    public int ModeGamePlay;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GuideDone(){
        GuideInt = 1;
        PlayerPrefs.SetInt(GuideID, GuideInt);
    }
}
