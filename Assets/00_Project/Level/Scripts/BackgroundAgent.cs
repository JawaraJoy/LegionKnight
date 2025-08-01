using UnityEngine;

namespace LegionKnight
{
    public class BackgroundAgent : MonoBehaviour
    {
        public void InitializeBackground(LevelDefinition level)
        {
            GameManager.Instance.InitializeBackground(level);
        }
        public void SetBackgroundOrnament(OrnamentType ornament)
        {
            GameManager.Instance.SetBackgroundOrnament(ornament);
        }
    }
}
