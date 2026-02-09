using UnityEngine;
using System;
using System.Globalization;
using TMPro;

namespace LegionKnight
{
    

    public class DayCountUI : UIView
    {
        [Header("UI")]
        [SerializeField] private TMP_Text countdownText;

        [Header("Date Strings (format: yyyy-MM-dd HH:mm)")]
        [SerializeField] private string startDateString;
        [SerializeField] private string endDateString;

        private void Start()
        {
            Refresh();
        }
        protected override void ShowInternal()
        {
            base.ShowInternal();
            Refresh();
        }
        /// <summary>
        /// Call this function whenever you want to refresh the timer.
        /// </summary>
        private void Refresh()
        {
            DateTime now = DateTime.Now; // or DateTime.UtcNow (use consistently)

            if (!TryParseDate(startDateString, out DateTime startDate) ||
                !TryParseDate(endDateString, out DateTime endDate))
            {
                countdownText.text = "Invalid date";
                Debug.LogError("Date format must be: yyyy-MM-dd HH:mm");
                return;
            }

            // Event not started yet
            if (now < startDate)
            {
                TimeSpan untilStart = startDate - now;
                countdownText.text =
                    $"{untilStart.Days}d {untilStart.Hours}h {untilStart.Minutes}m";
                return;
            }

            // Event already ended
            if (now >= endDate)
            {
                countdownText.text = "0d 0h 0m";
                return;
            }

            // Event running (countdown)
            TimeSpan remaining = endDate - now;
            countdownText.text =
                $"{remaining.Days}d {remaining.Hours}h {remaining.Minutes}m";
        }

        private bool TryParseDate(string value, out DateTime result)
        {
            return DateTime.TryParseExact(
                value,
                "yyyy-MM-dd HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result
            );
        }
    }


}
