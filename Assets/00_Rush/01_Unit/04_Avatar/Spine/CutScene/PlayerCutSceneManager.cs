using UnityEngine;

namespace LegionKnight
{
    public class PlayerCutSceneManager : PlayerCutScene
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerCutSceneManager m_PlayerCutSceneManager;
        public PlayerCutSceneManager PlayerCutSceneManager => m_PlayerCutSceneManager;
    }
}
