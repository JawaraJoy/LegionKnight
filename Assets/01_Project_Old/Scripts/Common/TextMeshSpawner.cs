using DamageNumbersPro;
using TMPro;
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
        [SerializeField]
        private float m_SizeText = 4.0f;

        private string GetText(object val)
        {
            return $"{m_BeforeText}{val}{m_AfterText}";
        }
        public void SpawnText(int val)
        {
            m_Spawned = m_TextMeshPrefab.Spawn(GetRadiusSpawnPosition(), GetText(val), transform);
            var textMesh = m_Spawned.GetTextMesh();
            if (textMesh == null) return;
            m_Spawned.GetTextMesh().fontSize = m_SizeText;
            m_Spawned.UpdateText();
        }
        private Vector3 GetRadiusSpawnPosition()
        {
            Vector3 spread = Random.insideUnitSphere * m_SprayRadius;
            Vector3 randomPos = new Vector3(spread.x, spread.y, 0);
            randomPos.y = 0.5f; // Ensure the text spawns above the ground
            return transform.position + randomPos;
        }
    }
}
