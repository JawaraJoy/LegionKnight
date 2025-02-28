using Rush;
using UnityEngine;

namespace LegionKnight
{
    public partial class Damageable : Contact2D
    {
        protected override void OnContactedBehaviourInvoke(IContactable other)
        {
            base.OnContactedBehaviourInvoke(other);

            if (other.GetSelf().TryGetComponent(out DamagePlatform platform))
            {
                //Player.Instance.AddCurrencyAmount(platform.GetNormalTouchDown().CurrencyDefinition, platform.GetNormalTouchDown().Amount);
                platform.SetCanContact(false);
            }
        }
    }
}
