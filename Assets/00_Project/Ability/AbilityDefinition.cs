using NUnit.Framework;
using UnityEngine;

namespace LegionKnight
{

    [CreateAssetMenu(fileName = "New Ability", menuName = "Legion Knight/Ability", order = 0)]
    public partial class AbilityDefinition : ScriptableObject, IObjectHasOwner
    {
        [SerializeField]
        private SkillOwner m_SkillOwner = SkillOwner.Player;
        [SerializeField]
        private AbilityDescription[] m_Descriptions;

        private Object m_Owner;
        public Object Owner => m_Owner;

        public string GetFinalDescription(CharacterUnit unit, int level)
        {
            if (m_Descriptions == null || m_Descriptions.Length == 0)
                return "No description available.";

            string description = "";
            for (int i = 0; i < m_Descriptions.Length; i++)
            {
                description += m_Descriptions[i].GetDescription(unit, level);
            }
            return description;
        }

        private GameObject GetOwnerInternal()
        {
            if (m_SkillOwner == SkillOwner.Player)
            {
                return Player.Instance.gameObject;
            }
            else if (m_SkillOwner == SkillOwner.Boss)
            {
                return GameManager.Instance.SpawnedBosenemy.gameObject;
            }
            else
            {
                Debug.LogError("Unknown skill owner type: " + m_SkillOwner);
                return null;
            }
        }
        public GameObject GetOwner()
        {
            return GetOwnerInternal();
        }

        public void SetOwner(Object owner)
        {
            m_Owner = owner;
        }
    }

    [System.Serializable]
    public class AbilityDescription : IObjectHasOwner
    {
        [SerializeField]
        private bool m_UseHeroAttackScale = false;
        [SerializeField]
        private float m_BaseVal;
        [SerializeField]
        private float m_UpgradeVal;

        [SerializeField, TextArea]
        private string m_Description;

        private Object m_Owner;
        public Object Owner => m_Owner;

        public string GetDescription(CharacterUnit unit, int level)
        {
            float finalVal = m_BaseVal + (m_UpgradeVal * (level - 1));
            if (m_UseHeroAttackScale)
            {
                finalVal = (float)GetHeroAttackScale(unit, level) + m_BaseVal + m_UpgradeVal * (level - 1);
            }
            return string.Format(m_Description, finalVal); // Use 'finalVal' here instead of recalculating
        }

        public void SetOwner(Object owner)
        {
            m_Owner = Owner;
        }

        private int GetHeroAttackScale(CharacterUnit unit, int level)
        {
            if (m_UseHeroAttackScale)
            {
                return unit.FinalStat().Attack;
            }
            return 0; // No hero attack scale applied
        }
    }
}
