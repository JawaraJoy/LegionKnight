using Rush;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    /// <summary>
    /// Item button dalam list StageSelectPanel.
    /// Menampilkan LevelImage sebagai button — saat ditekan,
    /// cek energy cost:
    ///   - Cost = 0 → langsung SelectStage + PlayStage tanpa panel apapun
    ///   - Cost > 0 dan cukup → buka EnergyConfirmationView
    ///   - Cost > 0 tapi kurang → buka EnergyWarningTextView
    /// </summary>
    public class StageSelectView : UIView
    {
        [SerializeField]
        private Image m_LevelImage;
        [SerializeField]
        private Button m_SelectButton;
        [SerializeField]
        private GameObject m_CompleteContent;

        [SerializeField]
        private StageConfig m_StageConfig;

        public void RefreshSelf()
        {
            RefreshInternal(true);
        }

        private void RefreshInternal(bool refreshSetUp)
        {
            if (RushGameManager.Instance.StageManager.HasStageSelection(m_StageConfig, out StageSelectionField field))
            {
                if (m_CompleteContent != null)
                    m_CompleteContent.SetActive(field.StageState == StageState.Completed);

                m_SelectButton.interactable = field.StageState != StageState.Locked;

                if (refreshSetUp)
                {
                    SetUpInternal(field);
                }
            }

        }
        private void SetUpInternal(StageSelectionField field)
        {
            m_StageConfig = field.StageConfig;

            if (m_LevelImage != null)
                m_LevelImage.sprite = m_StageConfig.SplashImage;

            if (m_CompleteContent != null)
                m_CompleteContent.SetActive(field.StageState == StageState.Completed);

            m_SelectButton.onClick.RemoveAllListeners();
            m_SelectButton.onClick.AddListener(OnSelectClicked);
            m_SelectButton.interactable = field.StageState != StageState.Locked;
        }

        public void Refresh()
        {

            RefreshInternal(false);
            
        }

        // ── Setup (dipanggil oleh StageSelectPanel saat spawn) ───────────────
        public void Setup(StageSelectionField field)
        {
            SetUpInternal(field);
        }

        // ── Button callback ───────────────────────────────────────────────────
        private void OnSelectClicked()
        {
            if (m_StageConfig != null)
                TryPlay(m_StageConfig, () => isActiveAndEnabled, null);
        }

        public static void TryPlay(StageConfig stage, System.Func<bool> canEnter, System.Action beforeStart)
        {
            var stages = RushGameManager.Instance.StageManager;
            if (!canEnter() || !stages.HasStageSelection(stage, out var entry) || entry.StageState == StageState.Locked)
                return;

            void EnterStage()
            {
                stages.SelectStage(stage);
                beforeStart?.Invoke();
                CanvasManager.Instance.GetPanel<LevelPanel>().Hide();
                RushGameManager.Instance.GameStateManager.ChangeState(stages.GameStateConfig);
            }

            var wallet = Player.Instance.EnergyController;
            int price = stage.EnergyAmountToPay;
            if (price <= 0)
            {
                wallet.ClearPreviousCost();
                EnterStage();
                return;
            }

            var panel = CanvasManager.Instance.GetPanel<EnergyConfirmationPanel>();
            if (panel == null || stage.EnergyConfig == null) return;
            if (stage.StageMode == StageMode.Collosal &&
                panel.HasBinding<ColossalEnergyDialog>(out var dialog))
            {
                dialog.Open(stage, canEnter, EnterStage);
                return;
            }
            int balance = wallet.GetEnergy(stage.EnergyConfig)?.Amount ?? 0;
            if (balance < price)
            {
                panel.SetWarningText(new[] { new Energy(stage.EnergyConfig, price - balance) });
                return;
            }

            panel.SetConfirmationText(new[] { new Energy(stage.EnergyConfig, price) },
                _ => EnterStage(), panel.SetWarningText, canEnter);
        }
    }
}
