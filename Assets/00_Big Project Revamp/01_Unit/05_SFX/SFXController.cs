using UnityEngine;

namespace Rush
{
    public class SFXController : MonoBehaviour, IUnitExtension
    {
        private ModuleContext m_ModuleContext;
        public IModuleContext ModuleContext => m_ModuleContext;
        [SerializeField]
        private AudioSource m_AudioToPlay;
        public void Init(Unit unit)
        {
            m_ModuleContext = new ModuleContext(unit, gameObject);
        }

        public void PlayEntranceSFX()
        {
            // Play entrance SFX here
            AudioClip entranceClip = m_ModuleContext.Unit.Config.DefaultAudio.Entrance;
            if (entranceClip != null && m_AudioToPlay != null)
            {
                m_AudioToPlay.PlayOneShot(entranceClip);
            }
        }
    }
}
