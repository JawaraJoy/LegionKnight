using Rush;
using UnityEngine;

namespace LegionKnight
{
    public class Pad : MonoBehaviour
    {
        [SerializeField]
        private PadDefinition m_Definition;
        public PadDefinition Definition => m_Definition;

        private FlyCollectManager m_Manager;

        private FlyCollectManager Manager
        {
            get
            {
                if (m_Manager == null)
                {
                    m_Manager = RushGameManager.Instance.FlyCollectManager;
                }
                return m_Manager;
            }
        }

        private void OnEnable()
        {
            Manager.RegisterPad(this);
        }
        private void OnDisable()
        {
            //Manager.UnregisterPad(this);
        }
    }
}
