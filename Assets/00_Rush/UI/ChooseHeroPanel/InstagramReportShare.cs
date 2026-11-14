using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Scripting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public abstract class InstagramReportShare : MonoBehaviour
    {
        [SerializeField] private GameObject m_Dialog;
        [SerializeField] private Button m_Open, m_Story, m_Post, m_Close;
        [SerializeField] private RawImage m_Preview;
        [SerializeField] private TMP_Text m_Status;
        private Texture2D m_Image;
        private string m_File;

        private bool m_Sending;
        private int m_Visit;
        protected virtual string RewardAccount => null;
        protected virtual void HandleLaunch(string account) { }
        protected virtual void HandleFailure(string outcome) { }

        public Button OpenButton => m_Open;

        protected virtual void Awake()
        {
            m_Story.onClick.AddListener(ShareStory);
            m_Post.onClick.AddListener(SharePost);
            m_Close.onClick.AddListener(CloseInternal);
        }

        protected void ShowInternal(Func<Texture2D> capture)
        {
            if (m_Dialog.activeSelf) return;
            m_Dialog.SetActive(true);
            try
            {
                m_Image = capture();
                m_Preview.texture = m_Image;
                m_Status.text = "Choose where to share your report.";
            }
            catch (Exception error)
            {
                Debug.LogException(error, this);
                m_Status.text = "Could not create the report. Close and try again.";
            }
            m_Story.interactable = m_Post.interactable = m_Image != null;
        }

        protected virtual void OnDisable() => CloseInternal();

        protected void CloseInternal()
        {
            m_Visit++;
            m_Dialog.SetActive(false);
            m_Preview.texture = null;
            if (m_Image != null)
            {
                if (Application.isPlaying) Destroy(m_Image);
                else DestroyImmediate(m_Image);
            }
            m_Image = null;
            m_File = null;
        }

        private void ShareStory() => Send(true);
        private void SharePost() => Send(false);

        private async void Send(bool story)
        {
            if (m_Image == null || m_Sending) return;
            m_Sending = true;
            int visit = m_Visit;
            string account = RewardAccount;
            string outcome;
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                using var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
                using var bridge = new AndroidJavaClass("com.aetherjump.share.BattleReportInstagram");
                if (m_File == null || !File.Exists(m_File))
                {
                    string path = bridge.CallStatic<string>("newReportPath", activity);
                    File.WriteAllBytes(path, m_Image.EncodeToPNG());
                    m_File = path;
                }
                var callback = new LaunchCallback();
                bridge.CallStatic("open", activity, m_File, story, account ?? string.Empty, callback);
                outcome = await callback.Completion.Task;
#else
                await Task.CompletedTask;
                outcome = "editor";
#endif
            }
            catch (Exception error)
            {
                Debug.LogWarning("[Instagram] " + error.Message);
                outcome = "failed";
            }
            finally { m_Sending = false; }
            if (this == null) return;
            if (outcome == "opened") HandleLaunch(account);
            if (visit != m_Visit) return;
            switch (outcome)
            {
                case "opened":
                    m_Status.text = story ? "Finish sharing in Instagram Stories." : "Finish sharing in Instagram Feed.";
                    break;
                case "cancelled": m_Status.text = "Sharing cancelled."; HandleFailure(outcome); break;
                case "saved": m_Status.text = "Image saved to Pictures/AetherJump."; HandleFailure(outcome); break;
                case "editor": m_Status.text = "Open the Android game to share to Instagram."; HandleFailure(outcome); break;
                case "missing": m_Status.text = "Install Instagram, then try again."; HandleFailure(outcome); break;
                default: m_Status.text = "Could not open Instagram. Please try again."; HandleFailure(outcome); break;
            }
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        [Preserve]
        private sealed class LaunchCallback : AndroidJavaProxy
        {
            internal readonly TaskCompletionSource<string> Completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
            internal LaunchCallback() : base("com.aetherjump.share.BattleReportInstagram$Callback") { }
            [Preserve] public void complete(string outcome) => Completion.TrySetResult(outcome);
        }
#endif
    }
}
