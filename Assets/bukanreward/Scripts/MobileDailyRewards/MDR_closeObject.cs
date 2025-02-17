using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppsDaddyO.Rewards
{
    public class MDR_closeObject : MonoBehaviour
    {
        // attached to the close button of the reward popup to close the gameObject
        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
