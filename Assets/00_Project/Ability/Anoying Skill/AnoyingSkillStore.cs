using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class AnoyingSkillStore : MonoBehaviour
    {
        private readonly List<Anoy> m_Anoys = new();


        private Anoy GetAnoyInternal(AnoyDefinition anoyDefinition)
        {
            Anoy anoy = m_Anoys.Find(a => a.AnoyDefinition == anoyDefinition);
            if (anoy == null)
            {
                Debug.LogError($"Anoy with definition {anoyDefinition.name} not found.");
                return null;
            }
            return anoy;
        }
        public void AddAnoy(IAnoy anoy)
        {
            Anoy existingAnoy = GetAnoyInternal(anoy.AnoyDefinition);
            if (existingAnoy == null)
            {
                existingAnoy = new Anoy(anoy.AnoyDefinition, anoy);
                m_Anoys.Add(existingAnoy);
            }
            else
            {
                Debug.LogWarning($"Anoy with definition {anoy.AnoyDefinition.name} already exists. Not adding again.");
            }
            existingAnoy.Init(anoy.AnoyDefinition, anoy);
        }
        public void RemoveAnoy(IAnoy anoy)
        {
            Anoy existingAnoy = GetAnoyInternal(anoy.AnoyDefinition);
            if (existingAnoy != null)
            {
                m_Anoys.Remove(existingAnoy);
            }
            else
            {
                Debug.LogWarning($"Anoy with definition {anoy.AnoyDefinition.name} not found. Cannot remove.");
            }
        }

        public void AddInterupt(AnoyDefinition anoyDefinition, int add)
        {
            Anoy anoy = GetAnoyInternal(anoyDefinition);
            if (anoy != null)
            {
                anoy.AddInterupt(add);
            }
            else
            {
                Debug.LogError($"Anoy with definition {anoyDefinition.name} not found. Cannot add interupt.");
            }
        }
    }
}
