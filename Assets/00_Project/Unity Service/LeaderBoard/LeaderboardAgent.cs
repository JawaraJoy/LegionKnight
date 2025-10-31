using UnityEngine;

namespace LegionKnight
{
    public class LeaderboardAgent : MonoBehaviour
    {

        public void SubmitLeaderBoardScoreEntry(int score)
        {
            UnityService.Instance.SubmitScoreEntry(score);
        }
        public void Init()
        {
            UnityService.Instance.InitLeaderBoard();
        }
    }
}
