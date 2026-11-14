using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Rush
{
    internal enum PatchNoteKind { Section, Entry, Paragraph }

    internal readonly struct PatchNoteBlock
    {
        public readonly PatchNoteKind Kind;
        public readonly string Heading;
        public readonly string Body;

        public PatchNoteBlock(PatchNoteKind kind, string heading, string body)
        {
            Kind = kind;
            Heading = heading;
            Body = body;
        }
    }

    internal static class PatchNoteText
    {
        private static readonly Regex s_Emphasis = new(@"\*\*(.+?)\*\*|(?<!\*)\*([^*]+)\*");
        private static readonly Regex s_ListMarker = new(@"^(?:[-*•]|\d+[.)])\s+");

        internal static IEnumerable<PatchNoteBlock> Read(string title, string text)
        {
            string[] lines = (text ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            bool firstLine = true;
            foreach (string raw in lines)
            {
                string line = raw.Trim();
                if (line.Length == 0) continue;
                int offset = 0;
                // Older notes mark sections with emoji; TMP's UI font has no emoji glyphs.
                while (offset < line.Length && (char.IsSurrogate(line[offset]) || line[offset] == '\uFE0F' ||
                       (line[offset] >= '\u2600' && line[offset] <= '\u27BF'))) offset++;
                string content = line.Substring(offset).TrimStart();
                if (content.Length == 0) continue;
                string heading = Plain(content.TrimStart('#').Trim());
                bool duplicateTitle = firstLine && string.Equals(heading, title?.Trim(), StringComparison.OrdinalIgnoreCase);
                firstLine = false;
                if (duplicateTitle) continue;

                // A standalone bold heading also renders directly in the Inbox letter.
                if (content.StartsWith("<b>", StringComparison.Ordinal) && content.EndsWith("</b>", StringComparison.Ordinal))
                {
                    yield return new PatchNoteBlock(PatchNoteKind.Section, content.Substring(3, content.Length - 7), string.Empty);
                    continue;
                }
                if (offset > 0 || Regex.IsMatch(content, @"^#{1,6}\s"))
                {
                    yield return new PatchNoteBlock(PatchNoteKind.Section, heading, string.Empty);
                    continue;
                }
                var marker = s_ListMarker.Match(content);
                string sentence = marker.Success ? content.Substring(marker.Length) : content;
                var emphasis = s_Emphasis.Match(sentence);
                if (marker.Success && emphasis.Success)
                {
                    string name = emphasis.Groups[1].Success ? emphasis.Groups[1].Value : emphasis.Groups[2].Value;
                    string remainder = sentence.Substring(emphasis.Index + emphasis.Length).Trim();
                    string prefix = sentence.Substring(0, emphasis.Index).Trim();
                    string body = Plain(sentence);
                    // A trailing clause may stand alone under the feature name.
                    if (prefix.StartsWith("Added", StringComparison.OrdinalIgnoreCase))
                    {
                        if (remainder.StartsWith(", allowing players to ", StringComparison.OrdinalIgnoreCase))
                            body = remainder.Substring(", allowing players to ".Length);
                    }
                    body = char.ToUpperInvariant(body[0]) + body.Substring(1);
                    yield return new PatchNoteBlock(PatchNoteKind.Entry, name, body);
                }
                else
                {
                    yield return new PatchNoteBlock(PatchNoteKind.Paragraph, string.Empty, Plain(sentence));
                }
            }
        }

        private static string Plain(string text) =>
            s_Emphasis.Replace(text, match => match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value);
    }
}
