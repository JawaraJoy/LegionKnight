using MoreMountains.Tools;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class VfxPLayer : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem m_Vfx;

        [SerializeField]
        private UnityEvent m_OnVFXStart;

        [SerializeField]
        private UnityEvent m_OnVFXStop;

        [SerializeField, MMReadOnly]
        private float m_Duration = 1.0f;

        private Action<VfxPLayer> m_OnFinishedCallback;

        public void SetOnFinished(Action<VfxPLayer> callback)
        {
            m_OnFinishedCallback = callback;
        }

        public void Play()
        {
            StopAllCoroutines();

            m_Vfx.Play();
            m_Duration = m_Vfx.main.duration;

            m_OnVFXStart?.Invoke();
            StartCoroutine(Stopping());
        }

        private IEnumerator Stopping()
        {
            yield return new WaitForSeconds(m_Duration);

            m_OnVFXStop?.Invoke();
            m_OnFinishedCallback?.Invoke(this);
        }
    }
}
