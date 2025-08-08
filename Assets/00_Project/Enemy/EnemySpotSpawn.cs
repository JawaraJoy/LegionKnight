using System.Collections;
using UnityEngine;

namespace LegionKnight
{
    public class EnemySpotSpawn : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);
            GameManager.Instance.SetSpawningSpot(transform);
        }
    }
}
