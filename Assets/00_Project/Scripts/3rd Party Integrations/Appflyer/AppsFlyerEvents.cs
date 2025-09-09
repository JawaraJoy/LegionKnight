using System.Collections.Generic;
using AppsFlyerSDK;


namespace LegionKnight
{
    public static class AppsFlyerEvents
    {
        //starting tutorial complete
        public static void TutorialComplete()
        {
            var values = new Dictionary<string, string>()
            {
                {"status", "complete" }
            };
            AppsFlyer.sendEvent("starting-tutorial_complete", values);
        }

        //casual mode play
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
        }

        //adventure mode start
        public static void AdventureModeStart(int floor)
        {
            var values = new Dictionary<string, string>()
            {
                {"floor_start_level", floor.ToString() }
            };
        }


        //floor complete
        public static void AdventureModeFloorComplete(int floorLevel, int coin, int score, string result)
        {
            var values = new Dictionary<string, string>()
            {
                {"floor_level", floorLevel.ToString() },
                { "coins", coin.ToString() },
                {"scores", score.ToString() },
                {"result", result.ToString() }
            };
        }

        //boss defeated
        public static void BossDefeat(string bossID, int bossFloor, int bossLevel, int duration, int coin, int score)
        {
            var values = new Dictionary<string, string>()
            {
                {"boss_id", bossID.ToString() },
                {"boss_floor", bossFloor.ToString() },
                {"boss_level", bossLevel.ToString() },
                {"duration", duration.ToString() },
                {"coins", coin.ToString() },
                {"score", score.ToString() }
            };
        }

        //boss rush mode start
        public static void BossRushStart(string bossID)
        {
            var values = new Dictionary<string, string>()
            {
                {"boss_id", bossID }
            };
        }

        //boss rush mode complete
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
        }


        //economics fungible
        public static void TrackCurrencyEarned ()
    }



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

    public class AdventureModeEventData
    {

    }



    public class Wallet
    {
        int coins { get; set; }
        int tickets { get; set; }
        int diamonds { get; set; }
        int shards_common { get; set; }
        int shards_rare { get; set; }
        int shards_epic { get; set; }

        public Wallet (int _coins, int _tickets, int _diamonds, int _shardCommon, int _shardRare, int _shardEpic)
        {
            coins = _coins;
            tickets = _tickets;
            diamonds = _diamonds;
            shards_common = _shardCommon;
            shards_rare = _shardRare;
            shards_epic = _shardEpic;
        }
    }
}
