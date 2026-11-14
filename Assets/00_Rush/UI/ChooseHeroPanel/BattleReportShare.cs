using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class BattleReportShare : MonoBehaviour
    {
        [SerializeField] private BattleReportCard m_Card;
        [SerializeField] private GameObject m_Dialog;
        [SerializeField] private Button m_Open, m_Story, m_Post, m_Close;
        [SerializeField] private RawImage m_Preview;
        [SerializeField] private TMP_Text m_Status;
        private Texture2D m_Image;
        private string m_File;

        public Button OpenButton => m_Open;

        private void Awake()
        {
            m_Story.onClick.AddListener(ShareStory);
            m_Post.onClick.AddListener(SharePost);
            m_Close.onClick.AddListener(Close);
        }

        public void Show(UnitConfig hero, StageMode mode, int wave, int score, int exp, IReadOnlyList<LootField> loot)
        {
            if (m_Dialog.activeSelf) return;
            m_Dialog.SetActive(true);
            try
            {
                m_Image = m_Card.Capture(hero, mode, wave, score, exp, loot);
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

        private void OnDisable() => Close();

        private void Close()
        {
            m_Dialog.SetActive(false);
            m_Preview.texture = null;
            if (m_Image != null) Destroy(m_Image);
            m_Image = null;
            m_File = null;
        }

        private void ShareStory() => Send(true);
        private void SharePost() => Send(false);

        private void Send(bool story)
        {
            if (m_Image == null) return;
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
                using var bridge = new AndroidJavaClass("com.aetherjump.share.BattleReportInstagram");
                if (m_File == null || !File.Exists(m_File))
                {
                    string path = bridge.CallStatic<string>("newReportPath", activity);
                    File.WriteAllBytes(path, m_Image.EncodeToPNG());
                    m_File = path;
                }
                m_Status.text = bridge.CallStatic<string>("open", activity, m_File, story);
            }
            catch (Exception error)
            {
                Debug.LogException(error, this);
                m_Status.text = "Could not open Instagram. Please try again.";
            }
#else
            m_Status.text = "Open the Android game to share to Instagram.";
#endif
        }
    }
}