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
        public void SetOffSite(string postName)
        {
            PlayerCamera.Instance.SetOffSite(postName);
        }
    }
}
