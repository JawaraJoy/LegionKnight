using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class Progression : MonoBehaviour
    {
        [SerializeField]
        private float m_ExponentialGrowth = 2; // Example: 2 means the experience required for each level increases exponentially
        [SerializeField]
        private int m_FirstLevelExp = 100; // Experience required for the first level
        [SerializeField]
        private int m_MaxLevel = 100;
        private int m_Level = 1;
        private int m_CurrentExp = 0;

        private const string m_CurrentExperienceKey = "Exp";
        private const string m_CurrentLevelKey = "Lv";

        [SerializeField]
        private List<int> m_ExpTable = new List<int>();

        [SerializeField]
        private UnityEvent<int> m_OnCurrentExpChange = new ();
        [SerializeField]
        private UnityEvent<int> m_OnLevelUp = new ();

        private bool m_LevelUpTriggered = false;

        private void InitInternal()
        {
            if (UnityService.Instance.HasData(m_CurrentLevelKey))
            {
                m_Level = UnityService.Instance.GetData<int>(m_CurrentLevelKey);
            }
            else
            {
                m_Level = 1; // Initialize level to 1 if not found
            }
            if (UnityService.Instance.HasData(m_CurrentExperienceKey))
            {
                m_CurrentExp = UnityService.Instance.GetData<int>(m_CurrentExperienceKey);
            }
            else
            {
                m_CurrentExp = 0; // Initialize current experience to 0 if not found
            }
            GameManager.Instance.InitLevelView();
        }
        public void Init()
        {
            InitInternal();
        }

        private void OnLevelUpInvoke()
        {
            m_OnLevelUp?.Invoke(m_Level);
            m_LevelUpTriggered = true;

            //GameManager.Instance.ShowLevelUpPanel();
        }

        public void ShowLevelUpPanel()
        {
            if (m_LevelUpTriggered)
            {
                GameManager.Instance.ShowLevelUpPanel();
                m_LevelUpTriggered = false;
            }
        }
        private void OnCurrentExpRateChangedInvoke()
        {
            m_OnCurrentExpChange?.Invoke(m_CurrentExp);
            GameManager.Instance.InitLevelView();
        }
        private void OnValidate()
        {
            // Ensure the experience table is generated correctly in the editor
            GenerateExpTable();

        }

        private float GetLevelProgressionRateInternal()
        {
            if (m_Level > 0 && m_Level <= m_ExpTable.Count)
            {
                return (float)m_CurrentExp / m_ExpTable[m_Level - 1];
            }
            return 0f; // Return 0 if level is out of bounds
        }

        public float GetLevelProgressionRate()
        {
            return GetLevelProgressionRateInternal();
        }

        public void AddExperience(int exp)
        {
            m_CurrentExp += exp;
            while (m_CurrentExp >= GetCurrentMaxExperience())
            {
                LevelUp();
            }
            UnityService.Instance.SaveData(m_CurrentExperienceKey, m_CurrentExp);
            OnCurrentExpRateChangedInvoke();
            Debug.Log($"Current Exp: {m_CurrentExp}, Level: {m_Level}");
        }
        private void LevelUp()
        {
            if (m_Level < m_MaxLevel)
            {
                m_Level++;
                m_CurrentExp -= GetCurrentMaxExperience();
            }
            else
            {
                m_CurrentExp = GetCurrentMaxExperience(); // Cap experience at max level
            }
            UnityService.Instance.SaveData(m_CurrentLevelKey, m_Level);
            OnLevelUpInvoke();
        }

        public int GetCurrentMaxExperience()
        {
            return m_ExpTable[m_Level - 1];
        }
        public int GetCurrentLevel()
        {
            return m_Level;
        }
        public int GetCurrentExperience()
        {
            return m_CurrentExp;
        }

        private void GenerateExpTable()
        {
            m_ExpTable.Clear();
            m_ExpTable.Add(m_FirstLevelExp);
            for (int i = 1; i < m_MaxLevel; i++)
            {
                int exp = Mathf.FloorToInt(m_ExpTable[i - 1] * m_ExponentialGrowth);
                m_ExpTable.Add(exp);
            }
            m_ExpTable[0] = m_FirstLevelExp; // Ensure the first level experience is set correctly
        }
    }
}
