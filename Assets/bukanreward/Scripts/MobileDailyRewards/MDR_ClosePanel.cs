using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AppsDaddyO.Rewards
{
    public class MDR_ClosePanel : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            transform.parent.parent.parent.parent.gameObject.SetActive(false);
        }
    }
}
