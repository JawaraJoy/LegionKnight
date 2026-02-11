using UnityEngine;

namespace Rush
{
    public static class PlatformUtility
    {

        public static PlatformAbilityTriggerDirection GetPlatformDirection(Vector2 startPost, Vector2 finalDestination)
        {
            if (finalDestination.x - startPost.x >= 0)
            {
                return PlatformAbilityTriggerDirection.Right;
            }
            else
            {
                return PlatformAbilityTriggerDirection.Left;
            }
        }
        public static bool IsPerfectLanding(TouchDownCheck touchDown, Platform2D platform)
        {
            float perfectRangePlatform = platform.Config.PerfectTouchRange;
            float perfectRangeTouchDown = touchDown.PerfectTouchRange;
            float perfectRangeGlobal = RushGameManager.Instance.PlatformManager.GlobalPerfectTouchRange;

            float finalPerfectRange = perfectRangePlatform + perfectRangeTouchDown + perfectRangeGlobal;

            Vector2 perfectCenter = platform.TouchDownSpot.position;
            Vector2 playerPosition = touchDown.transform.position;

            float distance = Mathf.Abs(playerPosition.x - perfectCenter.x);

            bool isPerfect = distance <= finalPerfectRange;
            if (isPerfect)
            {
                touchDown.TouchDown.OnPerfectTouchDownInvoke(platform.Context);
                platform.TouchDown.OnPerfectTouchDownInvoke(platform.Context);
            }else
            {
                touchDown.TouchDown.OnNormalTouchDownInvoke(platform.Context);
                platform.TouchDown.OnNormalTouchDownInvoke(platform.Context);
            }
            return isPerfect;
        }public static bool IsTouchDownSuccess(TouchDownCheck touchDown, Platform2D platform)
        {
            float perfectRangePlatform = platform.Config.PerfectTouchRange;
            float perfectRangeTouchDown = touchDown.PerfectTouchRange;
            float perfectRangeGlobal = RushGameManager.Instance.PlatformManager.GlobalPerfectTouchRange;
            float finalPerfectRange = perfectRangePlatform + perfectRangeTouchDown + perfectRangeGlobal;
            Vector2 perfectCenter = platform.TouchDownSpot.position;
            Vector2 playerPosition = touchDown.transform.position;
            float distance = Mathf.Abs(playerPosition.x - perfectCenter.x);
            bool isSuccess = distance <= finalPerfectRange * 2f;
            return isSuccess;
        }
        public static float GetPlatformMoveSpeed(PlatformConfig platformConfig)
        {
            float baseSpeed = platformConfig.Speed;
            float globalSpeedRate = RushGameManager.Instance.PlatformManager.GlobalSpeedRate;
            return baseSpeed * globalSpeedRate;
        }
    }
}
