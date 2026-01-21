using System.Collections;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Level Event", menuName = "Legion Knight/Level Event")]
    public class LevelManagerEvent : ScriptableObject
    {
        public void StopLevel()
        {
            GameManager.Instance.SetLevelOver(true);
            Player.Instance.SetPause(true);
            Debug.Log("Level Stop");
        }
        public void StartLevel(float delay)
        {
            GameManager.Instance.StartCoroutine(StartingLevelDelay(delay));
        }

        private IEnumerator StartingLevelDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            GameManager.Instance.SetLevelOver(false);
            GameManager.Instance.SpawnPlatform();
            Player.Instance.SetPause(false);
            Debug.Log("Level Started");
        }
    }
}
