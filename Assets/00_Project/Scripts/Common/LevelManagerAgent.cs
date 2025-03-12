using UnityEngine;

namespace LegionKnight
{
    public partial class LevelManagerAgent : MonoBehaviour
    {
        public void Play()
        {
            GameManager.Instance.Play();
        }
        public void StartBos()
        {
            GameManager.Instance.StartBos();
        }
        public void SetLevelOver(bool set)
        {
            GameManager.Instance.SetLevelOver(set);
        }
        public void SpawnPlatform()
        {
            GameManager.Instance.SpawnPlatform();
        }
    }
}
