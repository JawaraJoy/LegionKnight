using Rush;
using UnityEngine;

namespace LegionKnight
{
    public partial class TouchDown : Contact2D
    {
        protected override void OnContactedBehaviourInvoke(IContactable other)
        {
            base.OnContactedBehaviourInvoke(other);
            if (other.GetSelf().TryGetComponent(out PlatformContact platform))
            {
                //Player.Instance.AddCurrencyAmount(platform.GetNormalTouchDown().CurrencyDefinition, platform.GetNormalTouchDown().Amount);
                LevelManager.Instance.SpawnPlatform();
                LevelManager.Instance.Up();
                LevelManager.Instance.AddAmount(platform.GetNormalTouchDown().Amount);
                platform.SetCanMove(false);
                platform.gameObject.SetActive(false);
            }
        }
    }
}
