using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class FloatingDamageTextSpawner : MonoBehaviour
    {
        [SerializeField] private FloatingDamageTextConfig m_Config;

        private readonly List<FloatingDamageTextObject> m_Pool = new();

        public FloatingDamageTextConfig Config => m_Config;

        public void SpawnText(int amount)
        {
            if (amount <= 0) return;
            var obj = Get();
            obj.Setup(amount, this, GetSpawnPosition());
        }

        private FloatingDamageTextObject Get()
        {
            for (int i = 0; i < m_Pool.Count; i++)
            {
                if (!m_Pool[i].IsActive)
                    return m_Pool[i];
            }

            var newObj = Instantiate(m_Config.Prefab, transform);
            newObj.gameObject.SetActive(false);

            m_Pool.Add(newObj);

            return newObj;
        }

        private Vector3 GetSpawnPosition()
        {
            Vector2 spread = Random.insideUnitCircle * m_Config.SprayRadius;

            Vector3 pos = new Vector3(spread.x, spread.y, 0);
            pos.y = 0.5f;

            return transform.position + pos;
        }
    }
}