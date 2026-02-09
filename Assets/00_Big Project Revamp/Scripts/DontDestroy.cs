using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class DontDestroy : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
    
