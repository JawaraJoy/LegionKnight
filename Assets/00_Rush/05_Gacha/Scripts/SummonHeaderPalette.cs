using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class SummonHeaderPalette : MonoBehaviour
    {
        [Serializable]
        private struct TintField
        {
            [SerializeField] private Graphic m_Graphic;
            [SerializeField] private Color m_SummonColor;

            public Graphic Graphic => m_Graphic;
            public Color SummonColor => m_SummonColor;
        }

        [Serializable]
        private sealed class SelectedTabSlotField
        {
            [SerializeField] private RectTransform m_Target;
            [SerializeField] private Rect m_SelectedAnchors;
            private Vector2 m_Min;
            private Vector2 m_Max;
            private Vector2 m_Position;
            private Vector2 m_Size;

            public void Apply()
            {
                if (m_Target == null) return;
                m_Min = m_Target.anchorMin;
                m_Max = m_Target.anchorMax;
                m_Position = m_Target.anchoredPosition;
                m_Size = m_Target.sizeDelta;
                m_Target.anchorMin = m_SelectedAnchors.min;
                m_Target.anchorMax = m_SelectedAnchors.max;
                m_Target.sizeDelta = Vector2.zero;
                m_Target.anchoredPosition = Vector2.zero;
            }

            public void Restore()
            {
                if (m_Target == null) return;
                m_Target.anchorMin = m_Min;
                m_Target.anchorMax = m_Max;
                m_Target.sizeDelta = m_Size;
                m_Target.anchoredPosition = m_Position;
            }
        }

        [SerializeField] private SelectedTabSlotField[] m_TabSlots = Array.Empty<SelectedTabSlotField>();

        [SerializeField] private TintField[] m_Tints = Array.Empty<TintField>();
        private Color[] m_PreviousColors;

        private void OnEnable()
        {
            foreach (var slot in m_TabSlots) slot.Apply();
            m_PreviousColors = new Color[m_Tints.Length];
            for (int i = 0; i < m_Tints.Length; i++)
            {
                Graphic target = m_Tints[i].Graphic;
                if (target == null) continue;
                m_PreviousColors[i] = target.color;
                target.color = m_Tints[i].SummonColor;
            }
        }

        private void OnDisable()
        {
            foreach (var slot in m_TabSlots) slot.Restore();
            for (int i = 0; i < m_Tints.Length; i++)
            {
                if (m_Tints[i].Graphic != null)
                    m_Tints[i].Graphic.color = m_PreviousColors[i];
            }
        }
    }
}
