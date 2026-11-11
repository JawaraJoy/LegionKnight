using UnityEngine;

namespace LegionKnight
{
    public class DebugHandler : MonoBehaviour
    {
        [SerializeField]
        private bool m_UseDebug = true;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Debug.unityLogger.logEnabled = m_UseDebug;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
