using DamageNumbersPro;
using UnityEngine;

namespace LegionKnight
{
    public partial class TextMeshSpawner : MonoBehaviour
    {
        [SerializeField]
        private DamageNumberMesh m_TextMeshPrefab;

        [Header("Text")]
        [SerializeField]
        private string m_BeforeText;
        [SerializeField]
        private string m_AfterText;

        [Header("Settings")]
        [SerializeField]
        private float m_SprayRadius = 0.5f;
        [SerializeField]
        private float m_SizeText = 4.0f;

        private DamageNumber m_Spawned;

        private string GetText(object val)
        {

            return $"{m_BeforeText}{val}{m_AfterText}";
        }

        public void SpawnText(int val)
        {
            if (val <= 0) return;
            m_Spawned = m_TextMeshPrefab.Spawn(GetRadiusSpawnPosition(), GetText(val), transform);

            var textMesh = m_Spawned.GetTextMesh();
            if (textMesh == null) return;

            textMesh.fontSize = m_SizeText;

            m_Spawned.UpdateText();
        }

        private Vector3 GetRadiusSpawnPosition()
        {
            Vector2 spread = Random.insideUnitCircle * m_SprayRadius;
            Vector3 randomPos = new Vector3(spread.x, spread.y, 0);
            randomPos.y = 0.5f;

            return transform.position + randomPos;
        }
    }

    
}