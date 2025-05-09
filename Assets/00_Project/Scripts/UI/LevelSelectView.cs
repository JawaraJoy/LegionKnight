using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class LevelSelectView : UIView
    {
        [SerializeField]
        private LevelDefinition m_LevelDefnition;

        [SerializeField]
        private Image m_LevelImage;
        [SerializeField]
        private Button m_StartButton;
        [SerializeField]
        private GameObject m_LockImage;
        [SerializeField]
        private GameObject m_CompleteImage;
        private void OnEnable()
        {
            Init();
        }
        private void Init()
        {
            m_LevelImage.sprite = m_LevelDefnition.LevelImage;
            bool isUnlocked = GameManager.Instance.IsLevelUnlocked(m_LevelDefnition);
            bool isCompleted = GameManager.Instance.IsLevelCompleted(m_LevelDefnition);

            m_LockImage.SetActive(!isUnlocked);
            m_CompleteImage.SetActive(isCompleted);
            m_StartButton.interactable = isUnlocked;
        }
        public void StartLevel()
        {
            if (m_LevelDefnition != null)
            {
                GameManager.Instance.StartLevel(m_LevelDefnition);
            }
            else
            {
                Debug.LogError("LevelDefinition is not set.");
            }
        }
    }
}
