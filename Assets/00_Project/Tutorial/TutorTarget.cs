using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class TutorTarget : MonoBehaviour
    {
        [SerializeField]
        private TutorStepDefinition m_Definition;
        [SerializeField]
        private UnityEvent<TutorTarget> m_OStepStart = new();
        [SerializeField]
        private UnityEvent<TutorTarget> m_OnStepEnd = new();
        public TutorStepDefinition Definition => m_Definition;

        [SerializeField]
        private Button m_NextButton;
        public Button NextButton => m_NextButton;

        private void Start()
        {
            GameManager.Instance.TutorialManager.AddTarget(this);
        }
        private void OnDestroy()
        {
            GameManager.Instance.TutorialManager.RemoveTarget(this);
        }
    }
}
