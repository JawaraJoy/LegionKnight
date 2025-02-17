using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject CharacterObj;
    // Start is called before the first frame update
    void Start()
    {
        CharacterObj = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        CharacterObj.GetComponent<Player>().enabled = true;
    }
}
