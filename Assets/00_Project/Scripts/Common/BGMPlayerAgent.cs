using UnityEngine;

namespace LegionKnight
{
    public partial class BGMPlayerAgent : MonoBehaviour
    {
        public void PlayBGM()
        {
            GameSetting.Instance.PlayBGM();
        }
        public void PlayBGM(AudioClip clip)
        {
            GameSetting.Instance.PlayBGM(clip);
        }
        public void StopBGM()
        {
            GameSetting.Instance.StopBGM();
        }
    }
}
