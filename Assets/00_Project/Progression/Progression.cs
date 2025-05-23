using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public partial class ExpTable
    {
        [SerializeField]
        private int m_ExpTarget;
        [SerializeField]
        UnityEvent m_OnLevelEnter = new();

        public ExpTable(int expTarget)
        {
            m_ExpTarget = expTarget;
        }

        public int ExpTarget => m_ExpTarget;

        public void OnLevelEnterInvoke()
        {
            m_OnLevelEnter?.Invoke();
        }
    }
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
        private List<ExpTable> m_ExpTable = new ();

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
                m_Level = 1;
            }
            if (UnityService.Instance.HasData(m_CurrentExperienceKey))
            {
                m_CurrentExp = UnityService.Instance.GetData<int>(m_CurrentExperienceKey);
            }
            else
            {
                m_CurrentExp = 0;
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
        }
        public void AddOnLevelUp(UnityAction<int> action)
        {
            m_OnLevelUp.AddListener(action);
        }
        public void RemoveOnLevelUp(UnityAction<int> action)
        {
            m_OnLevelUp.RemoveListener(action);
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            GenerateExpTable();
        }
#endif

        private float GetLevelProgressionRateInternal()
        {
            if (m_Level > 0 && m_Level <= m_ExpTable.Count)
            {
                return (float)m_CurrentExp / m_ExpTable[m_Level - 1].ExpTarget;
            }
            return 0f;
        }

        public float GetLevelProgressionRate()
        {
            return GetLevelProgressionRateInternal();
        }

        public void AddExperience(int exp)
        {
            m_CurrentExp += exp;
            while (m_Level < m_MaxLevel && m_CurrentExp >= GetCurrentMaxExperience())
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
                m_CurrentExp -= GetCurrentMaxExperience();
                m_Level++;
                UnityService.Instance.SaveData(m_CurrentLevelKey, m_Level);
                OnLevelUpInvoke();
                if (m_Level - 1 < m_ExpTable.Count)
                    m_ExpTable[m_Level - 1].OnLevelEnterInvoke();
            }
            else
            {
                m_CurrentExp = GetCurrentMaxExperience();
            }
        }

        public int GetCurrentMaxExperience()
        {
            if (m_Level - 1 < m_ExpTable.Count)
                return m_ExpTable[m_Level - 1].ExpTarget;
            return m_ExpTable.Count > 0 ? m_ExpTable[^1].ExpTarget : m_FirstLevelExp;
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
            int exp = m_FirstLevelExp;
            for (int i = 0; i < m_MaxLevel; i++)
            {
                m_ExpTable.Add(new ExpTable(exp));
                exp = Mathf.FloorToInt(exp * m_ExponentialGrowth);
            }
        }
    }
}
