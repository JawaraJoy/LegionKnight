using UnityEngine;

namespace Rush
{
    public interface IHasIcon
    {
        Sprite Icon { get; }
    }
    public interface IHasSplashImage
    {
        Sprite SplashImage { get; }
    }
}
