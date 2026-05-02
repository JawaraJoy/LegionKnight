using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Rush
{
    public class AetherStateComboView : UIView, IUpdater
    {
        [System.Serializable]
        public enum SpawnShape
        {
            Circle,
            Square
        }

        [SerializeField]
        private ComboButtonView m_ComboButtonPrefab;
        [SerializeField]
        private Transform m_ComboButtonSpawnPoint;
        [SerializeField]
        private float m_ComboButtonSpawnRadius = 5f;
        [SerializeField]
        private int m_PrewarmCount = 3;
        [SerializeField]
        private SpawnShape m_SpawnShape = SpawnShape.Circle;

        [SerializeField]
        private Slider m_ComboStateDurationSlider;
        [SerializeField]
        private Image m_ComboStateDurationFillImage;
        [SerializeField]
        private TextMeshProUGUI m_ComboStateCountText;
        [SerializeField]
        private UnityEvent m_OnComboButtonPressed;

        private readonly List<ComboButtonView> m_ButtonPool = new();
        private readonly List<ComboButtonView> m_ActiveButtons = new();

        private PlatformHandler m_Handler;
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
            Handler.OnBoostStart.AddListener(OnBoostStartInternal);
            Handler.OnBoostEnd.AddListener(OnBoostEndInternal);
            PrewarmPool(m_PrewarmCount);
            HideInternal();
        }

        private void OnEnable()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }

        private void OnDisable()
        {
            // UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }

        private void OnDestroy()
        {
            // Handler.OnBoostStart.RemoveListener(OnBoostStartInternal);
            // Handler.OnBoostEnd.RemoveListener(OnBoostEndInternal);
        }

        // --- IUpdater ---

        public void Tick()
        {
            if (!m_IsBoostActive) return;

            m_BoostElapsed += Time.deltaTime;
            float remaining = Mathf.Max(0f, m_BoostDuration - m_BoostElapsed);
            SetSliderValueInternal(remaining);
        }

        // --- Boost Start / End ---

        private void OnBoostStartInternal(float duration, int comboCount)
        {
            PlatformBoostField boostField = Handler.Config.BoostField;
            if (boostField == null) return;

            int overflow = Mathf.Max(0, comboCount - boostField.BoostThreshold);
            int buttonCount = boostField.CalculateComboCount(overflow);

            m_PressedCombo = 0;
            m_BoostDuration = duration;
            m_BoostElapsed = 0f;
            m_IsBoostActive = true;

            SetupSliderInternal(duration);
            SetComboTextInternal(m_PressedCombo);
            ShowInternal();

            SpawnActiveButtonsInternal(buttonCount);
        }

        private void OnBoostEndInternal()
        {
            m_IsBoostActive = false;
            ReturnAllActiveButtonsToPool();
            HideInternal();
        }

        // --- Pool ---

        private void PrewarmPool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                ComboButtonView btn = CreateNewButton();
                m_ButtonPool.Add(btn);
            }
        }

        private void SpawnActiveButtonsInternal(int count)
        {
            ReturnAllActiveButtonsToPool();

            for (int i = 0; i < count; i++)
            {
                ComboButtonView btn = GetFromPool();
                RepositionButtonInternal(btn);
                btn.gameObject.SetActive(true);
                m_ActiveButtons.Add(btn);
            }
        }

        private ComboButtonView GetFromPool()
        {
            foreach (ComboButtonView btn in m_ButtonPool)
            {
                if (!btn.gameObject.activeSelf)
                    return btn;
            }

            ComboButtonView newBtn = CreateNewButton();
            m_ButtonPool.Add(newBtn);
            return newBtn;
        }

        private ComboButtonView CreateNewButton()
        {
            ComboButtonView btn = Instantiate(m_ComboButtonPrefab, m_ComboButtonSpawnPoint.parent);
            btn.ComboButton.onClick.AddListener(() => OnComboButtonPressedInternal(btn));
            btn.gameObject.SetActive(false);
            return btn;
        }

        private void ReturnAllActiveButtonsToPool()
        {
            foreach (ComboButtonView btn in m_ActiveButtons)
                btn.gameObject.SetActive(false);

            m_ActiveButtons.Clear();
        }

        // --- Combo Button ---

        private void OnComboButtonPressedInternal(ComboButtonView pressedButton)
        {
            if (!m_IsBoostActive) return;

            m_PressedCombo++;
            SetComboTextInternal(m_PressedCombo);
            m_OnComboButtonPressed?.Invoke();

            RepositionButtonInternal(pressedButton);
        }

        // --- Internal Helpers ---

        private void RepositionButtonInternal(ComboButtonView button)
        {
            Vector2 offset;

            if (m_SpawnShape == SpawnShape.Circle)
            {
                offset = Random.insideUnitCircle * m_ComboButtonSpawnRadius;
            }
            else // Square
            {
                offset = new Vector2(
                    Random.Range(-m_ComboButtonSpawnRadius, m_ComboButtonSpawnRadius),
                    Random.Range(-m_ComboButtonSpawnRadius, m_ComboButtonSpawnRadius)
                );
            }

            button.transform.position = m_ComboButtonSpawnPoint.position
                + new Vector3(offset.x, offset.y, 0f);
        }

        private void SetupSliderInternal(float totalDuration)
        {
            if (m_ComboStateDurationSlider == null) return;

            m_ComboStateDurationSlider.minValue = 0f;
            m_ComboStateDurationSlider.maxValue = totalDuration;
            m_ComboStateDurationSlider.value = totalDuration;

            if (m_ComboStateDurationFillImage != null)
                m_ComboStateDurationFillImage.fillAmount = 1f;
        }

        private void SetSliderValueInternal(float remaining)
        {
            if (m_ComboStateDurationSlider != null)
            {
                m_ComboStateDurationSlider.value = remaining;
            }

            if (m_ComboStateDurationFillImage != null)
            {
                m_ComboStateDurationFillImage.fillAmount = remaining;
            }
        }

        private void SetComboTextInternal(int pressed)
        {
            if (m_ComboStateCountText == null) return;
            m_ComboStateCountText.text = $"{pressed}x";
        }
    }
}