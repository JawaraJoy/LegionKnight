using UnityEngine;

namespace LegionKnight
{
    public partial class DrawItemView : ItemView
    {
        protected override void InitInternal(object defi)
        {
            base.InitInternal(defi);
            if (defi is GachaReward reward)
            {
                ScriptableObject d = reward.Definition;
                m_Amount.text = reward.Amount.ToString();
                CurrencyApplier(d);
                CharacterApplier(d);
            }
        }

        private void CurrencyApplier(ScriptableObject defi)
        {
            if (defi is CurrencyDefinition currency)
            {
                m_Icon.sprite = currency.Icon;
            }
        }
        private void CharacterApplier(ScriptableObject defi)
        {
            if (defi is CharacterDefinition character)
            {
                m_Icon.sprite = character.SmallIcon;
            }
        }
    }
}
