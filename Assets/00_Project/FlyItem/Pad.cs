using UnityEngine;

namespace LegionKnight
{
    public class Pad : MonoBehaviour
    {
        [SerializeField]
        private PadDefinition m_Definition;
        public PadDefinition Definition => m_Definition;

        private PadManager m_Manager;

        private PadManager Manager
        {
            get
            {
                if (m_Manager == null)
                {
                    m_Manager = GameManager.Instance.PadManager;
                }
                return m_Manager;
            }
        }

        private void Start()
        {
            Manager.RegisterPad(this);
        }
        private void OnDestroy()
        {
            Manager.UnregisterPad(this);
        }
    }
}
