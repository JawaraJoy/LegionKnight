using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    
    public partial class Character : MonoBehaviour
    {

        private CharacterDefinition CharacterDefinitionInternal => Player.Instance.CharacterDefinition;
    }
}
