using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ObjectFollow : MonoBehaviour
{
    public string m_TargetTag = "Player";
    public Transform target;
    public Vector3 offset;
    [Range(1,10)]
    public float smoothFactor;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag(m_TargetTag).transform;
    }

    private void LateUpdate()
    {
        Follow();
    }

    public void Follow()
    {
        if (target == null) return;
        Vector3 targetPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, smoothFactor*Time.fixedDeltaTime);
        transform.position = smoothPosition;
    }
}
