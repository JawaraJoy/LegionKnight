using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public sealed class ProfileDeleteConfirmation : MonoBehaviour
    {
        [SerializeField] private GameObject m_Dialog;
        [SerializeField] private UnityEvent m_OnConfirmed;

        public void Open()
        {
            if (isActiveAndEnabled) m_Dialog.SetActive(true);
        }

        public void Cancel() => m_Dialog.SetActive(false);

        public void Confirm()
        {
            if (!isActiveAndEnabled || !m_Dialog.activeInHierarchy) return;
            m_Dialog.SetActive(false);
            OnConfirmedInvoke();
        }

        private void OnConfirmedInvoke() => m_OnConfirmed?.Invoke();
        private void OnDisable() { if (m_Dialog != null) m_Dialog.SetActive(false); }
    }
}
