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
                CurrencyApplier(d);
                CharacterApplier(d);
                PlatformApplier(d);
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
                bool owned = Player.Instance.GetCharacterUnit(character).Owned;
                if (owned)
                {
                    StartCoroutine(CharcterDuplicated(character));
                }
                /*bool owned = Player.Instance.GetCharacterUnit(character).Owned;
                if (owned)
                {
                    m_Icon.sprite = character.ShardIcon;
                    m_Amount.text = character.ShardAmount.ToString();
                }*/
            }
        }

        private IEnumerator CharcterDuplicated(CharacterDefinition character)
        {
            yield return new WaitForSeconds(1f);
            m_Icon.sprite = character.IconOnDuplicated;
            m_Amount.text = character.StartingStar.ToString();
        }
        private void PlatformApplier(ScriptableObject defi)
        {
            if (defi is StandbyPlatformDefinition platform)
            {
                m_Icon.sprite = platform.Icon;
            }
        }
    }
}
