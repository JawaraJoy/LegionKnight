using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionText : MonoBehaviour
{
    Text verText;
    // Start is called before the first frame update
    void Start()
    {
        verText = GetComponent<Text>();
        verText.text = $"Version {Application.version}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
