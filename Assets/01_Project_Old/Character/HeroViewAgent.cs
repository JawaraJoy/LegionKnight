using UnityEngine;

namespace LegionKnight
{
    public class HeroViewAgent : MonoBehaviour
    {
        public void OnCharacterLevelUpCharacterView(CharacterDefinition defi)
        {
            CharacterPanel cp = CanvasManager.Instance.GetPanel<CharacterPanel>();
            cp.SetCharacterSelected(defi);
        }
    }
}
