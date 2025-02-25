using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    public partial class InitScene : SceneHandler
    {
        [SerializeField]
        private string m_HomeSceneName;
        private void Start()
        {
            LoadSceneInternal(m_HomeSceneName);
        }
    }
}
