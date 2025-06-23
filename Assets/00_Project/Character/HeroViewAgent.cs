using UnityEngine;

namespace LegionKnight
{
    public class HeroViewAgent : MonoBehaviour
    {
        public void OnCharacterLevelUpCharacterView(CharacterDefinition defi)
        {
            CharacterPanel cp = GameManager.Instance.GetPanel<CharacterPanel>();
            cp.SetCharacterSelected(defi);
        }
    }
}
