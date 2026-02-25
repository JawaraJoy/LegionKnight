using Rush;
using UnityEngine;

namespace LegionKnight
{
    public class AppsFlyerEventManager : Singleton<AppsFlyerEventManager>
    {
        [SerializeField]
        private EnergyConfig m_Energy;
        [SerializeField]
        private ItemConfig m_Coin;
        [SerializeField]
        private ItemConfig m_Diamond;
        [SerializeField]
        private ItemConfig m_Tickets;
        [SerializeField]
        private ItemConfig m_Shards;
        [SerializeField]
        private ItemConfig m_Exp;
        [SerializeField]
        private ItemConfig m_Score;

        public ItemConfig Coin => m_Coin;
        public ItemConfig Diamond => m_Diamond;
        public ItemConfig Tickets => m_Tickets;
        public ItemConfig Shards => m_Shards;
        public ItemConfig Exp => m_Exp;
        public ItemConfig Score => m_Score;
        /*private void Start()
        {
            AppsFlyerEvents.Start();
        }
        public void SessionStart()
        {
            AppsFlyerEvents.SessionStart();
        }


        // Call this function when a player returns to the game
        public void TrackRetention(string playerId, int day)
        {
            //AppsFlyerEvents.TrackRetention(playerId, day);
        }

        // Tracks the player’s general info like level, XP, energy, and currencies.
        // Call this function whenever player data needs to be updated (e.g., when they level up or check their stats).
        private void TrackPlayerInfo()
        {
            string playerId = UnityService.Instance.PlayerId;
            int level = Player.Instance.GetPlayerLevel();
            int totalXp = Mathf.RoundToInt(Player.Instance.GetPlayerCurrentExp());
            int energy = Player.Instance.GetEnergy(m_Energy).Amount;
            int coins = Player.Instance.GetCurrencyAmount(m_Coin);
            int diamonds = Player.Instance.GetCurrencyAmount(m_Diamond);
            int tickets = Player.Instance.GetCurrencyAmount(m_Tickets);
            int heroShards = Player.Instance.GetCurrencyAmount(m_Shards);

            List<Characters> characters = new();
            List<CharacterUnit> characterUnits = new(Player.Instance.CharacterUnits);
            foreach (CharacterUnit characterUnit in characterUnits)
            {
                string nameC = characterUnit.CharacterName;
                int levelC = characterUnit.Level;
                string rarity = characterUnit.Definition.Rarity.ToString();
                string breakT = characterUnit.Star.ToString();
                string platform = Player.Instance.GetUsedStanbyPlatform().Label;
                Characters chara = new Characters(nameC, levelC, rarity, breakT, platform);
                characters.Add(chara);
            }
            AppsFlyerEvents.TrackPlayerInfo(playerId, level, totalXp, energy, coins, diamonds, tickets, heroShards, characters);
        }

        // Tracks when the player levels up.
        // This function should be called whenever the player levels up, and you should pass the XP earned and the source of the level-up (e.g., Casual, Adventure).
        public void TrackLevelUp(int level, int xpEarned, string source)
        {
            //AppsFlyerEvents.TrackLevelUp(level, xpEarned, source);
            //TrackPlayerInfo();
        }

        // Tracks the level-up of a specific character.
        // Call this function when a character levels up, passing the old and new levels, as well as the character's rarity.
        public void TrackCharacterLevelUp(string characterName, int oldLevel, int newLevel, string characterRarity)
        {
            //AppsFlyerEvents.TrackCharacterLevelUp(characterName, oldLevel, newLevel, characterRarity);
            //TrackPlayerInfo();
        }

        // Tracks character breakthrough progress.
        // This should be called when a character reaches a new breakthrough phase (BT1, BT2, or BT3).
        public void TrackCharacterBreakthrough(string characterName, int currentLevel, int breakthroughPhase)
        {
            //AppsFlyerEvents.TrackCharacterBreakthrough(characterName, currentLevel, breakthroughPhase);
            //TrackPlayerInfo();
        }

        // Tracks the assignment of a helper platform to a character.
        // This should be called when a platform is assigned to a character (either automatically or manually).
        public void TrackHelperPlatformAssignment(string platformType, string characterName)
        {
            //AppsFlyerEvents.TrackHelperPlatformAssignment(platformType, characterName);
        }

        // Tracks the usage of a helper platform in gameplay.
        // This should be called when a platform is used in any game mode (Casual, Adventure, Boss Rush).
        public void TrackHelperPlatformUsage(string platformType, string characterName, string mode, int amountUsed)
        {
            //AppsFlyerEvents.TrackHelperPlatformUsage(platformType, characterName, mode, amountUsed);
        }

        // Tracks when the tutorial is completed.
        // Call this function when the player finishes the tutorial.
        public void TutorialComplete()
        {
            //AppsFlyerEvents.TutorialComplete();
        }

        // Casual mode completion event.
        // Call this function when a player completes Casual mode.
        public void CasualModeComplete()
        {
            *//*CharacterUnit usedCharacter = Player.Instance.GetCharacterUnit(Player.Instance.UsedCharacter);
            List<LootField> looted = new (GameManager.Instance.GetLootStorageManager().Looteds);
            Currency coinGet = new Currency(m_Coin, 0);
            Currency expGet = new Currency(m_Exp, 0);
            Currency scoreGet = new Currency(m_Score, 0);
            foreach(LootField lootField in looted)
            {
                if (lootField.Item is CurrencyDefinition currency)
                {
                    if (currency == m_Coin)
                    {
                        coinGet.AddAmount(lootField.Amount);
                    }
                    if (currency == m_Exp)
                    {
                        expGet.AddAmount(lootField.Amount);
                    }
                    if (currency == m_Score)
                    {
                        scoreGet.AddAmount(lootField.Amount);
                    }
                }
            }
            CasualModeEventData data = new
                (
                    usedCharacter.Definition.Id,
                    usedCharacter.Definition.Label,
                    coinGet.Amount,
                    scoreGet.Amount,
                    expGet.Amount,
                    0
                );
            AppsFlyerEvents.CasualModeComplete(data);*//*
        }

        // Adventure mode start event.
        // Call this function when a player starts an Adventure mode session (starting a new floor).
        public void AdventureModeStart(int floor)
        {
            //AppsFlyerEvents.AdventureModeStart(floor);
        }

        // Tracks when a player completes a floor in Adventure mode.
        // Call this function when a player completes a floor and gets their rewards.
        public void AdventureModeFloorComplete(int floorLevel, int coin, int score, string result)
        {
            *//*AppsFlyerEvents.AdventureModeFloorComplete(floorLevel, coin, score, result);

            LevelDefinition level = GameManager.Instance.LevelDefinition;
            if (level == null) return;
            BosDefinition bos = level.BosDefinition;
            if (bos == null) return;
            BossDefeat(bos.Id, level.LevelPower, bos.StartLevel, 0, coin, score);*//*
        }

        // Tracks when a player defeats a boss.
        // Call this function when a player defeats a boss in Adventure or Boss Rush mode.
        private void BossDefeat(string bossID, int bossFloor, int bossLevel, int duration, int coin, int score)
        {
            AppsFlyerEvents.BossDefeat(bossID, bossFloor, bossLevel, duration, coin, score);
        }

        // Boss Rush mode start event.
        // Call this function when a player starts a Boss Rush mode session.
        public void BossRushStart()
        {
            *//*LevelDefinition level = GameManager.Instance.LevelDefinition;
            if (level == null) return;
            BosDefinition bos = level.BosDefinition;
            if (bos == null) return;
            if (level.IsInfiniteLevel)
            {
                AppsFlyerEvents.BossRushStart(bos.Id);
            }*//*
        }

        // Tracks when a player completes a boss fight in Boss Rush mode.
        // Call this function when a player completes a boss fight in Boss Rush mode.
        public void BossRushComplete(string bossID, int bossLevel, int duration, int coin, int score)
        {
            LevelDefinition level = GameManager.Instance.LevelDefinition;
            if (level == null) return;
            BosDefinition bos = level.BosDefinition;
            if (bos == null) return;
            List<LootField> looted = new(GameManager.Instance.GetLootStorageManager().Looteds);
            Currency coinGet = new Currency(m_Coin, 0);
            Currency scoreGet = new Currency(m_Score, 0);
            foreach (LootField lootField in looted)
            {
                if (lootField.Item is CurrencyDefinition currency)
                {
                    if (currency == m_Coin)
                    {
                        coinGet.AddAmount(lootField.Amount);
                    }
                    if (currency == m_Score)
                    {
                        scoreGet.AddAmount(lootField.Amount);
                    }
                }
            }
            if (level.IsInfiniteLevel)
            {
                AppsFlyerEvents.BossRushComplete(bos.Id, bos.StartLevel, 0, coinGet.Amount, scoreGet.Amount);
            }
        }

        // Tracking purchases made in the store.
        // Call this function when the player makes a purchase.
        public void TrackInAppPurchase(string stockKeepingUnitID, double price, string currency, string transactionId, string store)
        {
            AppsFlyerEvents.TrackInAppPurchase(stockKeepingUnitID, price, currency, transactionId, store);
            //TrackPlayerInfo();
        }

        // Tracks currency usage for in-game actions (e.g., gacha pulls, purchases, upgrades).
        // Call this function when the player uses currency (Coins, Diamonds, Tickets, Hero Shards).
        public void TrackCurrencyUsage(string currencyType, int amount, string transactionType)
        {
            //AppsFlyerEvents.TrackCurrencyUsage(currencyType, amount, transactionType);
            //TrackPlayerInfo();
        }

        // Ad impression tracking and ad revenues tracking.
        // Call this function when an ad is shown and you want to track its impression and revenue.
        public void AdImpression(string network, string unit, string placement, double? revenueUSD)
        {
            AppsFlyerEvents.AdImpression(network, unit, placement, revenueUSD);
        }

        // Tracks the daily sign-in event.
        // Call this function when the player signs in.
        public void TrackDailySignIn()
        {
            AppsFlyerEvents.TrackDailySignIn();
        }

        // Tracks mission completion.
        // Call this function when the player completes a mission.
        public void TrackMissionCompletion(string missionId, string missionType, string rewardType, int rewardAmount)
        {
            AppsFlyerEvents.TrackMissionCompletion(missionId, missionType, rewardType, rewardAmount);
        }

        // Tracks achievement unlock.
        // Call this function when the player unlocks an achievement.
        public void TrackAchievementUnlock(string achievementId, string achievementName, string achievementType, int completionTime)
        {
            AppsFlyerEvents.TrackAchievementUnlock(achievementId, achievementName, achievementType, completionTime);
        }

        // Tracks the gacha pulls and results.
        // Call this function when the player makes a gacha pull.
        public void TrackGachaSummaryWithItems(int totalPulls, int totalDiamondsSpent, int totalTicketsSpent, List<GachaItem> itemsPulled)
        {
            AppsFlyerEvents.TrackGachaSummaryWithItems(totalPulls, totalDiamondsSpent, totalTicketsSpent, itemsPulled);
        }*/
    }
}
