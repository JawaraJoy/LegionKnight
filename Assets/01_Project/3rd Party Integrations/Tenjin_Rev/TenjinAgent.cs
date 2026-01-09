using UnityEngine;

namespace LegionKnight
{
    public class TenjinAgent : MonoBehaviour
    {
        private TenjinManager tenjinManager;

        public void Init()
        {
            tenjinManager = GameObject.FindFirstObjectByType<TenjinManager>();
            if(tenjinManager)
                tenjinManager.Init();
        }

        public void SendEventToFirstJump()
        {
            if(TenjinManager.Instance && PlayerPrefs.GetInt("Record_FirstJump", 0) == 0)
            {
                TenjinManager.Instance.SendEvent("event_first_jump");
                PlayerPrefs.SetInt("Record_FirstJump", 1);
                PlayerPrefs.Save();
            }
        }

        public void SendEventToStartTutorial()
        {
            if(TenjinManager.Instance)
                TenjinManager.Instance.SendEvent("event_tutorial_start");
        }

        public void SendEventToEndTutorial()
        {
            if(TenjinManager.Instance)
                TenjinManager.Instance.SendEvent("event_tutorial_complete");
        }

        public void SendEventToAcquireCharacter()
        {
            //--TenjinRecord
            if(TenjinManager.Instance && PlayerPrefs.GetInt("Record_FirstHero", 0) == 0)
            {
                TenjinManager.Instance.SendEvent("event_first_hero_acquired");
                PlayerPrefs.SetInt("Record_FirstHero", 1);
                PlayerPrefs.Save();
            }
        }

        public void SendEventToUnlockMode(string mode)
        {
            if(TenjinManager.Instance && PlayerPrefs.GetInt("Record_UnlockMode_" + mode, 0) == 0)
            {
                TenjinManager.Instance.SendEvent("event_mode_unlocked_" + mode);
                PlayerPrefs.SetInt("Record_UnlockMode_" + mode, 1);
                PlayerPrefs.Save();
            }
        }

        public void SendEventToPurchaseStart(SellProduct product)
        {
            if (TenjinManager.Instance && TenjinManager.productIdCode.ContainsKey(product.Definition.Id))
            {
                TenjinManager.Instance.SendEvent("event_purchase_started", TenjinManager.productIdCode[product.Definition.Id].ToString());
            }
        }

        public void SendEventToPurchaseSuccess(SellProduct product)
        {
            if (TenjinManager.Instance && TenjinManager.productIdCode.ContainsKey(product.Definition.Id))
            {
                TenjinManager.Instance.SendEvent("event_purchase_success", TenjinManager.productIdCode[product.Definition.Id].ToString());
            }
        }

        public void SendEventToFirstSummon()
        {
            //--TenjinRecord
            if(TenjinManager.Instance && PlayerPrefs.GetInt("Record_FirstSummon", 0) == 0)
            {
                TenjinManager.Instance.SendEvent("event_first_summon");
                PlayerPrefs.SetInt("Record_FirstSummon", 1);
                PlayerPrefs.Save();
            }
        }

        public void SendEventToGachaPull(bool isMultiDraw)
        {
            if (TenjinManager.Instance)
                TenjinManager.Instance.SendEvent("event_gacha_pull", isMultiDraw ? "1" : "0");
        }

    }
}
