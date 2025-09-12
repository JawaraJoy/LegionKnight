using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class ButtonHighlight : MonoBehaviour
    {
        [SerializeField]
        private string m_HightLightName;
        [SerializeField]
        private TweenHandler[] m_ButtonHighlights;

        [SerializeField]
        private UnityEvent m_OnHighlighted;

        public void HightLight(int index)
        {
            foreach (var button in m_ButtonHighlights)
            {
                button.ReverseTween(m_HightLightName);
            }
            m_ButtonHighlights[index].StartTween(m_HightLightName);
            m_OnHighlighted.Invoke();
        }
    }
}
