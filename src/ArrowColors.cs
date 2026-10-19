using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace NobleCompass
{
    /// <summary>The arrow's color: the fixed Arrow color, or a blend from Far to Near color as the noble gets closer.</summary>
    internal static class ArrowColors
    {
        private static readonly Dictionary<string, Color> Cache = new Dictionary<string, Color>();

        public static Color For(float distance)
        {
            if (!NcSettings.ColorByDistance)
                return Parse(NcSettings.ArrowColor, "#F2C97A");

            // 0 = right next to you, 1 = edge of the scouting range (also with unlimited range).
            float range = CompassTargetBehavior.ScoutRange();
            float t = range > 0.01f ? Math.Min(1f, distance / range) : 1f;
            return Blend(Parse(NcSettings.NearColor, "#5FE05F"), Parse(NcSettings.FarColor, "#E04A3A"), t);
        }

        /// <summary>#RRGGBB or #RRGGBBAA; anything else gives the fallback. Parsed once per distinct text.</summary>
        private static Color Parse(string text, string fallback)
        {
            string hex = (text ?? "").Trim().TrimStart('#');
            if (hex.Length == 6) hex += "FF";
            if (hex.Length != 8 || !hex.All(Uri.IsHexDigit)) hex = fallback.TrimStart('#') + "FF";

            if (!Cache.TryGetValue(hex, out Color color))
                Cache[hex] = color = Color.ConvertStringToColor("#" + hex);
            return color;
        }

        /// <summary>Blends around the color wheel the short way, so green to red passes yellow rather than brown.</summary>
        private static Color Blend(Color a, Color b, float t)
        {
            ToHsv(a, out float h1, out float s1, out float v1);
            ToHsv(b, out float h2, out float s2, out float v2);
            if (s1 < 0.01f) h1 = h2; // greys have no hue of their own
            if (s2 < 0.01f) h2 = h1;

            float dh = (h2 - h1 + 540f) % 360f - 180f;
            float h = (h1 + dh * t + 360f) % 360f;
            Color c = FromHsv(h, s1 + (s2 - s1) * t, v1 + (v2 - v1) * t);
            c.Alpha = a.Alpha + (b.Alpha - a.Alpha) * t;
            return c;
        }

        private static void ToHsv(Color c, out float h, out float s, out float v)
        {
            float max = Math.Max(c.Red, Math.Max(c.Green, c.Blue));
            float d = max - Math.Min(c.Red, Math.Min(c.Green, c.Blue));
            v = max;
            s = max > 0f ? d / max : 0f;
            if (d <= 0f) h = 0f;
            else if (max == c.Red) h = 60f * ((c.Green - c.Blue) / d % 6f);
            else if (max == c.Green) h = 60f * ((c.Blue - c.Red) / d + 2f);
            else h = 60f * ((c.Red - c.Green) / d + 4f);
            if (h < 0f) h += 360f;
        }

        private static Color FromHsv(float h, float s, float v)
        {
            float c = v * s, x = c * (1f - Math.Abs(h / 60f % 2f - 1f)), m = v - c;
            switch ((int)(h / 60f) % 6)
            {
                case 0: return new Color(c + m, x + m, m);
                case 1: return new Color(x + m, c + m, m);
                case 2: return new Color(m, c + m, x + m);
                case 3: return new Color(m, x + m, c + m);
                case 4: return new Color(x + m, m, c + m);
                default: return new Color(c + m, m, x + m);
            }
        }
    }
}
