using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace AppsDaddyO.TimeMan
{
    // Attached to GameObject timeManager for setting through inspector
    public class TimeManager : MonoBehaviour
    {
        public bool useOwnServer;
        public string ServerAddress;                // The ServerAddress where the current time is fetched
        [Header("REWARDS RESET TIME")]
        public int rewardHour;                      // The hour rewards should become available or reset if not claimed
        public int rewardMinute;                    // The minute rewards should become available or reset if not claimed
        public int rewardSecond;                    // The second rewards should become available or reset if not claimed
        
      // the current time as static variable for access accross scripts
        public static TimeManager timeMan;          // this timeManager script as a static variable for access accross scripts
        public static DateTime theCurrentTime;

        public GameObject rewardPanel,connectingPanel;
        private void Awake()
        {
            timeMan = this;                     // Set the Static Variable to this instance (timeManager)
        }
        // we create a coroutine which is required to get info from the internet with a callback action that returns the curent time. The callback is to be used in the RewardsManager. 
        private void Start()
        {
            StartCoroutine(GetCurrentTime());   // Start a coroutine to connect to server and get current time
        }
        public IEnumerator GetCurrentTime()
        {
            UnityWebRequest request = UnityWebRequest.Get(ServerAddress); // The Request to the server is created.
            yield return request.SendWebRequest(); // The request is created

            switch (request.result)
            {
                case UnityWebRequest.Result.InProgress:
                    break;
                case UnityWebRequest.Result.Success:
                    if (request.isDone) // If request was successful
                    {
                        connectingPanel.SetActive(false);
                        string data = request.downloadHandler.text; // download the text returned by the PHP file which is the current DateTime in string format.
                        if (!useOwnServer)
                        {
                            theCurrentTime = DateTime.Parse(ReturnStringFromWorldTimeApi(data)); // Convert the string received fron the request to a DateTime                        
                        }
                        else
                        {
                            theCurrentTime = DateTime.Parse(data); // Convert the string received fron the request to a DateTime
                        }

                        DailyReward.instance.CheckRewardData();
                    }
                    break;
                case UnityWebRequest.Result.ConnectionError:
                    connectingPanel.SetActive(true);
                    StartCoroutine(GetCurrentTime());
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    connectingPanel.SetActive(true);
                    StartCoroutine(GetCurrentTime());
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    connectingPanel.SetActive(true);
                    StartCoroutine(GetCurrentTime());
                    break;
                default:
                    break;
            }

            rewardPanel.SetActive(true);
        }
        string ReturnStringFromWorldTimeApi(string jsonText)
        {
            MyCustomDateTimeClass myObject = new MyCustomDateTimeClass();

            myObject = JsonUtility.FromJson<MyCustomDateTimeClass>(jsonText);

            return myObject.datetime;
        }
    }
}

[Serializable]
public class MyCustomDateTimeClass
{
    public string datetime;
}