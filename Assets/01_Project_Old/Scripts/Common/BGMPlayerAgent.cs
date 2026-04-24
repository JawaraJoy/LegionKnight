using UnityEngine;
using Rush;

namespace LegionKnight
{
    public partial class BGMPlayerAgent : MonoBehaviour
    {
        public void PlayBGM()
        {
            GameSetting.Instance.AudioSetting.BGMPlayer.Play(true);
        }
        public void PlayBGM(AudioClip clip)
        {
            GameSetting.Instance.AudioSetting.BGMPlayer.Play(clip, true);
        }
        public void StopBGM()
        {
            GameSetting.Instance.AudioSetting.BGMPlayer.Stop();
        }
    }
}
