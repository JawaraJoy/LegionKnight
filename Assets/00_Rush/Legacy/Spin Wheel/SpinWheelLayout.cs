using System;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rush
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class SpinWheelLayout : UIBehaviour
    {
        [Serializable]
        private sealed class LayoutField
        {
            [SerializeField] private RectTransform m_Design;
            [SerializeField] private CanvasGroup m_Actions;
            [SerializeField] private TMP_Text m_Daily;
            public RectTransform Design => m_Design;
            public CanvasGroup Actions => m_Actions;
            public TMP_Text Daily => m_Daily;
        }

        [Serializable]
        private sealed class WheelField
        {
            [SerializeField] private SpinRewardView[] m_Slots;
            [SerializeField] private SpinRewardView m_Preview;
            [SerializeField] private RectTransform m_Rotor;
            [SerializeField] private Toggle m_SkipToggle;
            public RectTransform Rotor => m_Rotor;
            public Toggle SkipToggle => m_SkipToggle;
            public SpinRewardView[] Slots => m_Slots;
            public SpinRewardView Preview => m_Preview;
        }

        [SerializeField] private LayoutField m_Layout;
        [SerializeField] private WheelField m_Wheel;
        [SerializeField] private SpinWheelResultPanel m_Receipt;
        [SerializeField] private TMP_Text[] m_CountLabels;
        private sealed class QuantityFormat : ITextPreprocessor
        {
            public string PreprocessText(string text)
            {
                if (string.IsNullOrEmpty(text)) return text;
                return text[0] == 'x' ? text.Substring(1) : "×" + text;
            }
        }
        private static readonly QuantityFormat s_QuantityFormat = new QuantityFormat();
        private const float c_DesignWidth = 720f;
        private const float c_MinimumHeight = 1310f;
        private SpinWheelManager m_Manager;

        protected override void OnEnable()
        {
            base.OnEnable();
            FitInternal();
            foreach (TMP_Text label in m_CountLabels) label.textPreprocessor = s_QuantityFormat;
            m_Receipt.OnHide.AddListener(OnReceiptClosedInternal);
            m_Manager = Player.Instance.SpinWheelManager;
            var definition = m_Manager.Definition;
            for (int i = 0; i < m_Wheel.Slots.Length; i++)
            {
                bool hasReward = i < definition.Rewards.Length && definition.Rewards[i] != null;
                var slot = m_Wheel.Slots[i];
                slot.gameObject.SetActive(hasReward);
                if (hasReward) slot.Init(definition.Rewards[i]);
            }
            m_Wheel.Preview.Hide();
            m_Layout.Daily.text = $"{definition.FreeSpinAmountEachDay} Spin Key added daily";
            ApplyRotationInternal(m_Manager.ClockwiseAngle);
            m_Wheel.SkipToggle.SetIsOnWithoutNotify(m_Manager.SkipAnimation);
            m_Wheel.SkipToggle.onValueChanged.AddListener(m_Manager.SetSkipAnimation);
            m_Manager.RotationChanged += ApplyRotationInternal;
            m_Manager.OnSpinStart.AddListener(LockInternal);
            m_Manager.OnClaim.AddListener(OnRewardInternal);
            m_Layout.Actions.interactable = !m_Manager.IsBusy && m_Manager.SelectedReward == null;
        }

        protected override void OnDisable()
        {
            m_Receipt.OnHide.RemoveListener(OnReceiptClosedInternal);
            if (m_Manager != null)
            {
                m_Wheel.SkipToggle.onValueChanged.RemoveListener(m_Manager.SetSkipAnimation);
                m_Manager.RotationChanged -= ApplyRotationInternal;
                m_Manager.OnSpinStart.RemoveListener(LockInternal);
                m_Manager.OnClaim.RemoveListener(OnRewardInternal);
            }
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            FitInternal();
        }

        private void FitInternal()
        {
            if (m_Layout?.Design == null) return;
            var size = ((RectTransform)transform).rect.size;
            float scale = Mathf.Min(size.x / c_DesignWidth, size.y / c_MinimumHeight);
            m_Layout.Design.localScale = Vector3.one * scale;
        }

        private void ApplyRotationInternal(float clockwiseAngle)
        {
            var upright = Quaternion.Euler(0f, 0f, clockwiseAngle);
            m_Wheel.Rotor.localRotation = Quaternion.Inverse(upright);
            foreach (var slot in m_Wheel.Slots)
                slot.transform.localRotation = upright;
        }

        private void LockInternal() => m_Layout.Actions.interactable = false;

        private void OnRewardInternal(SpinRewardDefinition reward) => m_Layout.Actions.interactable = !m_Receipt.IsShown;

        private void OnReceiptClosedInternal()
        {
            m_Wheel.Preview.Hide();
            m_Layout.Actions.interactable = !m_Manager.IsBusy && m_Manager.SelectedReward == null;
        }
    }
}
