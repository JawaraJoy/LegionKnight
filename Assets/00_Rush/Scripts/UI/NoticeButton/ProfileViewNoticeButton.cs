using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LegionKnight;

namespace Rush
{
    public class ProfileViewNoticeButton : NoticeButton
    {
        protected override bool HasNewContent()
        {
            List<ImageContent> list = new List<ImageContent>();
            ImageContent[] icons = Player.Instance.CustomProfile.Icons;
            ImageContent[] frames = Player.Instance.CustomProfile.Frames;
            foreach (ImageContent icon in icons)
            {
                list.Add(icon);
            }
            foreach (ImageContent frame in frames)
            {
                list.Add(frame);
            }
            bool hasNew = list.Any(x => x.Condition == ProductCondition.NewUnlocked);
            return hasNew;
        }
    }
}
