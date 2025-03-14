using UnityEngine;

namespace LegionKnight
{
    public partial class BosEnemy : MonoBehaviour
    {
        private void Start()
        {
            
        }
        public void SetLocalPosition(Vector2 post)
        {
            transform.localPosition = post;
        }
    }
}
