using UnityEngine;

namespace LegionKnight
{
    public class LeaderboardAgent : MonoBehaviour
    {

        public void SubmiteScoreEntry(int score)
        {
            UnityService.Instance.SubmiteScoreEntry(score);
        }
        public void Init()
        {
            UnityService.Instance.Init();
        }
    }
}
