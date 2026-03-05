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

        private Action<VfxPLayer> m_OnFinishedCallback;
        private Coroutine m_StopRoutine;
        private bool m_IsPlaying;

        public void SetOnFinished(Action<VfxPLayer> callback)
        {
            m_OnFinishedCallback = callback;
        }

        public void Play()
        {
            if (m_IsPlaying)
                return;

            m_IsPlaying = true;

            if (m_StopRoutine != null)
                StopCoroutine(m_StopRoutine);

            // Full reset particle state
            m_Vfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            m_Vfx.Play(true);

            m_OnVFXStart?.Invoke();

            m_StopRoutine = StartCoroutine(WaitForFinish());
        }

        private IEnumerator WaitForFinish()
        {
            // Wait until particle fully dead (safe for any config)
            yield return new WaitUntil(() => !m_Vfx.IsAlive(true));

            m_OnVFXStop?.Invoke();

            m_IsPlaying = false;
            m_StopRoutine = null;

            m_OnFinishedCallback?.Invoke(this);
        }

        public void ForceStop()
        {
            if (!m_IsPlaying)
                return;

            if (m_StopRoutine != null)
                StopCoroutine(m_StopRoutine);

            m_Vfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            m_IsPlaying = false;
            m_StopRoutine = null;

            m_OnFinishedCallback?.Invoke(this);
        }
    }
}