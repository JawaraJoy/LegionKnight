using LegionKnight;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class CounterPieceView : UIView
    {
        [SerializeField]
        private float m_HideDelay = 0.5f;
        [SerializeField]
        private UnityEvent m_OnStartHiding;

        public void HideDelay()
        {
            if (IsShowInternal)
            {
                RushGameManager.Instance.StartCoroutine(Hiding());
            }
        }
        private IEnumerator Hiding()
        {
            m_OnStartHiding.Invoke();
            yield return new WaitForSeconds(m_HideDelay);
            HideInternal();
        }
    }
}
