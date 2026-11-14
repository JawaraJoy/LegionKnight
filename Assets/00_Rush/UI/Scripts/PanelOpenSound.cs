using System;
using LegionKnight;
using UnityEngine;

namespace Rush
{
    public sealed class PanelOpenSound : MonoBehaviour
    {
        [SerializeField] private AudioSource m_DefaultSound;
        [SerializeField] private PanelView[] m_PanelsWithOwnSound;

        public void PlayFor(PanelView panel)
        {
            // These panels play their own clip when their content becomes visible.
            if (Array.IndexOf(m_PanelsWithOwnSound, panel) >= 0)
            {
                // A reveal can open in the same frame as its parent preview.
                if (m_DefaultSound.isPlaying) m_DefaultSound.Stop();
                return;
            }

            m_DefaultSound.Play();
        }
    }
}
