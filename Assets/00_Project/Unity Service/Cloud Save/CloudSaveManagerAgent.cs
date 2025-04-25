using UnityEngine;

namespace LegionKnight
{
    public partial class CloudSaveManagerAgent : MonoBehaviour
    {
        public void DeleteAllData()
        {
            UnityService.Instance.DeleteAllData();
        }
    }
}
