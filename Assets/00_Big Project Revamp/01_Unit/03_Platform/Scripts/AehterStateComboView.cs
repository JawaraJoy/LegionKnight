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
        private ComboButtonView[] m_ComboButtons;
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
            foreach (var comboButton in m_ComboButtons)
            {
                ComboButtonView btn = comboButton; // capture untuk lambda
                btn.ComboButton.onClick.AddListener(() => OnComboButtonPressedInternal(btn));
            }

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
            foreach (var comboButton in m_ComboButtons)
            {
                comboButton.ComboButton.onClick.RemoveAllListeners();
            }

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

        private void OnComboButtonPressedInternal(ComboButtonView pressedButton)
        {
            m_PressedCombo++;
            m_RemainingCombo--;

            SetComboTextInternal(m_PressedCombo, m_TotalComboCount);
            m_OnComboButtonPressed?.Invoke();

            if (m_RemainingCombo > 0)
                RepositionButtonInternal(pressedButton);
            else
                HideComboButtonInternal();
        }

        // --- Internal Helpers ---

        private void RepositionButtonInternal(ComboButtonView button)
        {
            Vector2 randomOffset = Random.insideUnitCircle * m_ComboButtonSpawnRadius;
            button.transform.position = m_ComboButtonSpawnPoint.position
                + new Vector3(randomOffset.x, randomOffset.y, 0f);
        }

        private void ShowComboButtonAtRandomPositionInternal()
        {
            foreach (var comboButton in m_ComboButtons)
            {
                Vector2 randomOffset = Random.insideUnitCircle * m_ComboButtonSpawnRadius;
                comboButton.transform.position = m_ComboButtonSpawnPoint.position
                    + new Vector3(randomOffset.x, randomOffset.y, 0f);
                comboButton.gameObject.SetActive(true);
            }
        }

        private void HideComboButtonInternal()
        {
            foreach (var comboButton in m_ComboButtons)
            {
                comboButton.gameObject.SetActive(false);
            }
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