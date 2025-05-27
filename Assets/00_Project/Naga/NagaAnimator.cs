using UnityEngine;

namespace LegionKnight
{
    public partial class NagaAnimator : ModelView
    {
        [SerializeField]
        private Animator m_Anim;
        [SerializeField]
        private string m_ShowTrigger = "ShowNaga";
        [SerializeField]
        private string m_HideTrigger = "HideNaga";

        public void SetTrigger(string triggerName)
        {
            if (m_Anim != null)
            {
                m_Anim.SetTrigger(triggerName);
            }
            else
            {
                Debug.LogWarning("Animator is not assigned.");
            }
        }
        public void ShowNaga()
        {
            ShowInternal();
            if (m_Anim != null)
            {
                m_Anim.SetTrigger(m_ShowTrigger);
            }
            else
            {
                Debug.LogWarning("Animator is not assigned.");
            }
        }
    }
}
