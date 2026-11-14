using System;
using System.Collections.Generic;
using System.Globalization;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class BattleReportCard : MonoBehaviour
    {
        public const int Width = 1080;
        public const int Height = 1350;

        [SerializeField] private Camera m_Camera;
        [SerializeField] private TMP_Text m_Hero, m_Rarity, m_Mode, m_Wave, m_Score, m_Experience, m_Empty;
        [SerializeField] private Image m_Portrait, m_Frame;
        [SerializeField] private RectTransform m_LootArea, m_Tile;
        [SerializeField] private Image m_TileFrame, m_TileIcon;
        [SerializeField] private TMP_Text m_TileName, m_TileAmount;
        private static readonly CultureInfo s_Numbers = CultureInfo.GetCultureInfo("en-US");

        public Texture2D Capture(UnitConfig hero, StageMode mode, int wave, int score, int experience, IReadOnlyList<LootField> loot)
        {
            var instance = Instantiate(this, new Vector3(24000, 24000, 0), Quaternion.identity);
            try
            {
                instance.gameObject.SetActive(true);
                instance.Bind(hero, mode, wave, score, experience, loot);
                return instance.RenderImage();
            }
            finally
            {
                instance.gameObject.SetActive(false);
                Release(instance.gameObject);
            }
        }

        private void Bind(UnitConfig hero, StageMode mode, int wave, int score, int experience, IReadOnlyList<LootField> loot)
        {
            var art = hero?.CollectibleField;
            var rarity = art?.RarityConfig;
            m_Portrait.sprite = art?.SplashImage != null ? art.SplashImage : art?.Icon;
            m_Portrait.enabled = m_Portrait.sprite != null;
            m_Hero.text = hero != null ? hero.BaseInfo.Name : "Unknown hero";
            m_Rarity.text = rarity != null ? rarity.BaseInfo.Name : string.Empty;
            m_Frame.color = rarity != null ? rarity.Color : Color.gray;
            m_Mode.text = mode == StageMode.Collosal ? "Colossal" : mode.ToString();
            m_Wave.text = "Wave " + wave.ToString("N0", s_Numbers);
            m_Score.text = score.ToString("N0", s_Numbers);
            m_Experience.text = "+" + experience.ToString("N0", s_Numbers);
            BindLoot(loot);
        }

        private void BindLoot(IReadOnlyList<LootField> loot)
        {
            var items = new List<CollectibleConfig>();
            var amounts = new Dictionary<CollectibleConfig, long>();
            if (loot != null)
            {
                foreach (var entry in loot)
                {
                    if (entry?.ItemLoot == null || entry.Amount <= 0) continue;
                    var item = entry.ItemLoot;
                    if (!amounts.TryGetValue(item, out var previous)) items.Add(item);
                    amounts[item] = previous + entry.Amount;
                }
            }
            m_Empty.gameObject.SetActive(items.Count == 0);
            if (items.Count == 0) return;
            var area = m_LootArea.rect.size;
            int columns = Mathf.Clamp(Mathf.CeilToInt(Mathf.Sqrt(items.Count * area.x / area.y)), 1, items.Count);
            int rows = Mathf.CeilToInt((float)items.Count / columns);
            float width = area.x / columns, height = area.y / rows;
            float scale = Mathf.Min(1, (width - 4) / m_Tile.rect.width, (height - 4) / m_Tile.rect.height);
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var field = item.CollectibleField;
                m_TileFrame.color = field?.RarityConfig != null ? field.RarityConfig.Color : Color.gray;
                m_TileIcon.sprite = field?.Icon;
                m_TileIcon.enabled = m_TileIcon.sprite != null;
                m_TileName.text = item.BaseInfo.Name;
                m_TileAmount.text = "×" + amounts[item].ToString("N0", s_Numbers);
                var tile = Instantiate(m_Tile, m_LootArea, false);
                tile.localScale = Vector3.one * Mathf.Max(.001f, scale);
                int row = i / columns;
                int inRow = Mathf.Min(columns, items.Count - row * columns);
                tile.anchoredPosition = new Vector2((area.x - inRow * width) * .5f + (i % columns + .5f) * width, -(row + .5f) * height);
                tile.gameObject.SetActive(true);
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