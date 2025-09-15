using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "AccountData", menuName = "Legion Knight/AccountData")]
    public class AccountData : ScriptableObject
    {
        [SerializeField]
        private DataField[] m_DataFields;
    }

    [System.Serializable]
    public class DataField
    {
        [SerializeField]
        private string m_Title;
        [SerializeField, TextArea]
        private string m_Value;
    }
}
