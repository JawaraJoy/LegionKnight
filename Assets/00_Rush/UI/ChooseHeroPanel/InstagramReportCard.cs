using System;
using TMPro;
using UnityEngine;

namespace Rush
{
    public abstract class InstagramReportCard : MonoBehaviour
    {
        public const int Width = 1080;
        public const int Height = 1350;

        [SerializeField] private Camera m_Camera;
        private static readonly Vector3 s_CapturePosition = new Vector3(24000, 24000, 0);

        protected Texture2D CaptureInternal<T>(Action<T> bind) where T : InstagramReportCard
        {
            var instance = (T)Instantiate(this, s_CapturePosition, Quaternion.identity);
            try
            {
                instance.gameObject.SetActive(true);
                bind(instance);
                return instance.RenderImage();
            }
            finally
            {
                instance.gameObject.SetActive(false);
                Release(instance.gameObject);
            }
        }

        private Texture2D RenderImage()
        {
            var previous = RenderTexture.active;
            var target = RenderTexture.GetTemporary(Width, Height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            Texture2D image = null;
            try
            {
                m_Camera.targetTexture = target;
                Canvas.ForceUpdateCanvases();
                foreach (var label in GetComponentsInChildren<TMP_Text>()) label.ForceMeshUpdate();
                m_Camera.Render();
                RenderTexture.active = target;
                image = new Texture2D(Width, Height, TextureFormat.RGB24, false);
                image.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
                image.Apply(false, false);
                return image;
            }
            catch
            {
                if (image != null) Release(image);
                throw;
            }
            finally
            {
                m_Camera.targetTexture = null;
                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(target);
            }
        }

        private static void Release(UnityEngine.Object target)
        {
            if (Application.isPlaying) Destroy(target);
            else DestroyImmediate(target);
        }
    }
}
