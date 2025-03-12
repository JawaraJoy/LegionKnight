using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    
    public partial class Character : MonoBehaviour
    {
        [SerializeField]
        private CharacterSprite m_CharacterModel;

        private CharacterDefinition CharacterDefinitionInternal => Player.Instance.CharacterDefinition;

        private void Start()
        {
            if (CharacterDefinitionInternal == null) return;
            OnSetCharacterDefinitionInvoke(CharacterDefinitionInternal);
        }
        
        public void SetCharacterDefinition(CharacterDefinition definition)
        {
            OnSetCharacterDefinitionInvoke(definition);
        }

        private void OnSetCharacterDefinitionInvoke(CharacterDefinition definition)
        {
            m_CharacterModel.SetSprite(definition.Icon);
        }
    }
}
