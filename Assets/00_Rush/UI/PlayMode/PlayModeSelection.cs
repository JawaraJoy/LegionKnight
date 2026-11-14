using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class PlayModeSelection : MonoBehaviour
    {
        [SerializeField] private RectTransform m_Board;
        [SerializeField] private StageConfig m_Classic;
        [SerializeField] private StageConfig m_Colossal;
        [SerializeField, Min(1)] private int m_ColossalLevel = 2;
        [SerializeField] private TMP_Text m_PlayerLevel;
        [SerializeField] private TMP_Text m_EnergyCost;
        [SerializeField] private Button m_ColossalButton;
        [SerializeField] private GameObject m_LockedAction;
        [SerializeField] private GameObject m_PlayAction;
        [SerializeField] private Image m_ColossalArt;
        [SerializeField] private Sprite m_AvailableArt;
        [SerializeField] private Sprite m_LockedArt;
        [SerializeField] private TMP_Text m_LockText;
        [SerializeField] private TutorialAccessPolicy m_ColossalAccess;
        private TutorialManager m_Tutorials;
        public int ColossalUnlockLevel => m_ColossalAccess != null ? m_ColossalAccess.MinimumPlayerLevel : m_ColossalLevel;
        private bool ColossalAvailable => m_Player != null && (m_ColossalAccess != null
            ? m_ColossalAccess.IsAllowed(m_Player.GetPlayerLevel(), GameSetting.Instance != null && GameSetting.Instance.DeveloperMode, m_Tutorials)
            : TutorialAccessPolicy.LevelIsAllowed(m_Player.GetPlayerLevel(), m_ColossalLevel, GameSetting.Instance != null && GameSetting.Instance.DeveloperMode, true));
        private Player m_Player;

        private void OnEnable()
        {
            FitBoard();
            m_Player = Player.Instance;
            if (m_Player == null) return;
            m_Tutorials = TutorialAgent.GetManager();
            if (m_Tutorials != null) m_Tutorials.AvailabilityChanged += Refresh;
            m_Player.AddOnStart(Refresh);
            m_Player.AddOnLevelUp(RefreshLevel);
            Refresh();
        }

        private void OnDisable()
        {
            if (m_Player == null) return;
            if (m_Tutorials != null) m_Tutorials.AvailabilityChanged -= Refresh;
            m_Tutorials = null;
            m_Player.RemoveOnStart(Refresh);
            m_Player.RemoveOnLevelUp(RefreshLevel);
            m_Player = null;
        }

        private void OnRectTransformDimensionsChange() => FitBoard();

        private void FitBoard()
        {
            if (m_Board == null) return;
            Rect space = ((RectTransform)transform).rect;
            Vector2 design = m_Board.sizeDelta;
            float scale = Mathf.Min(space.width / design.x, space.height / design.y);
            if (scale > 0) m_Board.localScale = Vector3.one * scale;
        }

        private void Refresh() => RefreshLevel(m_Player.GetPlayerLevel());

        private void RefreshLevel(int level)
        {
            bool available = ColossalAvailable;
            m_PlayerLevel.text = $"Player Level {level}";
            m_EnergyCost.text = $"{m_Colossal.EnergyAmountToPay} Energy";
            m_LockText.text = $"Requires Lv. {ColossalUnlockLevel}";
            m_ColossalButton.interactable = available;
            m_LockedAction.SetActive(!available);
            m_PlayAction.SetActive(available);
            m_ColossalArt.sprite = available ? m_AvailableArt : m_LockedArt;
        }

        public void PlayClassic() => TryEnter(m_Classic);
        public void PlayColossal() => TryEnter(m_Colossal);

        private bool CanEnter(StageConfig stage)
        {
            return isActiveAndEnabled && m_Player != null &&
                (stage == m_Classic || (stage == m_Colossal && ColossalAvailable));
        }

        private void TryEnter(StageConfig stage)
        {
            if (!CanEnter(stage)) return;
            var stages = RushGameManager.Instance.StageManager;
            if (!stages.HasStageSelection(stage, out var entry)) return;
            // The two endless modes are unlocked by player level, including on older saves.
            if (entry.StageState == StageState.Locked) entry.SetStageState(StageState.Unlocked);
            StageSelectView.TryPlay(stage, () => CanEnter(stage), null);
        }
    }
}
