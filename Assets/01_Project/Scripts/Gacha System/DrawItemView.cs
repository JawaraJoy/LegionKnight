using Rush;
using System.Collections;
using Unity.Services.CloudSave.Models;
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
                CurrencyApplier(d, reward.Amount);
                CharacterApplier(d);
                PlatformApplier(d, reward.Amount);

                if (d is IDescriptable descriptable)
                {
                    string itemName = descriptable.Label;
                    if (gameObject.TryGetComponent(out TextView text))
                    {
                        text.SetText(itemName);
                    }
                }
            }
        }

        private void CurrencyApplier(ScriptableObject defi, int amount)
        {
            if (defi is CurrencyDefinition currency)
            {
                m_Icon.sprite = currency.Icon;
                Player.Instance.AddCurrencyAmount(currency, amount);
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
                else
                {
                    Player.Instance.SetOwned(character, true);
                }
            }
        }

        private IEnumerator CharcterDuplicated(CharacterDefinition character)
        {
            yield return new WaitForSeconds(1.5f);
            m_Icon.sprite = character.ShardConvert.CurrencyDefinition.Icon;
            m_Amount.text = character.ShardConvert.Amount.ToString();
            Player.Instance.AddCurrencyAmount(character.ShardConvert.CurrencyDefinition, character.ShardConvert.Amount);
            string itemName = character.ShardConvert.CurrencyDefinition.Label;
            if (gameObject.TryGetComponent(out TextView text))
            {
                text.SetText(itemName);
            }
        }
        private void PlatformApplier(ScriptableObject defi, int amount)
        {
            if (defi is PlatformConfig platform)
            {
                m_Icon.sprite = platform.Icon;
                Player.Instance.AddPlatformAmount(platform, amount);
            }
        }
    }
}
