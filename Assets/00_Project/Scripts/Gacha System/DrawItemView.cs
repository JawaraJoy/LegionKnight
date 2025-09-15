using System.Collections;
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
            yield return new WaitForSeconds(1f);
            m_Icon.sprite = character.ShardConvert.CurrencyDefinition.Icon;
            m_Amount.text = character.ShardConvert.Amount.ToString();
            Player.Instance.AddCurrencyAmount(character.ShardConvert.CurrencyDefinition, character.ShardConvert.Amount);
        }
        private void PlatformApplier(ScriptableObject defi, int amount)
        {
            if (defi is StandbyPlatformDefinition platform)
            {
                m_Icon.sprite = platform.Icon;
                Player.Instance.AddPlatformAmount(platform, amount);
            }
        }
    }
}
