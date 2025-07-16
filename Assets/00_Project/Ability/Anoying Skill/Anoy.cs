using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public class Anoy
    {
        private AnoyDefinition m_AnoyDefinition;
        private int m_CurrentInterupCount;


        private IAnoy m_RegisteredAnoy;

        public AnoyDefinition AnoyDefinition => m_AnoyDefinition;

        public Anoy(AnoyDefinition anoyDefinition, IAnoy anoy)
        {
            m_AnoyDefinition = anoyDefinition;
            m_RegisteredAnoy = anoy;
        }
        public void Init(AnoyDefinition anoyDefinition, IAnoy anoy)
        {
            m_AnoyDefinition = anoyDefinition;
            m_RegisteredAnoy = anoy;
            m_CurrentInterupCount = 0;
            m_RegisteredAnoy?.OnInteruptUpdateInvoke(m_CurrentInterupCount);
        }

        public void Reset()
        {
            m_CurrentInterupCount = 0;
        }

        public void AddInterupt(int add)
        {
            if (m_AnoyDefinition == null)
            {
                Debug.LogError("AnoyDefinition is not initialized.");
                return;
            }
            m_CurrentInterupCount += add;
            m_RegisteredAnoy?.OnInteruptUpdateInvoke(m_CurrentInterupCount);
            if (m_CurrentInterupCount >= m_AnoyDefinition.InteruptDurability)
            {
                m_RegisteredAnoy?.StopAnoy();
                Debug.Log($"Anoy action triggered: {m_AnoyDefinition.AnoyName}");
                Reset(); // Reset after triggering
            }

        }
    }
}
