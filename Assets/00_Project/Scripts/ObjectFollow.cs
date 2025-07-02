using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

public class ObjectFollow : MonoBehaviour
{
    [SerializeField]
    private bool m_StayFollow = true;
    [SerializeField]
    private string m_TargetTag = "Player";
    [SerializeField]
    private Transform m_Target;
    public Vector3 offset;
    [Range(1,10)]
    public float smoothFactor;

    [SerializeField]
    private UnityEvent<Transform> m_OnTargetSet = new();

    private void Start()
    {
        m_Target = GameObject.FindGameObjectWithTag(m_TargetTag).transform;
        m_OnTargetSet?.Invoke(m_Target);
    }

    private void LateUpdate()
    {
        Follow();
    }

    public void Follow()
    {
        if (m_Target == null) return;
        if (!m_StayFollow) return;
        Vector3 targetPosition = m_Target.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, smoothFactor * Time.deltaTime);
        transform.position = smoothPosition;
    }
    public void SetStayFollow(bool set)
    {
        m_StayFollow = set;
    }

    public void SetOffsite(Vector3 set)
    {
        offset = set;
    }
}
