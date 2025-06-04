using MoreMountains.Tools;
using UnityEngine;

namespace LegionKnight
{
    public class PlayerHealthView : HealthView
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerHealthView m_HealthView;
        public void SetHealthBar(float set)
        {
            m_HealthView.SetHealthBar(set);
        }
        public void ShowHealth()
        {
            m_HealthView.Show();
        }
        public void HideHealth()
        {
            m_HealthView.Hide();
        }
    }
    public partial class PlayerAgent
    {
        public void SetHealthBar(float set)
        {
            Player.Instance.SetHealthBar(set);
        }
        public void ShowHealth()
        {
            Player.Instance.ShowHealth();
        }
        public void HideHealth()
        {
            Player.Instance.HideHealth();
        }
    }
}
