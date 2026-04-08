using AppsFlyerSDK;
using MoreMountains.Tools;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public enum FormulaSelect
    {
        One,
        Two,
    }
    public partial class Progression : MonoBehaviour
    {
        [SerializeField]
        private int m_MaxLevel = 100;
        [SerializeField]
        private FormulaSelect m_Formula = FormulaSelect.One;
        [Header("Formula One")]
        [SerializeField]
        private float m_ExponentialGrowth = 2; // Example: 2 means the experience required for each level increases exponentially
        [SerializeField]
        private int m_FirstLevelExp = 100; // Experience required for the first level
        [Header("____________")]
        
        private int m_Level = 1;
        private int m_CurrentExp = 0;

        private const string m_CurrentExperienceKey = "Exp";
        private const string m_CurrentLevelKey = "Lv";

        [SerializeField]
        private List<ExpTable> m_ExpTable = new ();

        [SerializeField]
        private UnityEvent<int> m_OnCurrentExpChange = new ();
        [SerializeField]
        private UnityEvent<float> m_OnCurrentExpRateChange = new();
        [SerializeField]
        private UnityEvent<int> m_OnLevelUp = new ();

        [SerializeField, MMReadOnly]
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
            CanvasManager.Instance.InitLevelView();
        }
        public void Init()
        {
            InitInternal();
        }

        private void OnLevelUpInvoke()
        {
            m_OnLevelUp?.Invoke(m_Level);
            m_LevelUpTriggered = true;

            GetCurrentOnLevelUpEnter()?.Invoke();

            DateTime levelupDate = DateTime.Now;
            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
                {"playerid", UnityService.Instance.PlayerId},
                {"tolevel", m_Level.ToString()},
                {"levelupdate", levelupDate.ToString()},
            };
            AppsFlyer.sendEvent(AFEventName.OnPlayerLevelUp, eventValues);
        }
        public void AddOnCurrentExpChange(UnityAction<int> action)
        {
            m_OnCurrentExpChange.AddListener(action);
        }
        public void RemoveOnCurrentExpChange(UnityAction<int> action)
        {
            m_OnCurrentExpChange.RemoveListener(action);
        }
        public void AddOnCurrentExpRateChange(UnityAction<float> action)
        {
            m_OnCurrentExpRateChange.AddListener(action);
        }
        public void RemoveOnCurrentExpRateChange(UnityAction<float> action)
        {
            m_OnCurrentExpRateChange.RemoveListener(action);
        }

        private void OnCurrentExoRateChangeInvoke(float val)
        {
            m_OnCurrentExpRateChange?.Invoke(val);
            CanvasManager.Instance.InitLevelView();
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
                LevelUpPanel levelUpPanel = CanvasManager.Instance.GetPanel<LevelUpPanel>();
                CanvasManager.Instance.ShowLevelUpPanel();
                m_LevelUpTriggered = false;
                LootChestDefinition lootDef = m_ExpTable[m_Level - 1].RewardLevelReached;
                if (lootDef != null)
                {
                    //levelUpPanel.ShowRewardLevelUp(lootDef);
                }
            }
        }
        private void OnCurrentExpChangeInvoke()
        {
            m_OnCurrentExpChange?.Invoke(m_CurrentExp);
            CanvasManager.Instance.InitLevelView();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            //GenerateExpTable();
        }
#endif

        private float GetLevelProgressionRateInternal()
        {
            if (m_Level > 0 && m_Level <= m_ExpTable.Count)
            {
                return (float)m_CurrentExp / m_ExpTable[m_Level - 1].CurrentMaxExp;
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
            OnCurrentExpChangeInvoke();
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
            }
            else
            {
                m_CurrentExp = GetCurrentMaxExperience();
            }
        }

        public int GetCurrentMaxExperience()
        {
            if (m_Level - 1 < m_ExpTable.Count)
                return m_ExpTable[m_Level - 1].CurrentMaxExp;
            return m_ExpTable.Count > 0 ? m_ExpTable[^1].CurrentMaxExp : m_FirstLevelExp;
        }
        private UnityEvent GetCurrentOnLevelUpEnter()
        {
            return m_ExpTable[m_Level - 1].OnLevelUpEnter;
        }

        public int GetCurrentLevel()
        {
            return m_Level;
        }
        public int GetCurrentExperience()
        {
            return m_CurrentExp;
        }

        [ContextMenu("Generate")]
        private void GenerateExpTable()
        {
            switch(m_Formula)
            {
                case FormulaSelect.One:
                    FormulaGenerateOne();
                    break;
                case FormulaSelect.Two:
                    FormulaGenerateTwo();
                    break;
            }
            
        }

        private void FormulaGenerateOne()
        {
            m_ExpTable.Clear();
            int exp = m_FirstLevelExp;
            for (int i = 0; i < m_MaxLevel; i++)
            {
                ExpTable expTable = new(exp, null);
                m_ExpTable.Add(expTable);
                exp = Mathf.FloorToInt(exp * m_ExponentialGrowth);
            }
        }
        private void FormulaGenerateTwo()
        {
            m_ExpTable.Clear();
            int exp = 0;
            for (int i = 0; i < m_MaxLevel ; i++)
            {
                int currentLevel = i + 1;
                exp = Mathf.RoundToInt(10 * currentLevel * currentLevel + 490);
                ExpTable expTable = new(exp, null);
                m_ExpTable.Add(expTable);
            }
        }
        private Coroutine m_ExpGrowCoroutine;

        /// <summary>
        /// Slowly adds experience over time, animating the growth and handling level-ups.
        /// </summary>
        /// <param name="exp">Total experience to add</param>
        /// <param name="growSpeed">How much exp to add per second (default: 50)</param>
        public void AddExperienceSlowly(int exp, float growSpeed = 50f)
        {
            if (m_ExpGrowCoroutine != null)
                StopCoroutine(m_ExpGrowCoroutine);
            m_ExpGrowCoroutine = StartCoroutine(AddExperienceSlowlyCoroutine(exp, growSpeed));
        }

        private IEnumerator AddExperienceSlowlyCoroutine(int exp, float growSpeed)
        {
            int expToAdd = exp;
            while (expToAdd > 0)
            {
                int maxExp = GetCurrentMaxExperience();
                int expNeeded = maxExp - m_CurrentExp;

                // Calculate how much to add this frame
                int addThisFrame = Mathf.Min(expToAdd, Mathf.CeilToInt(growSpeed * Time.deltaTime));
                // Don't add more than needed for this level
                addThisFrame = Mathf.Min(addThisFrame, expNeeded);

                m_CurrentExp += addThisFrame;
                expToAdd -= addThisFrame;

                UnityService.Instance.SaveData(m_CurrentExperienceKey, m_CurrentExp);
                OnCurrentExpChangeInvoke();
                OnCurrentExoRateChangeInvoke(GetLevelProgressionRateInternal());

                // Handle level up
                if (m_CurrentExp >= maxExp && m_Level < m_MaxLevel)
                {
                    LevelUp();
                }

                yield return null;
            }
            m_ExpGrowCoroutine = null;
        }
    }
}
