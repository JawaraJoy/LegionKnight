using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

struct GameVersionData
{
    public string Title;
    public string Description;
    public string Version;
    public string Url;
}

public class NewUpdatePopUpUi : MonoBehaviour
{
    [Header("## UI Reference :")]
    [SerializeField]
    private GameObject uiCanvas;
    [SerializeField]
    private Button notNowBtn;
    [SerializeField]
    private Button updateBtn;
    //[SerializeField]
    //private TextMeshProUGUI uiDescription;
    [Space(29f)]
    [Header("## Settings :")]
    [SerializeField, TextArea(2, 5)]
    private string androidJsonDataURL;
    [SerializeField, TextArea(2, 5)]
    private string iosJsonDataURL;

    static bool isAlreadyCheckedForUpdates = false;

    GameVersionData LatestGameVersion;

    // Start is called before the first frame update
    void Start()
    {
        HidePopUp();
        if (!isAlreadyCheckedForUpdates)
        {
            StopAllCoroutines();
            StartCoroutine(CheckforUpdate());
        }
    }

    IEnumerator CheckforUpdate()
    {
        UnityWebRequest request;
#if UNITY_EDITOR || UNITY_ANDROID
        request = UnityWebRequest.Get(androidJsonDataURL);
#elif UNITY_IOS
        request = UnityWebRequest.Get(iosJsonDataURL);
#endif
        request.chunkedTransfer = false;
        request.disposeDownloadHandlerOnDispose = true;
        request.timeout = 60;
        Debug.Log("[Request] Try to request");
        yield return request.SendWebRequest();

        if (request.isDone)
        {
            isAlreadyCheckedForUpdates = true;
            Debug.Log("[Request] Done");
            Debug.Log($"[Request] result : {request.isNetworkError}, {request.isHttpError}, {request.isNetworkError} ");
            if (!request.isNetworkError)
            {
                Debug.Log($"[Request] Succes with : {request.downloadHandler.text}");
                LatestGameVersion = JsonUtility.FromJson<GameVersionData>(request.downloadHandler.text);
                if (!string.IsNullOrEmpty(LatestGameVersion.Version) && !Application.version.Equals(LatestGameVersion.Version))
                {
                    //uiDescription.text = LatestGameVersion.Description;
                    //Debug.Log($"[Request] Ui Description : {uiDescription.text}");
                    Debug.Log($"[Request] Description : {LatestGameVersion.Description}");
                    ShowPopUp();
                }
                if (!string.IsNullOrEmpty(LatestGameVersion.Version) && Application.version.Equals(LatestGameVersion.Version))
                {
                    HidePopUp();
                }
            }
        }
        request.Dispose();
    }

    void ShowPopUp()
    {
        notNowBtn.onClick.AddListener(() =>
        {
            HidePopUp();
        });
        updateBtn.onClick.AddListener(() =>
        {
            Application.OpenURL(LatestGameVersion.Url);
            HidePopUp();
        });

        uiCanvas.gameObject.SetActive(true);
    }

    void HidePopUp()
    {
        uiCanvas.gameObject.SetActive(false);
        notNowBtn.onClick.RemoveAllListeners();
        updateBtn.onClick.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
