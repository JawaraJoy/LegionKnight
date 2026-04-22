using UnityEngine;

namespace Rush
{
    public class DailySignInManager : DailySignInHandler { }

    public partial class RushPlayer
    {
        [SerializeField] private DailySignInManager m_DailySignInManager;
        public DailySignInManager DailySignInManager => m_DailySignInManager;
    }
}