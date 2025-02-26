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
                GameManager.Instance.SpawnPlatform();
                GameManager.Instance.Up();

                GameManager.Instance.ApplyNormalReward(); // sementara normal, nanti bisa ditentukan apapkah perfect touch down atau normal

                platform.SetCanMove(false);
                platform.gameObject.SetActive(false);
                platform.SetActiveBehaviourCollider(false);

                GameManager.Instance.SetCurrentTouchDownPost(transform.position);
            }
        }
    }
}
