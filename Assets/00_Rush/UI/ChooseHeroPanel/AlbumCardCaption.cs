using System.Collections;
using LegionKnight;
using TMPro;
using UnityEngine;

namespace Rush
{
    public sealed class AlbumCardCaption : MonoBehaviour
    {
        [SerializeField] private CardSelectView m_Card;
        [SerializeField] private TextMeshProUGUI m_Caption;

        private IEnumerator Start()
        {
            while (m_Card.CardConfig == null) yield return null;
            m_Caption.text = m_Card.CardConfig.BaseInfo.Name;
        }
    }
}
