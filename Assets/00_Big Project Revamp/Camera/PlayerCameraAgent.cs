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
        public void SetOffsite(Vector3 set)
        {
            PlayerCamera.Instance.SetOffsite(set);
        }
        public void SetOffSite(CameraPostSetConfig config)
        {
            PlayerCamera.Instance.SetOffSite(config);
        }
    }
}
