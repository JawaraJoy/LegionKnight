using DamageNumbersPro;
using UnityEngine;

namespace LegionKnight
{
    public partial class CoinNumber : MonoBehaviour
    {
        [SerializeField]
        private DamageNumberMesh m_CoinNumber;

        public void SpawnCoinNumber(int coin)
        {
            string number = $"+{coin}";
            m_CoinNumber.Spawn(transform.position, number, transform);
        }
    }
}
