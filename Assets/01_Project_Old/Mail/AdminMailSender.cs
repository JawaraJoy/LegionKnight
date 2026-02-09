using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using UnityEngine;

namespace LegionKnight
{
    public class AdminMailSender : MonoBehaviour
    {
        public async Task SendTestMail(string playerId)
        {
            await UnityServices.InitializeAsync();

            // build args as a Dictionary<string, object>
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("playerId", playerId);
            args.Add("title", "Welcome Bonus");
            args.Add("message", "Thanks for playing!");
            args.Add("reward", "100 Coins");

            // Call Cloud Code and expect a dictionary response
            Dictionary<string, object> response =
                await CloudCodeService.Instance.CallEndpointAsync<Dictionary<string, object>>("sendMail", args);

            if (response != null && response.TryGetValue("success", out object successObj))
            {
                Debug.Log("Cloud Code sendMail.success: " + successObj.ToString());
            }
            else
            {
                Debug.Log("Cloud Code sendMail returned unexpected response or null. Full response: " + (response == null ? "null" : JsonUtility.ToJson(new SerializationHelper(response))));
            }
        }

        public async Task MarkMailReadViaCloudCode(string playerId, long mailId)
        {
            await UnityServices.InitializeAsync();

            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("playerId", playerId);
            args.Add("mailId", mailId);

            Dictionary<string, object> response =
                await CloudCodeService.Instance.CallEndpointAsync<Dictionary<string, object>>("markMailRead", args);

            if (response != null && response.TryGetValue("updated", out object updatedObj))
            {
                Debug.Log("Cloud Code markMailRead.updated: " + updatedObj.ToString());
            }
            else
            {
                Debug.Log("markMailRead returned unexpected response. Full response: " + (response == null ? "null" : JsonUtility.ToJson(new SerializationHelper(response))));
            }
        }
    }

    [System.Serializable]
    public class SerializationHelper
    {
        public Dictionary<string, object> dict;
        public SerializationHelper(Dictionary<string, object> dictionary) { dict = dictionary; }
    }
}
