using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerAgent : MonoBehaviour
    {
        public void Init()
        {
            Player.Instance.Init();
        }
        public void ResetPosition()
        {
            Player.Instance.SetPosition(Vector2.zero);
            Player.Instance.Reborn();
        }
    }
}
