using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rush
{
    public class AetherStateComboView : UIView, IUpdater
    {
        [SerializeField]
        private ComboButtonView m_ComboButton;
        [SerializeField]
        private Transform m_ComboButtonSpawnPoint;
        [SerializeField]
        private float m_ComboButtonSpawnRadius = 5f;
        [SerializeField]
        private Slider m_ComboStateDurationSlider;
        [SerializeField]
        private TextMeshProUGUI m_ComboStateCountText;
        [SerializeField]
        private UnityEvent m_OnComboButtonPressed;

        private PlatformHandler m_Handler;
        private int m_TotalComboCount;
        private int m_RemainingCombo;
        private int m_PressedCombo;
        private float m_BoostDuration;
        private float m_BoostElapsed;
        private bool m_IsBoostActive;

        public bool IsActive => gameObject.activeInHierarchy && m_IsBoostActive;

        private PlatformHandler Handler
        {
            get
            {
                if (m_Handler == null)
                    m_Handler = RushGameManager.Instance.StageManager.PlatformHandler;
                return m_Handler;
            }
        }

        private void Start()
        {
            m_ComboButton.ComboButton.onClick.AddListener(OnComboButtonPressedInternal);

            Handler.OnBoostStart.AddListener(OnBoostStartInternal);
            Handler.OnBoostEnd.AddListener(OnBoostEndInternal);

            HideInternal();
        }

        private void OnEnable()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }

        private void OnDisable()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }

        private void OnDestroy()
        {
            m_ComboButton.ComboButton.onClick.RemoveListener(OnComboButtonPressedInternal);

            Handler.OnBoostStart.RemoveListener(OnBoostStartInternal);
            Handler.OnBoostEnd.RemoveListener(OnBoostEndInternal);
        }

        // --- IUpdater ---

        public void Tick()
        {
            if (!m_IsBoostActive) return;

            m_BoostElapsed += Time.deltaTime;
            float remaining = Mathf.Max(0f, m_BoostDuration - m_BoostElapsed);
            SetSliderValueInternal(remaining, m_BoostDuration);
        }

        // --- Boost Start / End ---

        private void OnBoostStartInternal(float duration, int comboCount)
        {
            PlatformBoostField boostField = Handler.Config.BoostField;
            if (boostField == null) return;

            int overflow = Mathf.Max(0, comboCount - boostField.BoostThreshold);
            m_TotalComboCount = boostField.CalculateComboCount(overflow);
            m_RemainingCombo = m_TotalComboCount;
            m_PressedCombo = 0;

            m_BoostDuration = duration;
            m_BoostElapsed = 0f;
            m_IsBoostActive = true;

            SetupSliderInternal(duration);
            SetComboTextInternal(m_PressedCombo, m_TotalComboCount);
            ShowInternal();
            ShowComboButtonAtRandomPositionInternal();
        }

        private void OnBoostEndInternal()
        {
            m_IsBoostActive = false;
            HideComboButtonInternal();
            HideInternal();
        }

        // --- Combo Button ---

        private void OnComboButtonPressedInternal()
        {
            m_PressedCombo++;
            m_RemainingCombo--;

            HideComboButtonInternal();
            SetComboTextInternal(m_PressedCombo, m_TotalComboCount);
            m_OnComboButtonPressed?.Invoke();

            if (m_RemainingCombo > 0)
                ShowComboButtonAtRandomPositionInternal();
        }

        // --- Internal Helpers ---

        private void ShowComboButtonAtRandomPositionInternal()
        {
            Vector2 randomOffset = Random.insideUnitCircle * m_ComboButtonSpawnRadius;
            m_ComboButton.transform.position = m_ComboButtonSpawnPoint.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
            m_ComboButton.gameObject.SetActive(true);
        }

        private void HideComboButtonInternal()
        {
            m_ComboButton.gameObject.SetActive(false);
        }

        private void SetupSliderInternal(float totalDuration)
        {
            if (m_ComboStateDurationSlider == null) return;
            m_ComboStateDurationSlider.minValue = 0f;
            m_ComboStateDurationSlider.maxValue = totalDuration;
            m_ComboStateDurationSlider.value = totalDuration;
        }

        private void SetSliderValueInternal(float remaining, float total)
        {
            if (m_ComboStateDurationSlider == null) return;
            m_ComboStateDurationSlider.value = remaining;
        }

        private void SetComboTextInternal(int pressed, int total)
        {
            if (m_ComboStateCountText == null) return;
            m_ComboStateCountText.text = $"{pressed}x";
        }
    }
}