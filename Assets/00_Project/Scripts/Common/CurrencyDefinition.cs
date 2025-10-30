using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Currency", menuName = "Legion Knight/Currency")]
    public class CurrencyDefinition : ScriptableObject, IDescriptable
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_Label;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField, TextArea]
        private string m_Description;
        [SerializeField]
        private CustomVariable<float>[] m_CustomVariables;
        public string Id => m_Id;
        public Sprite Icon => m_Icon;
        public string Description => m_Description;

        public string Label => m_Label;
        public CustomVariable<float>[] CustomVariables => m_CustomVariables;
        public CustomVariable<float> GetCustomVariable(string name)
        {
            foreach (CustomVariable<float> variable in m_CustomVariables)
            {
                if (variable.Name == name)
                {
                    return variable;
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class CustomVariable<T> where T : IComparable, IComparable<T>, IConvertible, IEquatable<T>
    {
        [SerializeField]
        private string m_Name;
        [SerializeField]
        private T m_Value;
        public string Name => m_Name;
        public T Value => m_Value;
    }
}
