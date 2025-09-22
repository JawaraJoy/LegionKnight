using System.Collections.Generic;
using AppsFlyerSDK;
using Firebase.Messaging;


namespace LegionKnight
{
    public static class AppsFlyerEvents
    {
        // This function should be called when the game is launched.
        // It subscribes to the token received event, which is used for uninstall tracking.
        public static void Start()
        {
            FirebaseMessaging.TokenReceived += OnTokenReceived;
        }

        // This function is triggered when the token is received from Firebase.
        // It should be used for tracking uninstall tokens on Android devices.
        public static void OnTokenReceived(object sender, TokenReceivedEventArgs token)
        {
#if UNITY_ANDROID
            // Sends uninstall token to AppsFlyer on Android devices
            AppsFlyer.updateServerUninstallToken(token.Token);
#endif
        }

        // Session start event.
        // This should be called when a new session starts (when the app is launched).
        public static void SessionStart()
        {
            AppsFlyer.sendEvent("af_session_start", new Dictionary<string, string>());
        }


        // Call this function when a player returns to the game
        public static void TrackRetention(string playerId, int day)
        {
            var retentionEvent = new Dictionary<string, string>
            {
                { "player_id", playerId },
                { "retention_day", day.ToString() }  // 1, 7, or 30
            };

            AppsFlyer.sendEvent("retention", retentionEvent);
        }

        // Tracks the player’s general info like level, XP, energy, and currencies.
        // Call this function whenever player data needs to be updated (e.g., when they level up or check their stats).
        public static void TrackPlayerInfo(string playerId, int level, int totalXP, int energy, int coins, int diamonds, int tickets, int heroShards, List<Characters> characters)
        {
            // Track general player information (level, total XP, currencies)
            var playerInfoEvent = new Dictionary<string, string>
            {
                { "player_id", playerId },
                { "level", level.ToString() },
                { "total_xp", totalXP.ToString() },
                { "energy", energy.ToString() },
                { "coins", coins.ToString() },
                { "diamonds", diamonds.ToString() },
                { "tickets", tickets.ToString() },
                { "hero_shards", heroShards.ToString() }
            };

            // Send the player info event to AppsFlyer
            AppsFlyer.sendEvent("player_info", playerInfoEvent);

            // Track each character's information (name, level, XP, breakthrough phase, platform)
            foreach (var character in characters)
            {
                var characterEvent = new Dictionary<string, string>
                {
                    { "character_name", character.characterName },
                    { "character_level", character.level.ToString() },
                    { "character_rarity", character.rarity },
                    { "breakthrough_phase", character.breakthroughPhase },
                    { "helper_platform", character.helperPlatform }
                };

                // Log individual character-specific data
                AppsFlyer.sendEvent("character_data", characterEvent);
            }
        }

        // Tracks when the player levels up.
        // This function should be called whenever the player levels up, and you should pass the XP earned and the source of the level-up (e.g., Casual, Adventure).
        public static void TrackLevelUp(int level, int xpEarned, string source)
        {
            var levelUpEvent = new Dictionary<string, string>
            {
                { "level", level.ToString() },
                { "xp_earned", xpEarned.ToString() },
                { "source", source } // The source of level-up (e.g., Casual, Adventure, Boss Rush)
            };

            // Send the level-up event to AppsFlyer
            AppsFlyer.sendEvent("player_levelUp", levelUpEvent);
        }

        // Tracks the level-up of a specific character.
        // Call this function when a character levels up, passing the old and new levels, as well as the character's rarity.
        public static void TrackCharacterLevelUp(string characterName, int oldLevel, int newLevel, string characterRarity)
        {
            var levelUpEvent = new Dictionary<string, string>
            {
                { "character_name", characterName },
                { "old_level", oldLevel.ToString() },
                { "new_level", newLevel.ToString() },
                { "character_rarity", characterRarity }  // Normal, Rare, Epic
            };

            // Log the character's level-up event to AppsFlyer
            AppsFlyer.sendEvent("character_levelUp", levelUpEvent);
        }

        // Tracks character breakthrough progress.
        // This should be called when a character reaches a new breakthrough phase (BT1, BT2, or BT3).
        public static void TrackCharacterBreakthrough(string characterName, int currentLevel, int breakthroughPhase)
        {
            var breakthroughEvent = new Dictionary<string, string>
            {
                { "character_name", characterName },
                { "current_level", currentLevel.ToString() },
                { "breakthrough_phase", breakthroughPhase.ToString() }  // 1 = BT1, 2 = BT2, 3 = BT3
            };

            // Send the breakthrough event to AppsFlyer
            AppsFlyer.sendEvent("character_breakthrough", breakthroughEvent);
        }

        // Tracks the assignment of a helper platform to a character.
        // This should be called when a platform is assigned to a character (either automatically or manually).
        public static void TrackHelperPlatformAssignment(string platformType, string characterName)
        {
            var platformEvent = new Dictionary<string, string>
            {
                { "platform_type", platformType },  // Character platform, generic platform
                { "character_name", characterName }
            };

            // Send the platform assignment event to AppsFlyer
            AppsFlyer.sendEvent("helperPlatform_assigned", platformEvent);
        }

        // Tracks the usage of a helper platform in gameplay.
        // This should be called when a platform is used in any game mode (Casual, Adventure, Boss Rush).
        public static void TrackHelperPlatformUsage(string platformType, string characterName, string mode, int amountUsed)
        {
            var usageEvent = new Dictionary<string, string>
            {
                { "platform_type", platformType },       // Character-specific or Generic
                { "character_name", characterName },     // Name of character if it's a character-specific platform
                { "mode", mode },                        // Casual, Adventure, or Boss Rush
                { "amount_used", amountUsed.ToString() }  // Amount of the platform being used
            };

            // Log the helper platform usage event to AppsFlyer
            AppsFlyer.sendEvent("helper_platform_usage", usageEvent);
        }

        // Tracks when the tutorial is completed.
        // Call this function when the player finishes the tutorial.
        public static void TutorialComplete()
        {
            var values = new Dictionary<string, string>()
            {
                {"status", "complete" }
            };

            // Send the tutorial completion event to AppsFlyer
            AppsFlyer.sendEvent("starting-tutorial_complete", values);
        }

        // Casual mode completion event.
        // Call this function when a player completes Casual mode.
        public static void CasualModeComplete(CasualModeEventData casualModeEventData)
        {
            var values = new Dictionary<string, string>()
            {
                {"character_id", casualModeEventData.CharacterId },
                {"character_name", casualModeEventData.CharacterName },
                {"coins", casualModeEventData.Coin.ToString() },
                {"xp", casualModeEventData.Xp.ToString() },
                {"enemy_defeat", casualModeEventData.EnemiesDefeated.ToString() }
            };

            // Send the Casual mode completion event to AppsFlyer
            AppsFlyer.sendEvent("casual_mode_complete", values);
        }

        // Adventure mode start event.
        // Call this function when a player starts an Adventure mode session (starting a new floor).
        public static void AdventureModeStart(int floor)
        {
            var values = new Dictionary<string, string>()
            {
                {"floor_start_level", floor.ToString() }
            };

            // Send the Adventure mode start event to AppsFlyer
            AppsFlyer.sendEvent("adventure_mode_start", values);
        }

        // Tracks when a player completes a floor in Adventure mode.
        // Call this function when a player completes a floor and gets their rewards.
        public static void AdventureModeFloorComplete(int floorLevel, int coin, int score, string result)
        {
            var values = new Dictionary<string, string>()
            {
                {"floor_level", floorLevel.ToString() },
                { "coins", coin.ToString() },
                {"scores", score.ToString() },
                {"result", result.ToString() }
            };

            // Send the Adventure mode floor completion event to AppsFlyer
            AppsFlyer.sendEvent("adventure_mode_floor_complete", values);
        }

        // Tracks when a player defeats a boss.
        // Call this function when a player defeats a boss in Adventure or Boss Rush mode.
        public static void BossDefeat(string bossID, int bossFloor, int bossLevel, int duration, int coin, int score)
        {
            var values = new Dictionary<string, string>()
            {
                {"boss_id", bossID },
                {"boss_floor", bossFloor.ToString() },
                {"boss_level", bossLevel.ToString() },
                {"duration", duration.ToString() },
                {"coins", coin.ToString() },
                {"score", score.ToString() }
            };

            // Send the boss defeat event to AppsFlyer
            AppsFlyer.sendEvent("boss_defeat", values);
        }

        // Boss Rush mode start event.
        // Call this function when a player starts a Boss Rush mode session.
        public static void BossRushStart(string bossID)
        {
            var values = new Dictionary<string, string>()
            {
                {"boss_id", bossID }
            };

            // Send the Boss Rush mode start event to AppsFlyer
            AppsFlyer.sendEvent("boss_rush_start", values);
        }

        // Tracks when a player completes a boss fight in Boss Rush mode.
        // Call this function when a player completes a boss fight in Boss Rush mode.
        public static void BossRushComplete(string bossID, int bossLevel, int duration, int coin, int score)
        {
            var values = new Dictionary<string, string>()
            {
                {"boss_id", bossID },
                {"boss_level", bossLevel.ToString() },
                {"duration", duration.ToString() },
                {"coins", coin.ToString() },
                {"score", score.ToString() }
            };

            // Send the Boss Rush completion event to AppsFlyer
            AppsFlyer.sendEvent("boss_rush_complete", values);
        }

        // Tracking purchases made in the store.
        // Call this function when the player makes a purchase.
        public static void TrackInAppPurchase(string stockKeepingUnitID, double price, string currency, string transactionId, string store)
        {
            var values = new Dictionary<string, string>()
            {
                {"stockKeepingUnit_id", stockKeepingUnitID },
                {"price", price.ToString() },
                {"currency", currency },
                {"transaction_id", transactionId },
                {"store", store }
            };

            // Send the purchase event to AppsFlyer
            AppsFlyer.sendEvent("in_app_purchase", values);
        }

        // Tracks currency usage for in-game actions (e.g., gacha pulls, purchases, upgrades).
        // Call this function when the player uses currency (Coins, Diamonds, Tickets, Hero Shards).
        public static void TrackCurrencyUsage(string currencyType, int amount, string transactionType)
        {
            var currencyEvent = new Dictionary<string, string>
            {
                { "currency_type", currencyType },  // Coins, Diamonds, Tickets, Hero Shards
                { "amount", amount.ToString() },
                { "transaction_type", transactionType }  // Gacha Pull, Item Purchase, Upgrade
            };

            // Send the currency usage event to AppsFlyer
            AppsFlyer.sendEvent("currency_usage", currencyEvent);
        }

        // Ad impression tracking and ad revenues tracking.
        // Call this function when an ad is shown and you want to track its impression and revenue.
        public static void AdImpression(string network, string unit, string placement, double? revenueUSD)
        {
            var values = new Dictionary<string, string>
            {
                {"ad_network", network },
                {"ad_unit", unit },
                {"ad_placement", placement }
            };

            if (revenueUSD.HasValue)
            {
                values["ad_revenue"] = revenueUSD.Value.ToString("0.000000");
                values["ad_revenueCurrency"] = "USD";
            }

            // Log ad impression and optional revenue event to AppsFlyer
            AppsFlyer.sendEvent("ad_impression", values);
            if (revenueUSD.HasValue) AppsFlyer.sendEvent("af_ad_revenue", values);
        }

        // Tracks the daily sign-in event.
        // Call this function when the player signs in.
        public static void TrackDailySignIn()
        {
            var signInEvent = new Dictionary<string, string>
            {
                { "event", "daily_sign_in" }
            };

            // Send the daily sign-in event to AppsFlyer
            AppsFlyer.sendEvent("daily_sign_in", signInEvent);
        }

        // Tracks mission completion.
        // Call this function when the player completes a mission.
        public static void TrackMissionCompletion(string missionId, string missionType, string rewardType, int rewardAmount)
        {
            var missionEvent = new Dictionary<string, string>
            {
                { "mission_id", missionId },           // Mission ID (unique for each mission)
                { "mission_type", missionType },       // Daily, Weekly, Special, etc.
                { "reward_type", rewardType },         // Reward type (Coins, Diamonds, etc.)
                { "reward_amount", rewardAmount.ToString() }, // Amount of the reward
            };

            // Send the mission completion event to AppsFlyer
            AppsFlyer.sendEvent("mission_completion", missionEvent);
        }

        // Tracks achievement unlock.
        // Call this function when the player unlocks an achievement.
        public static void TrackAchievementUnlock(string achievementId, string achievementName, string achievementType, int completionTime)
        {
            var achievementEvent = new Dictionary<string, string>
            {
                { "achievement_id", achievementId },           // Unique ID for the achievement
                { "achievement_name", achievementName },       // Name of the achievement
                { "achievement_type", achievementType },       // Type (e.g., combat, exploration)
                { "completion_time", completionTime.ToString() } // Time taken to complete the achievement (e.g., days, hours)
            };

            // Send the achievement unlock event to AppsFlyer
            AppsFlyer.sendEvent("achievement_unlock", achievementEvent);
        }

        // Tracks the gacha pulls and results.
        // Call this function when the player makes a gacha pull.
        public static void TrackGachaSummaryWithItems(int totalPulls, int totalDiamondsSpent, int totalTicketsSpent, List<GachaItem> itemsPulled)
        {
            // Track summary event (total pulls, total currency spent, etc.)
            var gachaSummaryEvent = new Dictionary<string, string>
            {
                { "total_pulls", totalPulls.ToString() },
                { "total_diamonds_spent", totalDiamondsSpent.ToString() },
                { "total_tickets_spent", totalTicketsSpent.ToString() }
            };

            AppsFlyer.sendEvent("gacha_summary", gachaSummaryEvent);

            // Track individual item pulls (items pulled, rarity distribution)
            foreach (var item in itemsPulled)
            {
                var itemEvent = new Dictionary<string, string>
                {
                    { "item_name", item.itemName },             // Name of the item/character pulled
                    { "rarity", item.rarity },                  // Rarity (Normal, Rare, Epic)
                    { "pull_count", item.pullCount.ToString() }  // How many times this item was pulled
                };

                AppsFlyer.sendEvent("gacha_item_pulled", itemEvent);  // Log individual item pulled event
            }
        }
    }

    // Define event data for Casual Mode, Characters, Gacha Items, etc. 
    // Example classes to hold data like `CasualModeEventData`, `GachaItem`, and `Characters` are defined below.





    public class CasualModeEventData
    {
        public string CharacterId { get; private set; }
        public string CharacterName { get; private set; }
        public int Coin { get; private set; }
        public int Score { get; private set; }
        public int Xp { get; private set; }
        public int EnemiesDefeated { get; private set; }

        public CasualModeEventData (string charaterID, string characterName, int coin, int score, int xp, int enemiesDefeated)
        {
            CharacterId = charaterID;
            CharacterName = characterName;
            Coin = coin;
            Score = score;
            Xp = xp;
            EnemiesDefeated = enemiesDefeated;
        }
    }



    public class Characters
    {
        public string characterName;    // Name of the character (e.g., "Hero1")
        public int level;               // Current level of the character
        public string rarity;                  // Total XP of the character
        public string breakthroughPhase; // Breakthrough phase: BT1, BT2, BT3
        public string helperPlatform;   // Helper platform assigned to the character

        // Constructor to initialize character data
        public Characters(string name, int level, string rarity, string breakthroughPhase, string helperPlatform)
        {
            characterName = name;
            this.level = level;
            this.rarity = rarity;
            this.breakthroughPhase = breakthroughPhase;
            this.helperPlatform = helperPlatform;
        }
    }

    public class GachaItem
    {
        public string itemName;    // Name of the item/character (e.g., "Hero1", "Sword")
        public string rarity;      // Rarity of the item (e.g., "Normal", "Rare", "Epic")
        public int pullCount;      // Number of times this item was pulled

        // Constructor to initialize gacha item data
        public GachaItem(string name, string rarity, int pullCount)
        {
            itemName = name;
            this.rarity = rarity;
            this.pullCount = pullCount;
        }
    }
}
