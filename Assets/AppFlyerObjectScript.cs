using AppsFlyerSDK;
using UnityEngine;

namespace LegionKnight
{
    public class AppsFlyerObjectScript : MonoBehaviour
    {
        public string devkey;
        public string appid;

        private void Start()
        {
            AppsFlyer.initSDK(devkey, appid);
            AppsFlyer.startSDK();
        }
    }
}