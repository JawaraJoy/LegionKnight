using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class WeaponCounterView : UIView
    {
        private int m_Counter;

        [SerializeField]
        private List<Image> m_Actives = new();


        public void SetCounter(int set)
        {
            m_Counter = set;
            if (m_Counter > m_Actives.Count)
            {
                //m_Actives[i].gameObject.SetActive(true);
                return;
            }
            foreach (Image image in m_Actives)
            {
                image.gameObject.SetActive(false);
            }
            for (int i = 0; i < m_Counter; i++)
            {
                m_Actives[i].gameObject.SetActive(true);  
            }
        }
    }
    public partial class GameplayPanel
    {
        private WeaponCounterView GetWeaponCounter()
        {
            return GetBinding<WeaponCounterView>();
        }

        public void SetWeaponCounter(int set)
        {
            GetWeaponCounter().SetCounter(set);
        }
    }

    public partial class GameManager
    {
        public void SetWeaponCounterView(int set)
        {
            GetPanelInternal<GameplayPanel>().SetWeaponCounter(set);
        }
    }

    public partial class GameplayPanelAgent
    {
        public void SetWeaponCounterView(int set)
        {
            GameManager.Instance.SetWeaponCounterView(set);
        }
    }
}
