using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerPause : ObjectPause
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerPause m_PlayerPause;
        public void SetPause(bool set)
        {
            m_PlayerPause.SetPause(set);
        }
    }
}
