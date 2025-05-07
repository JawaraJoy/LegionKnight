using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class NameHandler : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<string> m_OnNameChanged = new();
        [SerializeField]
        private UnityEvent<string> m_OnNameFailToChanged = new();

        public async void ChangeName(string newName)
        {
            // Check if the player is authenticated
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogError("Player is not authenticated.");
                m_OnNameFailToChanged?.Invoke("Player is not authenticated.");
                return;
            }
            // Start the name change task
            await ChangeNameTask(newName);
        }
        private async Task ChangeNameTask(string newName)
        {
            // Validate the name
            if (string.IsNullOrEmpty(newName) || newName.Length > 20)
            {
                Debug.LogError("Name is invalid: It is either empty or exceeds the maximum length.");
                m_OnNameFailToChanged?.Invoke("Name is invalid or exceeds the maximum length.");
                return;
            }

            // Check if the name already exists in Unity Cloud Save
            bool nameExists = await CheckIfNameExists(newName);
            if (nameExists)
            {
                Debug.LogError($"Name '{newName}' is already taken by another player.");
                m_OnNameFailToChanged?.Invoke($"Name '{newName}' is already taken.");
                return;
            }

            // Update the player's name
            _ = AuthenticationService.Instance.UpdatePlayerNameAsync(newName).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log($"Player name updated successfully to '{newName}'.");

                    // Add the new name to Cloud Save
                    AddNameToCloudSave(newName);

                    m_OnNameChanged?.Invoke(newName);
                }
                else
                {
                    Debug.LogError($"Failed to update player name: {task.Exception}");
                    m_OnNameFailToChanged?.Invoke(task.Exception?.ToString() ?? "Failed to update player name.");
                }
            });
        }
        private async Task<bool> CheckIfNameExists(string name)
        {
            try
            {
                // Load the list of player names from Cloud Save
                var data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "PlayerNames" });

                if (data.TryGetValue("PlayerNames", out var playerNamesJson))
                {
                    // Deserialize the JSON string into a list of player names
                    var playerNames = JsonUtility.FromJson<PlayerNameList>(playerNamesJson.ToString());
                    return playerNames.Names.Contains(name);
                }

                // If no player names are found, the name is unique
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to check if name exists: {ex.Message}");
                return false; // Assume the name is valid if the check fails
            }
        }
        private async void AddNameToCloudSave(string newName)
        {
            try
            {
                // Load the existing list of player names
                var data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "PlayerNames" });

                List<string> playerNames;
                if (data.TryGetValue("PlayerNames", out var playerNamesJson))
                {
                    // Deserialize the JSON string into a list of player names
                    var playerNameList = JsonUtility.FromJson<PlayerNameList>(playerNamesJson.ToString());
                    playerNames = playerNameList.Names;
                }
                else
                {
                    // If no player names exist, create a new list
                    playerNames = new List<string>();
                }

                // Add the new name to the list
                if (!playerNames.Contains(newName))
                {
                    playerNames.Add(newName);

                    // Serialize the list back to JSON and save it to Cloud Save
                    var updatedPlayerNameList = new PlayerNameList { Names = playerNames };
                    var updatedJson = JsonUtility.ToJson(updatedPlayerNameList);
                    await CloudSaveService.Instance.Data.ForceSaveAsync(new Dictionary<string, object>
            {
                { "PlayerNames", updatedJson }
            });

                    Debug.Log($"Player name '{newName}' added to Cloud Save.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to add name to Cloud Save: {ex.Message}");
            }
        }

        [Serializable]
        private class PlayerNameList
        {
            public List<string> Names = new List<string>();
        }
    }
}
