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

        private StageConfig m_StageConfig;

        private GameStateConfig m_GamePlayState;

        private GameStateConfig GamePlayStage
        {
            get
            {
                if (m_GamePlayState == null)
                {
                    m_GamePlayState = RushGameManager.Instance.StageManager.GameStateConfig;
                }
                return m_GamePlayState;
            }
        }

        public void Refresh()
        {

            if (RushGameManager.Instance.StageManager.HasStageSelection(m_StageConfig, out StageSelectionField field))
            {
                if (m_CompleteContent != null)
                    m_CompleteContent.SetActive(field.StageState == StageState.Completed);

                m_SelectButton.interactable = field.StageState != StageState.Locked;
            }
            
        }

        // ── Setup (dipanggil oleh StageSelectPanel saat spawn) ───────────────
        public void Setup(StageSelectionField field)
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

        // ── Button callback ───────────────────────────────────────────────────
        private void OnSelectClicked()
        {
            if (m_StageConfig == null) return;

            // Jika tidak ada energy cost → langsung play tanpa buka panel apapun
            bool isFree = m_StageConfig.EnergyConfig == null || m_StageConfig.EnergyAmountToPay <= 0;
            if (isFree)
            {
                RushGameManager.Instance.StageManager.SelectStage(m_StageConfig);
                PlayStage();
                return;
            }

            // Ada energy cost → cek apakah player mampu
            bool canAfford = Player.Instance.EnergyController
                .GetEnergy(m_StageConfig.EnergyConfig)
                ?.CanPay(m_StageConfig.EnergyAmountToPay) ?? false;

            EnergyConfirmationPanel panel = CanvasManager.Instance
                .GetPanel<EnergyConfirmationPanel>();

            if (panel == null)
            {
                Debug.LogError("[StageSelectView] EnergyConfirmationPanel tidak ditemukan di CanvasManager.");
                return;
            }

            Energy[] costs = new Energy[]
            {
                new Energy(m_StageConfig.EnergyConfig, m_StageConfig.EnergyAmountToPay)
            };

            if (canAfford)
            {
                // Cukup → tampilkan "Spend X to play?"
                // m_ConfirmButton di EnergyConfirmationView di-wire ke Pay() via Inspector
                RushGameManager.Instance.StageManager.SelectStage(m_StageConfig);
                panel.SetConfirmationText(
                    costs,
                    onCanPayListen: _ => PlayStage(),
                    onCantPayListen: restCosts => panel.SetWarningText(restCosts)
                );
            }
            else
            {
                // Tidak cukup → hitung kekurangan lalu tampilkan warning
                Energy currentEnergy = Player.Instance.EnergyController
                    .GetEnergy(m_StageConfig.EnergyConfig);

                int missing = m_StageConfig.EnergyAmountToPay - (currentEnergy?.Amount ?? 0);

                Energy[] restCosts = new Energy[]
                {
                    new Energy(m_StageConfig.EnergyConfig, missing)
                };

                panel.SetWarningText(restCosts);
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private void PlayStage()
        {
            CanvasManager.Instance.GetPanel<LevelPanel>().Hide();
            RushGameManager.Instance.GameStateManager.ChangeState(GamePlayStage);
        }
    }
}