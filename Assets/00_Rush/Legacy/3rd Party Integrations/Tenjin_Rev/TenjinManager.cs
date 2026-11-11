using Rush;
using System.Collections.Generic;
using Unity.Services.LevelPlay;
using UnityEngine;
using UnityEngine.Purchasing;

namespace LegionKnight
{   
    public class TenjinManager : Singleton<TenjinManager>, IUpdater
    {
        private BaseTenjin baseTenjin;
        private int floorProgression = 0;
        private float timeStartPlay = 0;
        private float recordTimeInterval = 1;
        private float lastRecordTime = 0;
        
        private bool progressionComplete = false;

        [SerializeField] private ItemConfig currencyCheat;

        private void Start()
        {
            Connect();
        }

        public static Dictionary<string, int> productIdCode = new Dictionary<string, int>()
        {
            ["beginner_bosst"] = 0,
            ["first_recharge"] = 1,
            ["hero_welcome_pack"] = 2,
            ["diamond_100"] = 3,
            ["diamond_250"] = 4,
            ["diamond_500"] = 5,
            ["diamond_1100"] = 6,
            ["diamond_2200"] = 7,
            ["diamond_5000"] = 8,
            ["diamond_11000"] = 9,
            ["daily_diamond_26"] = 10,
            ["diamond_50"] = 11,
            ["diamond_105"] = 12,
            ["diamond_220"] = 13,
            ["diamond_480"] = 14,
            ["diamond_1040"] = 15,
            ["diamond_2240"] = 16,
            ["diamond_99999"] = 17,
            ["killjoy_launch_starter_bundle"] = 18,
            ["killjoy_premium_profile_bundle"] = 19,
            ["killjoy_total_value_pack"] = 20,
            ["killjoy_breakthrough_pack"] = 21,
        };

        public static Dictionary<PurchaseFailureReason, int> failedReasonCode = new Dictionary<PurchaseFailureReason, int>()
        {
            [PurchaseFailureReason.PurchasingUnavailable] = 0,
            [PurchaseFailureReason.ExistingPurchasePending] = 1,
            [PurchaseFailureReason.ProductUnavailable] = 2,
            [PurchaseFailureReason.SignatureInvalid] = 3,
            [PurchaseFailureReason.UserCancelled] = 4,
            [PurchaseFailureReason.PaymentDeclined] = 5,
            [PurchaseFailureReason.DuplicateTransaction] = 6,
            [PurchaseFailureReason.ValidationFailure] = 7,
            [PurchaseFailureReason.StoreNotConnected] = 8,
            [PurchaseFailureReason.PurchaseMissing] = 9,
            [PurchaseFailureReason.Unknown] = 10
        };

        public bool IsActive => gameObject.activeInHierarchy;

        public void Init()
        {
            Debug.Log("***** TENJIN INIT *****");
            Debug.Log(Instance);

            if (!Instance)
            {
                timeStartPlay = Time.time;
                Instance.Connect();

                float playTime = PlayerPrefs.GetFloat("Record_PlayTime", 0);
                if (playTime > 0)
                {
                    SendEventToRecordPlayTime(playTime);
                    StartSession();
                }
                else
                {
                    StartSession();
                }
            }
        }

        void Cheat()
        {
            Debug.Log("***** TENJIN CHEAT *****");
            //Player.Instance.AddCurrencyAmount(currencyCheat, 1000000);
        }

        private void StartSession()
        {
            PlayerPrefs.SetFloat("Record_PlayTime", 0);    
            PlayerPrefs.Save();

            SendEvent("event_app_open");
        }

        private void Connect()
        {
            baseTenjin = Tenjin.getInstance("AEY4UUZVJHZ32RSTWM2CHQHWVSA1UWVI");
            baseTenjin.SetCustomerUserId(Player.Instance.PlayerName);

            // Sends install/open event to Tenjin
            baseTenjin.Connect();

            Debug.Log("***** TENJIN START *****");
        }

        public void StartRecordProgression()
        {
            Instance.floorProgression = 0;
            Instance.progressionComplete = false;
        }

        public void UpdatePlatformProgression()
        {
            Instance.floorProgression++;
        }

        public void StopRecordProgression()
        {
            if(!progressionComplete)
            {
                Instance.progressionComplete = true;
                Instance.SendEvent("event_floor_reached", Instance.floorProgression.ToString());
            }
        }

        public void SendEvent(string eventName, string value = null)
        {
            if(!baseTenjin)
            {
                Debug.LogError("Tenjin is not initiated");
                return;
            }

            if(string.IsNullOrWhiteSpace(eventName))
            {
                Debug.LogError("Event name cannot be empty");
                return;
            }

            if(value == null)
            {
                baseTenjin.SendEvent(eventName);
            }
            else
            {
                baseTenjin.SendEvent(eventName, value);
            }
        }

        public void SendEventToRecordPlayTime(float playtime)
        {
            int playtimeInSecond = Mathf.RoundToInt(playtime);

            SendEvent("event_session_end", playtimeInSecond.ToString());
            Debug.Log("Playtime: " + playtimeInSecond);
        }

        public void SendEventToStartTutorial()
        {
            Instance.SendEvent("event_tutorial_start");
        }

        public void SendEventToEndTutorial()
        {
            Instance.SendEvent("event_tutorial_complete");
        }

        public void SendEventToAcquireCharacter()
        {
            //--TenjinRecord
            if(PlayerPrefs.GetInt("Record_FirstHero", 0) == 0)
            {
                Instance.SendEvent("event_first_hero_acquired");
                PlayerPrefs.SetInt("Record_FirstHero", 1);
                PlayerPrefs.Save();
            }
        }
        
        public void SendEventToFirstJump()
        {
            if(PlayerPrefs.GetInt("Record_FirstJump", 0) == 0)
            {
                Instance.SendEvent("event_first_jump");
                PlayerPrefs.SetInt("Record_FirstJump", 1);
                PlayerPrefs.Save();
            }
        }

        public void SendEventToFirstSummon()
        {
            //--TenjinRecord
            if(PlayerPrefs.GetInt("Record_FirstSummon", 0) == 0)
            {
                Instance.SendEvent("event_first_summon");
                PlayerPrefs.SetInt("Record_FirstSummon", 1);
                PlayerPrefs.Save();
            }
        }

        public void SendEventToUnlockMode(string mode)
        {
            //--TenjinRecord
            if(PlayerPrefs.GetInt("Record_UnlockMode_" + mode, 0) == 0)
            {
                Instance.SendEvent("event_mode_unlocked");
                PlayerPrefs.SetInt("Record_UnlockMode_" + mode, 1);
                PlayerPrefs.Save();
            }
        }

        public void SendEventToCharacterBreakthrough(int tier)
        {
            Instance.SendEvent("event_breakthrough_unlocked", tier.ToString());
        }

        public void SendEventToUnlockAchievement(BadgeConfig badge)
        {
            if(badge)
            {
                string achievementId = badge.BaseInfo.Id;

                //--TenjinRecord
                if(PlayerPrefs.GetInt("Record_UnlockAchievement_" + achievementId, 0) == 0)
                {
                    Instance.SendEvent("event_achievement_unlocked_" + achievementId);
                    PlayerPrefs.SetInt("Record_UnlockAchievement_" + achievementId, 1);
                    PlayerPrefs.Save();
                }
            }
        }

        public void SendEventToHeroLevelUp(HeroUnitConfig heroConfig, int level)
        {
            if(heroConfig != null)
                Instance.SendEvent("event_hero_level_up_" + heroConfig.BaseInfo.Id, level.ToString());
        }

        public void SendEventToReEnergy()
        {
            Instance.SendEvent("event_energy_refilled");
        }

        
        public void SendEventToGachaPull(bool isMultiDraw)
        {
            Instance.SendEvent("event_gacha_pull", isMultiDraw ? "1" : "0");
        }

        public void SendEventToGachaPullType(List<GachaCollectableConfig> gachaRewards)
        {
            foreach (var gachaReward in gachaRewards)
            {
                Instance.SendEvent("event_gacha_pulltype", gachaReward.Collect.CollectibleField.RarityConfig.BaseInfo.Name);
            }
        }

        public void SendEventToAdRewarded(LevelPlayReward reward)
        {
            Instance.SendEvent("event_ad_rewarded", reward.ToString());
        }

        public void SendEventToAdShown(bool isIntersitial)
        {
            Instance.SendEvent("event_ad_shown", isIntersitial? "1" : "0");
        }

        private void OnEnable()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }
        public void Tick()
        {
            if (Instance)
            {
                if (Time.time - lastRecordTime > recordTimeInterval)
                {
                    lastRecordTime = Time.time;
                    PlayerPrefs.SetFloat("Record_PlayTime", Time.time - timeStartPlay);
                    PlayerPrefs.Save();
                }
            }
        }
    }
}
