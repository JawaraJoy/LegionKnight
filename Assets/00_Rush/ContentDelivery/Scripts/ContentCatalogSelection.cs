using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Rush.ContentDelivery
{
    public sealed class ContentCatalogSelection : IDisposable
    {
        private AsyncOperationHandle<IResourceLocator> m_Handle;
        private IResourceLocator m_Locator;
        private IResourceLocator[] m_Previous;
        private bool m_Active;
        private bool m_Committed;
        private bool m_WasAlreadyVisible;
        public ContentReleaseManifest Manifest { get; }
        public IList<IResourceLocation> Downloads { get; private set; }
        public IResourceLocation Scene { get; private set; }
        public string Error { get; private set; }

        public ContentCatalogSelection(ContentReleaseManifest manifest) => Manifest = manifest;

        public IEnumerator Load(string path, string sceneKey, string downloadLabel)
        {
            Error = null;
            var visibleBeforeLoad = Addressables.ResourceLocators.ToArray();
            m_Handle = Addressables.LoadContentCatalogAsync(new Uri(path).AbsoluteUri, false);
            yield return m_Handle;
            if (m_Handle.Status != AsyncOperationStatus.Succeeded)
            { Error = "The content catalog could not be opened."; yield break; }
            m_Locator = m_Handle.Result;
            m_WasAlreadyVisible = visibleBeforeLoad.Contains(m_Locator);
            // Stage without exposing duplicate scene/GUID keys to the running client.
            if (!m_WasAlreadyVisible) Addressables.RemoveResourceLocator(m_Locator);
            if (!m_Locator.Locate(sceneKey, typeof(SceneInstance), out var scenes) || scenes.Count != 1 ||
                !m_Locator.Locate(downloadLabel, null, out var downloads) || downloads.Count == 0)
            { Error = "This content release is missing required game files."; yield break; }
            Scene = scenes[0];
            Downloads = downloads;
        }

        public void Activate()
        {
            if (m_Active || m_Locator == null || Error != null) return;
            m_Previous = Addressables.ResourceLocators.Where(l => l.GetType().Name != "DynamicResourceLocator").ToArray();
            foreach (var old in m_Previous) Addressables.RemoveResourceLocator(old);
            Addressables.AddResourceLocator(m_Locator);
            m_Active = true;
        }

        public void MarkReady() => m_Committed = true;

        public void Rollback()
        {
            if (!m_Active) return;
            Addressables.RemoveResourceLocator(m_Locator);
            foreach (var old in m_Previous)
                if (!Addressables.ResourceLocators.Contains(old)) Addressables.AddResourceLocator(old);
            m_Active = false;
        }

        public void Dispose()
        {
            if (!m_Committed) Rollback();
            if (m_Locator != null && !m_Committed && !m_WasAlreadyVisible) Addressables.RemoveResourceLocator(m_Locator);
            if (m_Handle.IsValid()) Addressables.Release(m_Handle);
        }
    }
}
