using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace Rush
{
    public class GoogleSheetReviewApi : MonoBehaviour
    {
        [SerializeField]
        private string m_WebAppUrl;

        public void SubmitReview(ReviewRequest request, System.Action<ReviewResponse> callback)
        {
            StartCoroutine(SubmitRoutine(request,callback));
        }

        private IEnumerator SubmitRoutine(ReviewRequest request, System.Action<ReviewResponse> callback)
        {
            string json = JsonUtility.ToJson(request);

            byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

            UnityWebRequest requestWeb = new (m_WebAppUrl, UnityWebRequest.kHttpVerbPOST);

            requestWeb.uploadHandler = new UploadHandlerRaw(body);

            requestWeb.downloadHandler = new DownloadHandlerBuffer();

            requestWeb.SetRequestHeader("Content-Type", "application/json");

            yield return requestWeb.SendWebRequest();

            if (requestWeb.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(new ReviewResponse()
                    {
                        Success = true
                    });
            }
            else
            {
                callback?.Invoke(new ReviewResponse()
                    {
                        Success = false,
                        Message = requestWeb.error
                    });
            }
        }
    }
}