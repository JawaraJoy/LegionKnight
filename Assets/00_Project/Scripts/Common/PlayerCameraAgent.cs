using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerCameraAgent : MonoBehaviour
    {
        public void SetStayFollow(bool set)
        {
            //if (PlayerCamera.Instance == null) return;
            PlayerCamera.Instance.SetStayFollow(set);
        }
    }
}
