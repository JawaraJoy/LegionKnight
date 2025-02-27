using UnityEngine;

namespace LegionKnight
{
    public partial class LevelManagerAgent : MonoBehaviour
    {
        public void Play()
        {
            GameManager.Instance.Play();
        }
    }
}
