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
            Player.Instance.SetPosition(new Vector2(0, -2.4f));
            Player.Instance.Reborn();
        }
    }
}
