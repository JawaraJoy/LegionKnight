using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public static class PlatformUtility
    {
        public static PlatformDirection GetPlatformDirection(Vector2 startPost, Vector2 finalDestination)
        {
            if (finalDestination.x - startPost.x >= 0)
            {
                return PlatformDirection.Right;
            }
            else
            {
                return PlatformDirection.Left;
            }
        }
        public static bool IsPerfectLanding(TouchDownCheck touchDown, Platform2D platform, float perfectRangeGlobal)
        {
            float perfectRangePlatform = platform.PlatformConfig.PerfectTouchRange;
            float perfectRangeTouchDown = touchDown.PerfectTouchRange;

            float finalPerfectRange = perfectRangePlatform + perfectRangeTouchDown + perfectRangeGlobal;

            Vector2 perfectCenter = platform.TouchDownSpot.position;
            Vector2 playerPosition = touchDown.transform.position;

            float distance = Mathf.Abs(playerPosition.x - perfectCenter.x);

            bool isPerfect = distance <= finalPerfectRange;
            
            return isPerfect;
        }
        
        public static float GetPlatformMoveSpeed(PlatformConfig platformConfig, float globalSpeedRate)
        {
            float baseSpeed = platformConfig.Speed;
            return baseSpeed * globalSpeedRate;
        }
        public static List<PlatformConfig> GetPlatformConfigWaitingListFromPreparationRandomly(PlatformConfig[] preparationConfigs, int inputCount)
        {
            List<PlatformConfig> result = new ();

            if (preparationConfigs == null || preparationConfigs.Length == 0 || inputCount <= 0)
                return result;

            // Hitung total weight
            float totalWeight = 0f;
            foreach (var config in preparationConfigs)
            {
                totalWeight += config.ChanceToSpawn;
            }

            for (int i = 0; i < inputCount; i++)
            {
                float randomValue = Random.Range(0f, totalWeight);
                float cumulative = 0f;

                foreach (var config in preparationConfigs)
                {
                    cumulative += config.ChanceToSpawn;

                    if (randomValue <= cumulative)
                    {
                        result.Add(config);
                        break;
                    }
                }
            }

            return result;
        }

        public static Vector2 GetStartingSpawnHorizontalPosition(float horizontalDistance, Vector2 lastContactPoint)
        {
            // Random kiri atau kanan
            bool spawnOnRight = Random.value < 0.5f;

            float direction = spawnOnRight ? 1f : -1f;

            Vector2 startPos = new (horizontalDistance * direction,lastContactPoint.y);

            return startPos;
        }
        public static List<SkillConfig> GetPlatformSkillConfigs(PlatformConfig config)
        {
            List<SkillConfig> skillConfigs = new List<SkillConfig>();
            SkillConfig[] leftNormal = config.LeftSkillField.OnNormalTouchSkill;
            SkillConfig[] leftPerfect = config.LeftSkillField.OnPerfectTouchSkill;
            SkillConfig[] rightNormal = config.RightSkillField.OnNormalTouchSkill;
            SkillConfig[] rightPerfect = config.RightSkillField.OnPerfectTouchSkill;
            SkillConfig normalAttack = config.AttackSkill;

            skillConfigs.AddRange(leftNormal);
            skillConfigs.AddRange(leftPerfect);
            skillConfigs.AddRange(rightNormal);
            skillConfigs.AddRange(rightPerfect);
            skillConfigs.Add(normalAttack);
            return skillConfigs;
        }
    }
}
