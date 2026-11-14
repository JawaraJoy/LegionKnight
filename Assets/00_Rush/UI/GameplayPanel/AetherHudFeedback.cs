using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    [DisallowMultipleComponent]
    public sealed class AetherHudFeedback : MonoBehaviour
    {
        [SerializeField] private Button m_Button;
        [SerializeField] private RectTransform m_Emblem;
        [SerializeField] private Image m_Wings, m_Core, m_Star, m_Gauge;
        [SerializeField] private Image[] m_Pips;
        [SerializeField] private TMP_Text m_Stock, m_Extra, m_Status, m_Time;
        [SerializeField] private GameObject m_Badge, m_Duration;
        [SerializeField] private AudioSource m_Sound;
        [SerializeField] private AudioClip m_ReadyClip, m_StartClip;
        [SerializeField] private AetherHudTrails m_Trails;
        [SerializeField] private CanvasGroup m_IdleArt;
        [SerializeField] private TMP_Text m_ReadyPrompt;
        private bool m_WasActive;
        private float m_FadeTime, m_LastActivePhase;
        private PlatformHandler m_Handler;
        private PlatformBoostButtonView m_View;
        private bool m_Ready;
        private int m_Count = -1, m_Threshold = -1, m_Stocks = -1, m_MaxStock = -1, m_ExtraCount = -1, m_Tenths = -1;
        private float m_Total, m_LastFill = -1, m_ReadyTime;
        private static readonly Color s_Gold = new Color32(255,216,145,255), s_Lilac = new Color32(208,182,255,255),
            s_Quiet = new Color32(143,143,174,215), s_Ivory = new Color32(253,242,214,255);
        private void Start()
        {
            m_View = GetComponent<PlatformBoostButtonView>();
            m_Handler = RushGameManager.Instance.StageManager.PlatformHandler;
            m_Button.onClick.AddListener(ActivateInternal);
            m_Handler.OnBoostStart.AddListener(StartBoostInternal);
            m_Handler.OnPrepare.AddListener(PrepareInternal);
            RefreshInternal();
        }
        private void OnDestroy()
        {
            if (m_Button != null) m_Button.onClick.RemoveListener(ActivateInternal);
            if (m_Handler == null) return;
            m_Handler.OnBoostStart.RemoveListener(StartBoostInternal);
            m_Handler.OnPrepare.RemoveListener(PrepareInternal);
        }
        private void OnDisable()
        {
            m_WasActive = false;
            m_FadeTime = 0;
            if (m_Trails != null) m_Trails.Present(false, false, 0f, 0f);
        }
        private void PrepareInternal() { m_WasActive = false; m_FadeTime = 0; m_Ready = false; m_Total = 0; m_LastFill = -1; m_Tenths = -1; }
        private void ActivateInternal() { if (m_Handler != null) m_Handler.ActivateBoost(); }
        private void StartBoostInternal(float duration, int combo)
        {
            m_Total = duration; m_Ready = false; m_Tenths = -1;
            if (isActiveAndEnabled) m_Sound.PlayOneShot(m_StartClip);
        }
        private void LateUpdate() => RefreshInternal();
        private void RefreshInternal()
        {
            if (m_Handler == null) return;
            var config = m_Handler.Config != null ? m_Handler.Config.BoostField : null;
            int threshold = config != null ? config.BoostThreshold : 0, count = m_Handler.GlobalPerfectCount;
            int stocks = m_Handler.CurrentBoostStock, maxStock = config != null ? config.MaxBoostStock : 0;
            bool active = m_Handler.IsBoostActive && m_Handler.BoostRemainingDuration > 0;
            bool charged = !active && !m_Handler.IsBoostActive && threshold > 0 && count >= threshold && stocks > 0;
            bool canTap = charged && !m_Handler.IsPaused && m_Handler.CurrentTouchedPlatform != null && m_Handler.CurrentTouchedPlatform.CanBoost;
            if (!charged) m_Ready = false;
            if (canTap && !m_Ready)
            { m_Ready = true; m_ReadyTime = 0; m_Sound.PlayOneShot(m_ReadyClip); m_View.OnBoostFullInvoke(); }
            bool ready = charged && m_Ready;
            if (ready && !m_Handler.IsPaused) m_ReadyTime += Time.deltaTime;
            m_Button.interactable = canTap;
            m_Button.targetGraphic.raycastTarget = canTap;
            if (m_Stocks != stocks || maxStock != m_MaxStock) { m_Stocks = stocks; m_MaxStock = maxStock; m_Stock.SetText("{0}/{1}",(float)stocks,(float)maxStock); }
            int extra = active ? 0 : Mathf.Max(0,count-threshold);
            if (m_ExtraCount != extra) { m_ExtraCount = extra; m_Badge.SetActive(extra > 0); m_Extra.SetText("+{0}",extra); m_View.OnBoostOverflowChangedInvoke(extra); }
            float fill = threshold > 0 ? Mathf.Clamp01((float)count/threshold) : 0;
            if (!Mathf.Approximately(m_LastFill,fill))
            { if (m_LastFill > 0 && fill <= 0) m_View.OnBoostBackToZeroInvoke(); if (fill > m_LastFill) m_View.OnBoostFilledInvoke(fill); m_LastFill = fill; }
            if (m_Count != count || m_Threshold != threshold)
            { m_Count = count; m_Threshold = threshold; for (int i=0;i<m_Pips.Length;i++) m_Pips[i].color = (i+1)<=Mathf.CeilToInt(fill*m_Pips.Length) ? s_Gold : s_Quiet; }
            if (active != m_Duration.activeSelf) m_Duration.SetActive(active);
            m_ReadyPrompt.enabled = ready;
            float remain = m_Handler.BoostRemainingDuration;
            if (active)
            {
                if (m_Total <= 0 && config != null) m_Total = config.CalculateBoostDuration(count);
                int tenths = Mathf.CeilToInt(remain*10);
                if (tenths != m_Tenths) { m_Tenths = tenths; m_Time.SetText("{0:1}s",tenths*.1f); }
                m_Gauge.fillAmount = Mathf.Clamp01(remain/Mathf.Max(.001f,m_Total));
            }
            else m_Total = 0;
            float phase = active ? Mathf.Max(0, m_Total - remain) : m_ReadyTime;
            float wave = .5f + .5f * Mathf.Sin(phase * 5.5f);
            float fade = active ? Mathf.SmoothStep(0, 1, phase / .18f) : 0;
            m_IdleArt.alpha = 1f - fade;
            float pop = m_ReadyTime < .42f ? .19f * Mathf.Sin(Mathf.PI * m_ReadyTime / .42f) : 0f;
            float pulse = .028f * Mathf.Sin(Mathf.Max(0, m_ReadyTime - .42f) * 4.35f);
            m_Emblem.localScale = Vector3.one * (active ? 1f - .24f * fade : ready ? 1f + pop + pulse : 1f);
            m_Wings.color = active ? s_Lilac : ready ? Color.Lerp(s_Ivory,s_Gold,wave) : s_Quiet;
            m_Core.color = active ? s_Lilac : ready ? s_Gold : s_Quiet;
            m_Star.color = active || ready ? s_Ivory : s_Quiet;
            float edges = 0f;
            if (active)
            {
                m_LastActivePhase = phase;
                edges = Mathf.SmoothStep(0, 1, phase / .22f);
            }
            else
            {
                if (m_WasActive) m_FadeTime = .38f;
                if (!m_Handler.IsPaused) m_FadeTime = Mathf.Max(0f, m_FadeTime - Time.deltaTime);
                if (m_FadeTime > 0f && !ready)
                {
                    phase = m_LastActivePhase + .38f - m_FadeTime;
                    edges = Mathf.SmoothStep(0, 1, m_FadeTime / .38f);
                }
            }
            m_WasActive = active;
            m_Trails.Present(ready, active, phase, edges);
        }
    }
}
