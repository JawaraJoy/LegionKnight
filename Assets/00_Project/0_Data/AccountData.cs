using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "AccountData", menuName = "Legion Knight/AccountData")]
    public class AccountData : ScriptableObject
    {
        [SerializeField, TextArea]
        private List<string> m_Data = new List<string>();
    }
}
