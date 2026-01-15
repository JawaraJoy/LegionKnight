using UnityEngine;
using Rush;

namespace LegionKnight
{
    public partial class LevelManagerAgent : MonoBehaviour
    {
        [SerializeField]
        private LevelDefinition m_ApplyEffectOnLevel;

        private bool IsApplyEffectOnLevel()
        {
            return m_ApplyEffectOnLevel != null;
        }
        private bool IsLevelMatch()
        {
            return IsApplyEffectOnLevel() && m_ApplyEffectOnLevel == GameManager.Instance.LevelDefinition;
        }
        public void Play()
        {
            GameManager.Instance.Play();
        }
        public void StartBos()
        {
            GameManager.Instance.StartBos();
        }
        public void RemovePlatform(Platform platform)
        {
            GameManager.Instance.RemovePlatform(platform);
        }
        public void RemoveStandByPlatform(PlatformConfig platform)
        {
            GameManager.Instance.RemoveStandByPlatform(platform);
        }
        public void AddStandByPlatform(PlatformConfig platform)
        {
            if (IsLevelMatch())
            {
                GameManager.Instance.AddStandByPlatform(platform);
            }
        }
        public void SetLevelOver(bool set)
        {
            GameManager.Instance.SetLevelOver(set);
        }
        public void SpawnPlatform()
        {
            GameManager.Instance.SpawnPlatform();
        }
        public void SetLevelDefinition(LevelDefinition set)
        {
            GameManager.Instance.SetLevelDefinition(set);
        }
        public void SetLevelUnlocked(LevelDefinition set, bool unlocked)
        {
            GameManager.Instance.SetLevelUnlocked(set, unlocked);
        }
        public void SetLevelCompleted(LevelDefinition set, bool completed)
        {
            GameManager.Instance.SetLevelCompleted(set, completed);
        }
        public void StartLevel(LevelDefinition defi)
        {
            GameManager.Instance.StartLevel(defi);
        }
    }

    public partial class GameManager
    {
        public void SetLevelUnlocked(LevelDefinition set, bool unlocked)
        {
            m_LevelManager.SetLevelUnlocked(set, unlocked);
        }
        public void SetLevelCompleted(LevelDefinition set, bool completed)
        {
            m_LevelManager.SetLevelCompleted(set, completed);
        }
        public void SetLevelDefinition(LevelDefinition set)
        {
            m_LevelManager.SetLevelDefinition(set);
        }
        public void StartLevel(LevelDefinition defi)
        {
            if (defi != null)
            {
                m_LevelManager.StartLevel(defi);
            }
            else
            {
                Debug.LogError("LevelDefinition is not set.");
            }
        }
    }
}
