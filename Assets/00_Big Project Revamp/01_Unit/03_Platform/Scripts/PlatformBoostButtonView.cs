using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rush
{
    public class PlatformBoostButtonView : UIView
    {
        [SerializeField]
        private Button m_BoostButton;
        [SerializeField]
        private Image m_BoostFillImage;
        [SerializeField]
        private GameObject m_OverFlowTextContent;
        [SerializeField]
        private TextMeshProUGUI m_BoostOverFlowText;
        [SerializeField]
        private TextMeshProUGUI m_BoostStockCountText;
        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        [Header("Events")]
        [SerializeField]
        private UnityEvent<float> m_OnBoostFilled = new(); // (fillAmount 0-1) dipanggil tiap fill berubah
        [SerializeField]
        private UnityEvent m_OnBoostFull = new();          // dipanggil saat fill mencapai 1 (threshold tercapai)
        [SerializeField]
        private UnityEvent m_OnBoostBackToZero = new();    // dipanggil saat fill kembali ke 0
        [SerializeField]
        private UnityEvent<int> m_OnBoostOverflowChanged = new(); // (overflow) dipanggil tiap overflow berubah

        public UnityEvent<float> OnBoostFilled => m_OnBoostFilled;
        public UnityEvent OnBoostFull => m_OnBoostFull;
        public UnityEvent OnBoostBackToZero => m_OnBoostBackToZero;
        public UnityEvent<int> OnBoostOverflowChanged => m_OnBoostOverflowChanged;

        private PlatformHandler m_Handler;
        private float m_LastFill = -1f; // track fill sebelumnya untuk deteksi full & zero

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
            m_BoostButton.onClick.AddListener(OnBoostButtonClickedInternal);

            Handler.OnBoostEnabled.AddListener(OnBoostEnabledInternal);
            Handler.OnBoostDisabled.AddListener(OnBoostDisabledInternal);
            Handler.OnCurrentBoostStockChanged.AddListener(OnBoostStockChangedInternal);
            Handler.OnPrepare.AddListener(OnPrepareInternal);
            Handler.OnPerfectCountChanged.AddListener(OnPerfectCountChangedInternal);

            SetCanvasInteractableInternal(false);
            SetOverflowTextVisibleInternal(false);
        }

        private void OnDestroy()
        {
            //m_BoostButton.onClick.RemoveListener(OnBoostButtonClickedInternal);
            //Handler.OnBoostEnabled.RemoveListener(OnBoostEnabledInternal);
            //Handler.OnBoostDisabled.RemoveListener(OnBoostDisabledInternal);
            //Handler.OnCurrentBoostStockChanged.RemoveListener(OnBoostStockChangedInternal);
            //Handler.OnPrepare.RemoveListener(OnPrepareInternal);
            //Handler.OnPerfectCountChanged.RemoveListener(OnPerfectCountChangedInternal);
        }

        // --- Button Click ---

        private void OnBoostButtonClickedInternal()
        {
            Handler.ActivateBoost();
            SetCanvasInteractableInternal(false);
        }

        // --- Prepare ---

        private void OnPrepareInternal()
        {
            SetCanvasInteractableInternal(false);
            SetOverflowTextVisibleInternal(false);
            SetStockTextInternal(Handler.CurrentBoostStock, Handler.Config.BoostField.MaxBoostStock);
            SetFillInternal(0f);
        }

        // --- Boost Enabled / Disabled ---

        private void OnBoostEnabledInternal(int overflow)
        {
            SetCanvasInteractableInternal(true);
            SetOverflowTextVisibleInternal(overflow > 0);
            SetOverflowTextInternal(overflow);
            m_OnBoostOverflowChanged?.Invoke(overflow);
        }

        private void OnBoostDisabledInternal()
        {
            SetCanvasInteractableInternal(false);
            SetOverflowTextVisibleInternal(false);
            m_OnBoostOverflowChanged?.Invoke(0);
        }

        // --- Stock Changed ---

        private void OnBoostStockChangedInternal(int currentStock, int maxStock)
        {
            SetStockTextInternal(currentStock, maxStock);
            if (currentStock <= 0)
                HideInternal();
            else
                ShowInternal();
        }

        // --- Perfect Count ---

        private void OnPerfectCountChangedInternal(int currentCount)
        {
            int threshold = Handler.Config.BoostField.BoostThreshold;
            float fill = threshold > 0 ? Mathf.Clamp01((float)currentCount / threshold) : 0f;

            SetFillInternal(fill);

            // OnBoostFull — tepat saat fill pertama kali mencapai 1
            if (fill >= 1f && m_LastFill < 1f)
                m_OnBoostFull?.Invoke();

            // OnBoostBackToZero — saat fill kembali ke 0 dari nilai sebelumnya
            if (fill <= 0f && m_LastFill > 0f)
                m_OnBoostBackToZero?.Invoke();

            m_LastFill = fill;
        }

        // --- Setters ---

        private void SetFillInternal(float fill)
        {
            if (m_BoostFillImage == null) return;
            m_BoostFillImage.fillAmount = fill;

            // Hanya invoke saat fill bertambah
            if (fill > m_LastFill)
                m_OnBoostFilled?.Invoke(fill);
        }

        private void SetCanvasInteractableInternal(bool interactable)
        {
            if (m_CanvasGroup == null) return;
            m_CanvasGroup.interactable = interactable;
            m_CanvasGroup.blocksRaycasts = interactable;
        }

        private void SetOverflowTextVisibleInternal(bool visible)
        {
            if (m_BoostOverFlowText == null) return;
            m_OverFlowTextContent.SetActive(visible);
        }

        private void SetOverflowTextInternal(int overflow)
        {
            if (m_BoostOverFlowText == null) return;
            m_BoostOverFlowText.text = $"+{overflow}";
        }

        private void SetStockTextInternal(int currentStock, int maxStock)
        {
            if (m_BoostStockCountText == null) return;
            m_BoostStockCountText.text = $"{currentStock}/{maxStock}";
        }
    }
}