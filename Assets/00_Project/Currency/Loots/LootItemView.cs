using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class LootItemView : ItemView
    {
        protected override void InitInternal(object defi)
        {
            base.InitInternal(defi);
            if (defi is LootField lootField)
            {
                ScriptableObject itemDef = lootField.Item;
                int amount = lootField.Amount;
                CurrencyApplier(itemDef, amount);
                CharacterApplier(itemDef);
                StandbyPlatformApplier(itemDef, amount);
                EnergyApplier(itemDef, amount);
                SetAmountInternal(amount);
            }
        }

        private void CurrencyApplier(ScriptableObject defi, int amount)
        {
            if (defi is CurrencyDefinition currency)
            {
                m_Icon.sprite = currency.Icon;
                //Player.Instance.AddCurrencyAmount(currency, amount);
            }
        }
        private void CharacterApplier(ScriptableObject defi)
        {
            if (defi is CharacterDefinition character)
            {
                m_Icon.sprite = character.SmallIcon;
                bool owned = Player.Instance.GetCharacterUnit(character).Owned;
                if (owned)
                {
                    StartCoroutine(CharcterDuplicated(character));
                }
            }
        }

        private IEnumerator CharcterDuplicated(CharacterDefinition character)
        {
            yield return new WaitForSeconds(1f);
            m_Icon.sprite = character.ShardConvert.CurrencyDefinition.Icon;
            m_Amount.text = character.ShardConvert.Amount.ToString();
            //Player.Instance.AddCurrencyAmount(character.ShardConvert.CurrencyDefinition, character.ShardConvert.Amount);
        }
        private void StandbyPlatformApplier(ScriptableObject defi, int amount)
        {
            if (defi is StandbyPlatformDefinition platform)
            {
                m_Icon.sprite = platform.Icon;
            }
        }
        private void EnergyApplier(ScriptableObject defi, int amount)
        {
            if (defi is EnergyDefinition energy)
            {
                m_Icon.sprite = energy.Icon;
            }
        }
    }
}
