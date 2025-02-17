using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localPosition = new Vector3(0,-312,0);
        gameObject.transform.localScale = new Vector3(1.2f,1.2f,1.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangePositionMenu(){
        gameObject.transform.localPosition = new Vector3(0,-312,0);
        gameObject.transform.localScale = new Vector3(1.2f,1.2f,1.2f);
    }
    public void ChangePositionSelect(){
        gameObject.transform.localPosition = new Vector3(0,436,0);
        gameObject.transform.localScale = new Vector3(1,1,1);
    }
}
