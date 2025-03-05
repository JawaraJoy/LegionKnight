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

        private string GetText(object val)
        {
            return $"{m_BeforeText}{val}{m_AfterText}";
        }
        public void SpawnText(int val)
        {
            m_Spawned = m_TextMeshPrefab.Spawn(transform.position, GetText(val), transform);
        }
    }
}
