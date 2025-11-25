using UnityEngine;

namespace LegionKnight
{
    public partial class CloudSaveManagerAgent : MonoBehaviour
    {
        public void DeleteAllData()
        {
            UnityService.Instance.DeleteAllData();
        }
        public void LoadAllData()
        {
            UnityService.Instance.LoadAllData();
        }
    }
}
