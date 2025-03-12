using System.Collections;
using UnityEngine;

namespace LegionKnight
{
    public partial class JumpToward : MonoBehaviour
    {
        [SerializeField]
        private Vector3 m_TowardJumpPulse;

        public void Jump()
        {
            //transform.position += m_TowardJumpPulse;
            StartCoroutine(Jumping());
        }
        private IEnumerator Jumping()
        {
            Vector3 destination = transform.position += m_TowardJumpPulse;
            float distance = Vector3.Distance(transform.position, destination);
            while(distance > 0.01f)
            {
                transform.position += m_TowardJumpPulse * Time.deltaTime;
            }
            yield return null;
        }
    }
}
