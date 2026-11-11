using System.Globalization;
using TMPro;
using UnityEngine;

namespace Rush
{
    [ExecuteAlways]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class LeaderboardNumber : MonoBehaviour, ITextPreprocessor
    {
        private enum NumberFormat { Score, Rank, PaddedRank }
        [SerializeField] private NumberFormat m_Format;

        private void OnEnable()
        {
            var label = GetComponent<TextMeshProUGUI>();
            label.textPreprocessor = this;
            label.SetVerticesDirty();
        }

        private void OnDisable()
        {
            var label = GetComponent<TextMeshProUGUI>();
            if (label.textPreprocessor == this) label.textPreprocessor = null;
        }

        public string PreprocessText(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (m_Format == NumberFormat.Score)
            {
                return decimal.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out var score)
                    ? score.ToString("#,0.############################", CultureInfo.InvariantCulture) : text;
            }
            return text[0] == '#' && int.TryParse(text.Substring(1), out int rank)
                ? rank.ToString(m_Format == NumberFormat.PaddedRank ? "D2" : "D", CultureInfo.InvariantCulture) : text;
        }
    }
}
