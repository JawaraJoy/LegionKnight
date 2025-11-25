using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Energy", menuName = "Legion Knight/Energy", order = 1)]
    public class EnergyDefinition : ScriptableObject, IDescriptable
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_Label; // name
        [SerializeField, TextArea]
        private string m_Description;
        [SerializeField]
        private Sprite m_Icon; // icon
        [SerializeField]
        private bool m_CanBreakMaxAmount = false; // Optional, if true, allows exceeding max amount temporarily
        [SerializeField]
        private int m_MaxAmount;
        [SerializeField]
        private bool m_CanRegen = false;
        [SerializeField]
        private int m_RegenEverySeconds = 1;
        [SerializeField]
        private int m_RegenAmount = 1;
        public string Id => m_Id;
        public string Label => m_Label;
        public Sprite Icon => m_Icon;
        public int MaxAmount => m_MaxAmount;
        public bool CanBreakMaxAmount => m_CanBreakMaxAmount;
        public int RegenEverEverySeconds => m_RegenEverySeconds;
        public int RegenAmount => m_RegenAmount;
        public bool CanRegen => m_CanRegen;
        public string Description => m_Description;

        public void AddEnergy(int amount)
        {
            Player.Instance.AddEnergy(this, amount);
        }
        public void SetEnergy(int amount)
        {
            Player.Instance.SetEnergy(this, amount);
        }
    }
}
