using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attention : MonoBehaviour
{
    public GameObject HideObj;
    public float Times;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ObjHide());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ObjHide(){
        yield return new WaitForSeconds(Times);
        HideObj.SetActive(false);
    }
}
