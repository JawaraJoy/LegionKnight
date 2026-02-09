using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class PadHandler : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private List<Pad> m_Pads = new();
        public List<Pad> Pads => m_Pads;

        public void RegisterPad(Pad pad)
        {
            if (!m_Pads.Contains(pad))
            {
                m_Pads.Add(pad);
            }
        }
        public void UnregisterPad(Pad pad)
        {
            if (m_Pads.Contains(pad))
            {
                m_Pads.Remove(pad);
            }
        }

        public Pad GetPadByDefinition(PadDefinition definition)
        {
            return m_Pads.Find(pad => pad.Definition.Id == definition.Id);
        }
    }
}
