﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayOnScene(){
        animator.SetBool("SwitchPlayScene", true);
    }
    public void PlayOffScene(){
        animator.SetBool("SwitchPlayScene", false);
    }
}
