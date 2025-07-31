using DamageNumbersPro;
using UnityEngine;

namespace LegionKnight
{
    public partial class TextMeshSpawner : MonoBehaviour
    {
        [SerializeField]
        private DamageNumberMesh m_TextMeshPrefab;
        [SerializeField]
        private string m_BeforeText;
        [SerializeField]
        private string m_AfterText;
        private DamageNumber m_Spawned;
        [SerializeField]
        private float m_SprayRadius = 0.5f;

        private string GetText(object val)
        {
            return $"{m_BeforeText}{val}{m_AfterText}";
        }
        public void SpawnText(int val)
        {
            m_Spawned = m_TextMeshPrefab.Spawn(GetRadiusSpawnPosition(), GetText(val), transform);
        }
        private Vector3 GetRadiusSpawnPosition()
        {
            Vector3 randomPos = Random.insideUnitSphere * m_SprayRadius;
            randomPos.y = 0.5f; // Ensure the text spawns above the ground
            return transform.position + randomPos;
        }
    }
}
