using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

namespace LegionKnight
{
    public class LeaderboardAgent : MonoBehaviour
    {

        public void SubmiteScoreEntry(int score)
        {
            UnityService.Instance.SubmiteScoreEntry(score);
        }
    }
}
