using System.Collections;
using UnityEngine;

public class ColliderActivator : MonoBehaviour
{
    [SerializeField] 
    private Collider2D[] m_CollectColliders;
    [SerializeField] 
    private float m_RestoreToPreviousDelay;

    private bool[] m_PreviousStates;
    private Coroutine m_CurrentRoutine;

    private void Awake()
    {
        m_PreviousStates = new bool[m_CollectColliders.Length];
    }

    public void SetActiveCollider(bool active)
    {
        if (m_CurrentRoutine != null)
            StopCoroutine(m_CurrentRoutine);

        m_CurrentRoutine = StartCoroutine(SetActivingCollider(active));
    }

    private IEnumerator SetActivingCollider(bool active)
    {
        // Simpan state awal tiap collider
        for (int i = 0; i < m_CollectColliders.Length; i++)
        {
            m_PreviousStates[i] = m_CollectColliders[i].enabled;
            m_CollectColliders[i].enabled = active;
        }

        yield return new WaitForSeconds(m_RestoreToPreviousDelay);

        // Restore state awal
        for (int i = 0; i < m_CollectColliders.Length; i++)
        {
            m_CollectColliders[i].enabled = m_PreviousStates[i];
        }

        m_CurrentRoutine = null;
    }
}