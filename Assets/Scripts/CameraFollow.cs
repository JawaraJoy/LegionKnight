using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    [Range(1,10)]
    public float smoothFactor;

    private void FixedUpdate()
    {
        StartCoroutine(EnableCameraFollow());
    }

    void Update (){
        
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Follow()
    {
        Vector3 targetPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, smoothFactor*Time.fixedDeltaTime);
        transform.position = smoothPosition;
    }

    IEnumerator EnableCameraFollow(){
        yield return new WaitForSeconds(1f);
        Follow();
    }
}
